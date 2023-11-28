using System.Collections;
using FluffyUnderware.DevTools.Extensions;
using HarmonyLib;
using Plugin.Input;
using Rewired;
using TechtonicaVR.Assets;
using UnityEngine;
using Valve.Newtonsoft.Json.Linq;
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
        // DontDestroyOnLoad(instance.gameObject);

        return instance;
    }

    private void Awake()
    {
        // DontDestroyOnLoad(gameObject);
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
        // DontDestroyOnLoad(gameObject);
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

        // Set the camera's field of view to match the SteamVR camera's field of view
        // mainCamera.fieldOfView = SteamVR.instance.fieldOfView;
        // mainCamera.stereoTargetEye = StereoTargetEyeMask.Left;
        // mainCamera.projectionMatrix = mainCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
        // mainCamera.targetTexture = Plugin.MyDisplay.GetRenderTextureForRenderPass(0);

        // secondCamera.fieldOfView = SteamVR.instance.fieldOfView;
        // secondCamera.stereoTargetEye = StereoTargetEyeMask.Right;
        // secondCamera.projectionMatrix = secondCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
        // secondCamera.targetTexture = Plugin.MyDisplay.GetRenderTextureForRenderPass(1);
    }

    private void SetupCamera()
    {
        Plugin.Logger.LogInfo("Setting up camera...");
        // Matrix4x4 leftEyeProjectionMatrix = Plugin.MyDisplay.GetStereoProjectionMatrix(0);
        // Matrix4x4 rightEyeProjectionMatrix = Plugin.MyDisplay.GetStereoProjectionMatrix(1);

        mainCamera.gameObject.AddComponent<SteamVR_Camera>();
        mainCamera.gameObject.AddComponent<SteamVR_TrackedObject>();
        mainCamera.gameObject.AddComponent<TechMainCamera>();
        HmdMatrix44_t leftEyeMatrix = OpenVR.System.GetProjectionMatrix(EVREye.Eye_Left, mainCamera.nearClipPlane, mainCamera.farClipPlane);
        HmdMatrix44_t rightEyeMatrix = OpenVR.System.GetProjectionMatrix(EVREye.Eye_Right, mainCamera.nearClipPlane, mainCamera.farClipPlane);

        if (PlayerFirstPersonController.instance != null)
        {
            vrRoot = PlayerFirstPersonController.instance.transform;


            mainCamera.transform.parent = vrRoot;
            foreach (var a in ReInput.mapping.Actions)
            {
                Plugin.Logger.LogInfo("Action: " + a.name);
                Plugin.Logger.LogInfo("  id: " + a.id);
            }

            StartCoroutine(PatchCoroutine());
            SpawnHands(vrRoot);
        }

        FindObjectsOfType<Headlamp>().ForEach(h => h.transform.parent = mainCamera.transform);

        // mainCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, HmdMatrix44ToMatrix4x4(leftEyeMatrix));

        // secondCamera = new GameObject("VRCam").AddComponent<Camera>();
        // secondCamera.gameObject.AddComponent<SteamVR_CameraHelper>();
        // secondCamera.CopyFrom(mainCamera);
        // secondCamera.transform.parent = mainCamera.transform.parent;
        // secondCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, HmdMatrix44ToMatrix4x4(rightEyeMatrix));
        // secondCamera.enabled = true;
    }

    IEnumerator PatchCoroutine()
    {
        yield return new WaitForSeconds(1);

        PatchBehaviour.Create();
        yield break;
    }

    private void SpawnHands(Transform vrRoot)
    {
        SteamVRInputMapper.rightHandObject = new GameObject("RightHand");
        SteamVRInputMapper.rightHandObject.transform.parent = vrRoot;
        // var rightHandModel = GameObject.Instantiate(AssetLoader.RightHandBase, Vector3.zero, Quaternion.identity, SteamVRInputMapper.rightHandObject.transform);
        // rightHandModel.GetComponent<Renderer>().enabled = true;

        SteamVRInputMapper.leftHandObject = new GameObject("LeftHand");
        SteamVRInputMapper.leftHandObject.transform.parent = vrRoot;
        // var leftHandModel = GameObject.Instantiate(AssetLoader.LeftHandBase, Vector3.zero, Quaternion.identity, SteamVRInputMapper.leftHandObject.transform);
        // leftHandModel.GetComponent<Renderer>().enabled = true;
    }


    public static Matrix4x4 HmdMatrix44ToMatrix4x4(HmdMatrix44_t hmdMatrix)
    {
        Matrix4x4 matrix = new Matrix4x4();

        matrix[0, 0] = hmdMatrix.m0;
        matrix[0, 1] = hmdMatrix.m1;
        matrix[0, 2] = hmdMatrix.m2;
        matrix[0, 3] = hmdMatrix.m3;
        matrix[1, 0] = hmdMatrix.m4;
        matrix[1, 1] = hmdMatrix.m5;
        matrix[1, 2] = hmdMatrix.m6;
        matrix[1, 3] = hmdMatrix.m7;
        matrix[2, 0] = hmdMatrix.m8;
        matrix[2, 1] = hmdMatrix.m9;
        matrix[2, 2] = hmdMatrix.m10;
        matrix[2, 3] = hmdMatrix.m11;
        matrix[3, 0] = hmdMatrix.m12;
        matrix[3, 1] = hmdMatrix.m13;
        matrix[3, 2] = hmdMatrix.m14;
        matrix[3, 3] = hmdMatrix.m15;

        return matrix;
    }
}
