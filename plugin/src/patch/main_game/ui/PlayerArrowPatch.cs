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
		gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
		return true;
	}
}
