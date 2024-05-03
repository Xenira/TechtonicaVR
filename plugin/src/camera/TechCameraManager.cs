using System.Collections;
using TechtonicaVR.Input;
using TechtonicaVR.Assets;
using UnityEngine;
using Valve.VR;
using PiUtils.Util;
using PiUtils.Debug;
using TechtonicaVR.VrPlayer;
using PiVrLoader.VRCamera;
using PiVrLoader.Input;

namespace TechtonicaVR.VRCamera;
public class TechCameraManager : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<TechCameraManager>();

	public Transform vrRoot;
	public Transform camRoot;
	public static GameObject rightHandObject;
	public static GameObject rightHandModel;
	public static GameObject leftHandObject;
	public static GameObject leftHandModel;

	public SteamVR_CameraHelper cameraHelperPrefab;

	private SteamVR_CameraHelper cameraHelper;

	public static bool mainGameLoaded = false;
	private static TechCameraManager instance;

	public static TechCameraManager Create()
	{
		if (instance != null)
		{
			Logger.LogWarning("TechCameraManager already exists, destroying it");
			Destroy(instance.gameObject);
		}

		instance = new GameObject(nameof(TechCameraManager)).AddComponent<TechCameraManager>();

		return instance;
	}

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	private void Awake()
	{
		SetupCamera();
	}

	private void SetupCamera()
	{
		VrMainCamera.OnMainCameraCreated += (cam) =>
		{
			var techCam = cam.gameObject.AddComponent<TechMainCamera>();
			if (PlayerFirstPersonController.instance != null)
			{
				vrRoot = PlayerFirstPersonController.instance.transform;
				camRoot = new GameObject("CamRoot").transform;
				techCam.camRoot = camRoot;

				techCam.camRoot.parent = vrRoot;
				techCam.camRoot.localPosition = Vector3.zero;
				cam.transform.parent = techCam.camRoot;

				StartCoroutine(PatchCoroutine());
				SpawnHands(techCam.camRoot);
				AddAudioSrc(VRCameraManager.mainCamera);

				if (ModConfig.VignetteEnabled())
				{
					Vignette.Create();
				}
				Teleport.Create(TechInputMapper.teleport, Player.instance.builder.defaultMask).transform.parent = VRCameraManager.mainCamera.transform;

				new GameObject("MainGamePlayer").AddComponent<MainGamePlayer>();

				mainGameLoaded = true;
			}
		};
	}

	IEnumerator PatchCoroutine()
	{
		yield return new WaitForSeconds(1);

		MainGamePatch.Create();
		yield break;
	}

	private void SpawnHands(Transform vrRoot)
	{
		rightHandObject = new GameObject("RightHand");
		rightHandObject.AddComponent<Gizmo>();

		rightHandObject.transform.parent = vrRoot;
		rightHandModel = GameObject.Instantiate(AssetLoader.RightHandBase, Vector3.zero, Quaternion.identity, rightHandObject.transform);
		rightHandModel.transform.localPosition = new Vector3(0, 0.01f, -0.09f);
		rightHandModel.transform.localRotation = Quaternion.Euler(358.4256f, 103.2413f, 240.2217f);
		var rightLaserPointer = rightHandObject.AddComponent<LaserPointer>();
		rightLaserPointer.inputSource = SteamVR_Input_Sources.RightHand;
		rightLaserPointer.interactButton = TechInputMapper.UIClick;
		rightLaserPointer.direction = Vector3.down;

		SteamVRInputMapper.rightHandObject = rightHandObject;

		leftHandObject = new GameObject("LeftHand");
		leftHandObject.AddComponent<Gizmo>();

		leftHandObject.transform.parent = vrRoot;
		leftHandModel = GameObject.Instantiate(AssetLoader.LeftHandBase, Vector3.zero, Quaternion.identity, leftHandObject.transform);
		leftHandModel.transform.localPosition = new Vector3(0, 0.01f, -0.09f);
		leftHandModel.transform.localRotation = Quaternion.Euler(335.2912f, 256.7355f, 116.7813f);

		SteamVRInputMapper.leftHandObject = leftHandObject;
	}

	private void AddAudioSrc(Camera mainCamera)
	{
		var audioSource = mainCamera.gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
	}
}
