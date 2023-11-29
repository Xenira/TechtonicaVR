using Plugin.Input;
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
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
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
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
        return true;
    }
}
