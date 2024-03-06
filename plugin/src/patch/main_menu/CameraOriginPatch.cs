using PiUtils.Patches;
using UnityEngine;

namespace TechtonicaVR.Patches.MainMenu;

public class CameraOriginPatch : GameObjectPatch
{
	public CameraOriginPatch() : base("Main Camera (origin)")
	{
	}

	protected override bool Apply(GameObject gameObject)
	{
		gameObject.transform.position = new Vector3(-1614, 23, -2312);
		return true;
	}
}
