using Plugin.Input;
using TechtonicaVR.Debug;
using TechtonicaVR.VRCamera.Patch;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

public class CursorCanvasPatch : GameObjectPatch
{
    public CursorCanvasPatch() : base("Cursor Canvas")
    {
    }

    protected override bool Apply(GameObject gameObject)
    {
        var tlc = gameObject.transform.GetChild(0);
        TargetRaycastPatch.cursorTlc = tlc;

        // Debug
        var line = tlc.gameObject.AddComponent<DebugLine>();
        line.start = tlc;
        line.end = SteamVRInputMapper.rightHandObject.transform;

        return true;
    }
}