using PiUtils.Objects.Behaviours;
using PiUtils.Util;
using TechtonicaVR.Input;
using TechtonicaVR.VRCamera;
using TTIK.Network;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace TechtonicaVR.Ik;

public class IkSetup
{
	private static PluginLogger Logger = PluginLogger.GetLogger<IkSetup>();

	public static void SetupIk()
	{
		GameObject headTarget = null;
		Grabable headGrab = null;
		GameObject leftHandTarget = null;
		Grabable leftHandGrab = null;
		GameObject rightHandTarget = null;
		Grabable rightHandGrab = null;
		NetworkIkPlayer.OnLocalPlayerInitialized += (instance) =>
		{
			Logger.LogDebug("Local ik player initialized. Requesting 3pt tracking...");
			headTarget = new GameObject("HeadTarget");
			headTarget.AddComponent<DontDestroyOnLoad>();
			headTarget.transform.localPosition = new Vector3(0.0066f, -0.1317f, -0.1482f);
			headTarget.transform.localRotation = Quaternion.Euler(5.8911f, 85.6196f, 240.5798f);
			headGrab = headTarget.AddComponent<Grabable>();

			leftHandTarget = new GameObject("LeftHandTarget");
			leftHandTarget.AddComponent<DontDestroyOnLoad>();
			leftHandTarget.transform.localPosition = new Vector3(0.0446f, 0.0913f, -0.1265f);
			leftHandTarget.transform.localRotation = Quaternion.Euler(22.2776f, 183.4834f, 127.5222f);
			leftHandGrab = leftHandTarget.AddComponent<Grabable>();

			rightHandTarget = new GameObject("RightHandTarget");
			rightHandTarget.AddComponent<DontDestroyOnLoad>();
			rightHandTarget.transform.localPosition = new Vector3(0.0004f, 0.0901f, -0.1355f);
			rightHandTarget.transform.localRotation = Quaternion.Euler(328.5594f, 6.5692f, 279.517f);
			rightHandGrab = rightHandTarget.AddComponent<Grabable>();

			AsyncGameObject.DelayUntil(() => instance.CmdInitIkPlayer(), () => VRCameraManager.mainGameLoaded);
		};
		NetworkIkPlayer.OnIkInitialized += (instance) =>
		{
			Logger.LogDebug("Ik player initialized. Starting calibration...");
			AsyncGameObject.DelayUntil(() =>
			{
				headTarget.transform.SetParent(VRCameraManager.mainCamera.transform, false);
				leftHandTarget.transform.SetParent(VRCameraManager.leftHandObject.transform, false);
				rightHandTarget.transform.SetParent(VRCameraManager.rightHandObject.transform, false);
				instance.calibrate(headTarget.transform, leftHandTarget.transform, rightHandTarget.transform);
				instance.calibrated(null);
			}, () => VRCameraManager.mainGameLoaded);

			instance.OnCalibrated += () =>
			{
				Logger.LogDebug("Ik player calibrated. Starting tracking...");
				VRCameraManager.leftHandModel.SetActive(!ModConfig.displayBody.Value);
				VRCameraManager.rightHandModel.SetActive(!ModConfig.displayBody.Value);

				instance.avatar.SetActive(ModConfig.displayBody.Value);

				// SteamVRInputMapper.Grab.ButtonPressed += (object _sender, SteamVR_Input_Sources source) =>
				// {
				// 	Logger.LogDebug($"Grabbing target with source {source}");
				// 	// leftHandGrab.TryGrab(VRCameraManager.rightHandObject.transform);
				// 	// headGrab.TryGrab(VRCameraManager.rightHandObject.transform);
				// 	rightHandGrab.TryGrab(VRCameraManager.leftHandObject.transform);
				// };
				// SteamVRInputMapper.Grab.ButtonReleased += (object _sender, SteamVR_Input_Sources _source) =>
				// {
				// 	Logger.LogDebug("Releasing target");
				// 	headGrab.Release();
				// 	leftHandGrab.Release();
				// 	rightHandGrab.Release();
				// };
			};
		};

		// #### Not used until we have FBT ####
		// SteamVRInputMapper.IKCalibrate.ButtonReleased += (object _sender, SteamVR_Input_Sources _source) =>
		// {
		// 	Logger.LogDebug("Calibration button released. Setting calibration values");
		// 	if (NetworkIkPlayer.localInstance.ikPlayer.calibrating == false)
		// 	{
		// 		return;
		// 	}
		// 	NetworkIkPlayer.localInstance.ikPlayer.calibrated(null);
		// };
	}
}
