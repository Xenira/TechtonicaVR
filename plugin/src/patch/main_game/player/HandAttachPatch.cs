using PiUtils.Patches;
using TechtonicaVR.Input;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.Player;

public class RightHandAttachPatch : GameObjectPatch
{
	public RightHandAttachPatch() : base("Right Hand Attach")
	{
	}

	protected override bool Apply(GameObject gameObject)
	{
		if (SteamVRInputMapper.rightHandObject == null)
		{
			return false;
		}
		gameObject.transform.parent = SteamVRInputMapper.rightHandObject.transform;
		gameObject.transform.localPosition = new Vector3(0.0016f, 0.0749f, -0.0961f);
		gameObject.transform.localRotation = Quaternion.Euler(345.2184f, 358.572f, 280.9678f);
		return true;
	}
}

public class LeftHandAttachPatch : GameObjectPatch
{
	public LeftHandAttachPatch() : base("Left Hand Attach")
	{
	}

	protected override bool Apply(GameObject gameObject)
	{
		if (SteamVRInputMapper.leftHandObject == null)
		{
			return false;
		}

		gameObject.transform.parent = SteamVRInputMapper.leftHandObject.transform;
		gameObject.transform.localPosition = new Vector3(0.0571f, 0.0371f, -0.0295f);
		gameObject.transform.localRotation = Quaternion.Euler(28.9023f, 352.767f, 77.4064f);
		return true;
	}
}
