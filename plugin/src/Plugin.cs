using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using Unity.XR.OpenVR;
using Valve.VR;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR;
using TechtonicaVR.VRCamera;
using TechtonicaVR.Util;
using Plugin.Input;
using TechtonicaVR.Assets;
using UnityEngine.SceneManagement;

namespace TechtonicaVR;

[BepInPlugin("de.xenira.techtonica", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Techtonica.exe")]
public class Plugin : BaseUnityPlugin
{
	public static new ManualLogSource Logger;

	public static string gameExePath = Process.GetCurrentProcess().MainModule.FileName;
	public static string gamePath = Path.GetDirectoryName(gameExePath);
	public static string HMDModel = "";

	public static XRManagerSettings managerSettings = null;


	public static List<XRDisplaySubsystemDescriptor> displaysDescs = new List<XRDisplaySubsystemDescriptor>();
	public static List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();
	public static XRDisplaySubsystem MyDisplay = null;

	public static GameObject SecondEye = null;
	public static Camera SecondCam = null;

	//Create a class that actually inherits from MonoBehaviour
	public class VRLoader : MonoBehaviour
	{
	}

	//Variable reference for the class
	public static VRLoader staticVrLoader = null;

	private void Awake()
	{
		// Plugin startup logic
		Logger = base.Logger;
		Logger.LogInfo($"Loading plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION}...");

		Logger.LogInfo("TechtonicaVR Mod Copyright (C) 2023 Xenira");
		Logger.LogInfo("This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License version 3 as published by the Free Software Foundation.");
		Logger.LogInfo("This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.");
		Logger.LogInfo("You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.");

		ModConfig.Init(Config);

		if (!ModConfig.ModEnabled())
		{
			Logger.LogInfo("Mod is disabled, skipping...");
			return;
		}

		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

		if (staticVrLoader == null)
		{
			GameObject vrLoader = new GameObject("VRLoader");
			staticVrLoader = vrLoader.AddComponent<VRLoader>();
			DontDestroyOnLoad(staticVrLoader);
		}

		staticVrLoader.StartCoroutine(InitVRLoader());

		VRCameraManager.Create();

		new AssetLoader();

		Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

		// Add listener for scene change
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// Handle scene change logic here
		Logger.LogInfo($"Scene {scene.name} loaded!");
		if (scene.name == "Main Menu")
		{
			Logger.LogInfo("Scene is MainMenu, creating MainMenuPatch...");
			MainMenuPatch.Create();
		}
		else if (scene.name == "Loading")
		{
			Logger.LogInfo("Scene is LoadingScreen, creating LoadingScreenPatch...");
			LoadingScreenPatch.Create();
		}
	}

	public static System.Collections.IEnumerator InitVRLoader()
	{
		Logger.LogInfo("Initiating VRLoader...");

		SteamVR_Actions.PreInitialize();

		Logger.LogDebug("Creating XRGeneralSettings");
		var general = ScriptableObject.CreateInstance<XRGeneralSettings>();
		Logger.LogDebug("Creating XRManagerSettings");
		managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
		Logger.LogDebug("Creating OpenVRLoader");
		var xrLoader = ScriptableObject.CreateInstance<OpenVRLoader>();

		Logger.LogDebug("Setting OpenVR settings");
		var settings = OpenVRSettings.GetSettings();
		// settings.MirrorView = OpenVRSettings.MirrorViewModes.Right;
		settings.StereoRenderingMode = OpenVRSettings.StereoRenderingModes.MultiPass;

		Logger.LogDebug("Adding XRLoader to XRManagerSettings");
		general.Manager = managerSettings;
		managerSettings.loaders.Clear();
		managerSettings.loaders.Add(xrLoader);
		managerSettings.InitializeLoaderSync();

		XRGeneralSettings.AttemptInitializeXRSDKOnLoad();
		XRGeneralSettings.AttemptStartXRSDKOnBeforeSplashScreen();

		Logger.LogDebug("Initializing SteamVR");
		SteamVR.Initialize(true);

		ApplicationManifestHelper.UpdateManifest(Paths.ManagedPath + @"\..\StreamingAssets\techtonicaVR.vrmanifest",
				"steam.app.1457320",
				"https://steamcdn-a.akamaihd.net/steam/apps/1457320/header.jpg",
				"Techtonica VR",
				$"Techtonica VR mod {MyPluginInfo.PLUGIN_VERSION} by Xenira",
				steamBuild: true,
				steamAppId: 1457320);

		Logger.LogDebug("Getting XRDisplaySubsystemDescriptors");
		SubsystemManager.GetInstances(displays);
		Logger.LogDebug("Got " + displays.Count + " XRDisplaySubsystems");
		foreach (var display in displays)
		{
			Logger.LogDebug("Display running status: " + display.running);
			Logger.LogDebug("Display name: " + display.SubsystemDescriptor.id);
		}
		MyDisplay = displays[0];
		Logger.LogDebug("Starting XRDisplaySubsystem");
		MyDisplay.Start();
		Logger.LogDebug("After starting, display running status: " + MyDisplay.running);

		Logger.LogDebug("Getting HMD Model");
		HMDModel = SteamVR.instance.hmd_ModelNumber;
		Logger.LogInfo("SteamVR hmd modelnumber: " + HMDModel);

		SteamVR_Settings.instance.pauseGameWhenDashboardVisible = true;
		SteamVR_Settings.instance.autoEnableVR = true;

		SteamVRInputMapper.MapActions();

		Logger.LogInfo("Reached end of InitVRLoader");

		PrintSteamVRSettings();
		PrintOpenVRSettings();
		PrintUnityXRSettings();

		yield return null;

	}

	private static void PrintSteamVRSettings()
	{
		SteamVR_Settings settings = SteamVR_Settings.instance;
		if (settings == null)
		{
			Logger.LogWarning("SteamVR Settings are null.");
			return;
		}
		Logger.LogDebug("SteamVR Settings:");
		Logger.LogDebug("  actionsFilePath: " + settings.actionsFilePath);
		Logger.LogDebug("  editorAppKey: " + settings.editorAppKey);
		Logger.LogDebug("  activateFirstActionSetOnStart: " + settings.activateFirstActionSetOnStart);
		Logger.LogDebug("  autoEnableVR: " + settings.autoEnableVR);
		Logger.LogDebug("  inputUpdateMode: " + settings.inputUpdateMode);
		Logger.LogDebug("  legacyMixedRealityCamera: " + settings.legacyMixedRealityCamera);
		Logger.LogDebug("  mixedRealityCameraPose: " + settings.mixedRealityCameraPose);
		Logger.LogDebug("  lockPhysicsUpdateRateToRenderFrequency: " + settings.lockPhysicsUpdateRateToRenderFrequency);
		Logger.LogDebug("  mixedRealityActionSetAutoEnable: " + settings.mixedRealityActionSetAutoEnable);
		Logger.LogDebug("  mixedRealityCameraInputSource: " + settings.mixedRealityCameraInputSource);
		Logger.LogDebug("  mixedRealityCameraPose: " + settings.mixedRealityCameraPose);
		Logger.LogDebug("  pauseGameWhenDashboardVisible: " + settings.pauseGameWhenDashboardVisible);
		Logger.LogDebug("  poseUpdateMode: " + settings.poseUpdateMode);
		Logger.LogDebug("  previewHandLeft: " + settings.previewHandLeft);
		Logger.LogDebug("  previewHandRight: " + settings.previewHandRight);
		Logger.LogDebug("  steamVRInputPath: " + settings.steamVRInputPath);
	}

	private static void PrintOpenVRSettings()
	{
		OpenVRSettings settings = OpenVRSettings.GetSettings(false);
		if (settings == null)
		{
			Logger.LogWarning("OpenVRSettings are null.");
			return;
		}
		Logger.LogDebug("OpenVR Settings:");
		Logger.LogDebug("  StereoRenderingMode: " + settings.StereoRenderingMode);
		Logger.LogDebug("  InitializationType: " + settings.InitializationType);
		Logger.LogDebug("  ActionManifestFileRelativeFilePath: " + settings.ActionManifestFileRelativeFilePath);
		Logger.LogDebug("  MirrorView: " + settings.MirrorView);

	}

	private static void PrintUnityXRSettings()
	{
		Logger.LogDebug("Unity.XR.XRSettings: ");
		Logger.LogDebug("  enabled: " + XRSettings.enabled);
		Logger.LogDebug("  deviceEyeTextureDimension: " + XRSettings.deviceEyeTextureDimension);
		Logger.LogDebug("  eyeTextureDesc: " + XRSettings.eyeTextureDesc);
		Logger.LogDebug("  eyeTextureHeight: " + XRSettings.eyeTextureHeight);
		Logger.LogDebug("  eyeTextureResolutionScale: " + XRSettings.eyeTextureResolutionScale);
		Logger.LogDebug("  eyeTextureWidth: " + XRSettings.eyeTextureWidth);
		Logger.LogDebug("  gameViewRenderMode: " + XRSettings.gameViewRenderMode);
		Logger.LogDebug("  isDeviceActive: " + XRSettings.isDeviceActive);
		Logger.LogDebug("  loadedDeviceName: " + XRSettings.loadedDeviceName);
		Logger.LogDebug("  occlusionMaskScale: " + XRSettings.occlusionMaskScale);
		Logger.LogDebug("  renderViewportScale: " + XRSettings.renderViewportScale);
		Logger.LogDebug("  showDeviceView: " + XRSettings.showDeviceView);
		Logger.LogDebug("  stereoRenderingMode: " + XRSettings.stereoRenderingMode);
		Logger.LogDebug("  supportedDevices: " + XRSettings.supportedDevices);
		Logger.LogDebug("  useOcclusionMesh: " + XRSettings.useOcclusionMesh);
	}


}
