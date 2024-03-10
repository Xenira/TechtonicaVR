using PiUtils.Util;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace TechtonicaVR.VRCamera;

public class TechMainCamera : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<TechMainCamera>();
	public static TechMainCamera instance;

	public Transform camRoot;
	private PostProcessLayer postProcessLayer;
	private Vector3 offset = Vector3.zero;
	private void Start()
	{
		// Get the PostProcessLayer component attached to the camera
		postProcessLayer = GetComponent<PostProcessLayer>();

		// Disable the PostProcessLayer
		if (postProcessLayer != null)
		{
			postProcessLayer.enabled = false;
		}

		instance = this;
	}

	private void Update()
	{
		if (PlayerFirstPersonController.instance == null || camRoot == null)
		{
			return;
		}

		var camPosition = transform.localPosition;

		var diff = camPosition + offset;
		diff.y = 0;
		diff = PlayerFirstPersonController.instance.transform.rotation * diff;

		offset = -camPosition;

		if (float.IsNaN(diff.x) || float.IsNaN(diff.z))
		{
			Logger.LogError("Diff is NaN");
			return;
		}

		var newCamRootPosition = offset;
		newCamRootPosition.y = 0;

		PlayerFirstPersonController.instance.transform.position += diff;
		camRoot.localPosition = newCamRootPosition;
	}
}
