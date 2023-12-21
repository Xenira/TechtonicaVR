using TechtonicaVR.VRCamera;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;
public class PlayerArrowPatch : GameObjectPatch
{
	public PlayerArrowPatch() : base("Player Arrow")
	{
	}

	protected override bool Apply(GameObject gameObject)
	{
		gameObject.transform.parent = VRCameraManager.mainCamera.transform;
		return true;
	}
}
