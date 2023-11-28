using Plugin.Input;
using TechtonicaVR.VRCamera;
using UnityEngine;

namespace TechtonicaVR.Patches.Player;

public class AimTransformPatch : GameObjectPatch
{
    public AimTransformPatch() : base("Aim Transform")
    {
    }

    protected override bool Apply(GameObject gameObject)
    {
        if (VRCameraManager.mainCamera == null)
        {
            return false;
        }

        gameObject.transform.parent = VRCameraManager.mainCamera.transform;
        return true;
    }
}