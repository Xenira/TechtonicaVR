using System.Collections;
using FluffyUnderware.DevTools.Extensions;
using Plugin.Input;
using Rewired;
using TechtonicaVR.Assets;
using TechtonicaVR.Debug;
using UnityEngine;
using Valve.VR;

namespace TechtonicaVR.VRCamera;
public class VRCameraManager : MonoBehaviour
{
	public Transform vrRoot;

	public SteamVR_CameraHelper cameraHelperPrefab;

	private SteamVR_CameraHelper cameraHelper;

	public static Camera mainCamera;

	public static VRCameraManager Create()
	{
		var instance = new GameObject(nameof(VRCameraManager)).AddComponent<VRCameraManager>();

		return instance;
	}

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	private void OnDestory()
	{
		Plugin.Logger.LogInfo("Destroying vr camera manager...");
	}

	private void OnEnable()
	{
		Plugin.Logger.LogInfo("Enabling vr camera manager...");
	}

	private void OnDisable()
	{
		Plugin.Logger.LogInfo("Disabling vr camera manager...");
	}

	private void Update()
	{
		if (Camera.main == null)
		{
			return;
		}

		if (Camera.main != mainCamera)
		{
			mainCamera = Camera.main;
			SetupCamera();
		}
	}

	private void SetupCamera()
	{
		Plugin.Logger.LogInfo("Setting up camera...");

		mainCamera.gameObject.AddComponent<SteamVR_Camera>();
		mainCamera.gameObject.AddComponent<SteamVR_TrackedObject>();
		var techCam = mainCamera.gameObject.AddComponent<TechMainCamera>();
		HmdMatrix44_t leftEyeMatrix = OpenVR.System.GetProjectionMatrix(EVREye.Eye_Left, mainCamera.nearClipPlane, mainCamera.farClipPlane);
		HmdMatrix44_t rightEyeMatrix = OpenVR.System.GetProjectionMatrix(EVREye.Eye_Right, mainCamera.nearClipPlane, mainCamera.farClipPlane);

		if (PlayerFirstPersonController.instance != null)
		{
			vrRoot = PlayerFirstPersonController.instance.transform;
			techCam.camRoot = new GameObject("CamRoot").transform;

			techCam.camRoot.parent = vrRoot;
			techCam.camRoot.localPosition = Vector3.zero;
			mainCamera.transform.parent = techCam.camRoot;
			foreach (var a in ReInput.mapping.Actions)
			{
				Plugin.Logger.LogInfo("Action: " + a.name);
				Plugin.Logger.LogInfo("  id: " + a.id);
			}

			StartCoroutine(PatchCoroutine());
			SpawnHands(techCam.camRoot);
		}

		FindObjectsOfType<Headlamp>().ForEach(h => h.transform.parent = mainCamera.transform);
	}

	IEnumerator PatchCoroutine()
	{
		yield return new WaitForSeconds(1);

		MainGamePatch.Create();
		yield break;
	}

	private void SpawnHands(Transform vrRoot)
	{
		var rightHandObject = new GameObject("RightHand");
		rightHandObject.AddComponent<Gizmo>();

		rightHandObject.transform.parent = vrRoot;
		var rightHandModel = GameObject.Instantiate(AssetLoader.RightHandBase, Vector3.zero, Quaternion.identity, rightHandObject.transform);
		rightHandModel.transform.localPosition = new Vector3(0, 0.01f, -0.09f);
		rightHandModel.transform.localRotation = Quaternion.Euler(358.4256f, 103.2413f, 240.2217f);

		SteamVRInputMapper.rightHandObject = rightHandObject;

		var leftHandObject = new GameObject("LeftHand");
		leftHandObject.AddComponent<Gizmo>();

		leftHandObject.transform.parent = vrRoot;
		var leftHandModel = GameObject.Instantiate(AssetLoader.LeftHandBase, Vector3.zero, Quaternion.identity, leftHandObject.transform);
		leftHandModel.transform.localPosition = new Vector3(0, 0.01f, -0.09f);
		leftHandModel.transform.localRotation = Quaternion.Euler(335.2912f, 256.7355f, 116.7813f);

		SteamVRInputMapper.leftHandObject = leftHandObject;
	}
}
