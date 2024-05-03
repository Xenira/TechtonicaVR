using System.IO;
using BepInEx;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using TechtonicaVR.VRCamera;
using TechtonicaVR.Assets;
using UnityEngine.SceneManagement;
using PiUtils.Util;
using TechtonicaVR.Ik;
using TechtonicaVR.Input;
using PiVrLoader.Util;

namespace TechtonicaVR;

[BepInPlugin("de.xenira.techtonicavr", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Techtonica.exe")]
[BepInDependency("de.xenira.ttik", "0.2.2")]
[BepInDependency("de.xenira.pi_utils", "0.4.0")]
[BepInDependency("de.xenira.pi_vr_loader", "0.1.0")]
[BepInDependency("Tobey.UnityAudio", BepInDependency.DependencyFlags.SoftDependency)]
public class TechtonicaVR : BaseUnityPlugin
{
	private static PluginLogger Logger;

	//Create a class that actually inherits from MonoBehaviour
	public class VRLoader : MonoBehaviour
	{
	}

	//Variable reference for the class
	public static VRLoader staticVrLoader = null;

	private void Awake()
	{
		Logger = PluginLogger.GetLogger<TechtonicaVR>();

		Logger.LogInfo($"Loading plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION}...");
		License.LogLicense(Logger, "xenira", MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION);

		AssetLoader.assetLoader = new PiUtils.Assets.AssetLoader(Path.Combine(Path.GetDirectoryName(Info.Location), "assets"));

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

		var dllPath = Path.GetDirectoryName(Info.Location);
		PiVrLoader.PiVrLoader.CopyVrConfig(Path.Combine(dllPath, "vr_config", "StreamingAssets"), "StreamingAssets", true);
		PiVrLoader.PiVrLoader.CopyVrConfig(Path.Combine(dllPath, "vr_config", "UnitySubsystems"), "UnitySubsystems", true);

		ApplicationManifestHelper.UpdateManifest(Path.Combine(Paths.ManagedPath, "..", "StreamingAssets", "techtonicaVR.vrmanifest"),
				"steam.app.1457320",
				"https://steamcdn-a.akamaihd.net/steam/apps/1457320/header.jpg",
				"Techtonica VR",
				$"Techtonica VR mod {MyPluginInfo.PLUGIN_VERSION} by Xenira",
				steamBuild: true,
				steamAppId: 1457320);

		TechCameraManager.Create();

		StartCoroutine(AssetLoader.Load());

		IkSetup.SetupIk();

		Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

		TechInputMapper.MapActions();

		// Add listener for scene change
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// Handle scene change logic here
		Logger.LogDebug($"Scene {scene.name} loaded!");
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
}
