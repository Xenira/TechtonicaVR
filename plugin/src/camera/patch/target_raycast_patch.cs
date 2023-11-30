using HarmonyLib;
using Plugin.Input;
using UnityEngine;

namespace TechtonicaVR.VRCamera.Patch;

[HarmonyPatch]
public class TargetRaycastPatch
{

    public static Transform cursorTlc;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerFirstPersonController), nameof(PlayerFirstPersonController.UpdateAimingRaycasts))]
    public static bool Postfix(PlayerFirstPersonController __instance)
    {
        var right_hand_transform = SteamVRInputMapper.rightHandObject.transform;
        var forward = right_hand_transform.forward;

        // Rotate the forward vector 45 degrees down
        var rotation = Quaternion.Euler(45f, 0f, 0f);
        forward = rotation * forward;

        __instance._hasCamHit = Physics.Raycast(new Ray(right_hand_transform.position, forward), out __instance._camHit, __instance.cam.farClipPlane, __instance.aimLayer, QueryTriggerInteraction.Collide);
        if (__instance.hasCamHit)
        {
            __instance.lookAtTarget.position = __instance.camHit.point;
            __instance._hasPlayerAimHit = __instance._hasCamHit;
        }
        else
        {
            __instance.lookAtTarget.position = __instance.cam.transform.position + __instance.cam.transform.forward * __instance.cam.farClipPlane;
            __instance._camHit = default;
            __instance._playerAimHit = default;
        }
        UIManager.instance.hud.cursorDotUI.cursorSetting = CursorDotUI.CursorSetting.Default;

        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CursorDotUI), nameof(PlayerFirstPersonController.LateUpdate))]
    public static void Postfix(CursorDotUI __instance)
    {
        if (cursorTlc != null && Player.instance.fpcontroller.hasCamHit)
        {
            var camHit = Player.instance.fpcontroller.camHit;
            Vector3 screenPoint = Player.instance.fpcontroller.cam.WorldToScreenPoint(camHit.point);
            Vector3 canvasPoint = cursorTlc.parent.GetComponent<Canvas>().worldCamera.ScreenToWorldPoint(screenPoint);
            cursorTlc.position = canvasPoint;
        }
    }
}