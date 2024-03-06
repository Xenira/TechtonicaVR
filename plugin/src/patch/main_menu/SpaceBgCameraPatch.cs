using PiUtils.Patches;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Valve.VR;

namespace TechtonicaVR.Patches.MainMenu;

public class SpaceBGCameraPatch : GameObjectPatch
{
	public SpaceBGCameraPatch() : base("Space BG Camera")
	{
	}

	protected override bool Apply(GameObject gameObject)
	{
		var origin = GameObject.Find("Main Camera (origin)");
		if (origin == null)
		{
			return false;
		}

		gameObject.AddComponent<SteamVR_TrackedObject>();
		gameObject.GetComponent<PostProcessLayer>().enabled = false;
		Object.Destroy(gameObject.GetComponent("SpaceGraphicsToolkit.SgtCamera"));

		gameObject.transform.parent = origin.transform;
		return true;
	}
}
