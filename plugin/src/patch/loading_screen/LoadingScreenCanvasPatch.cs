using PiUtils.Patches;
using TechtonicaVR.VRCamera;
using UnityEngine;

namespace TechtonicaVR.Patches.LoadingScreen;

public class LoadingScreenCanvasPatch : GameObjectPatch
{
	public LoadingScreenCanvasPatch() : base("Canvas")
	{
	}

	protected override bool Apply(GameObject gameObject)
	{
		gameObject.transform.position = new Vector3(0, 0, 0);
		gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

		var canvas = gameObject.GetComponent<Canvas>();
		canvas.renderMode = RenderMode.WorldSpace;

		VRCameraManager.mainCamera.farClipPlane = 20f;

		return true;
	}
}
