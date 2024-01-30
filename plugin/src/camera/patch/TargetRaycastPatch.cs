using HarmonyLib;
using TechtonicaVR.Input;
using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.VRCamera.Patch;

[HarmonyPatch]
public class TargetRaycastPatch
{

	private static Canvas cursorCanvas;
	private static Transform _cursorTlc;
	public static Transform cursorTlc
	{
		get => _cursorTlc;
		set
		{
			_cursorTlc = value;
			if (value != null)
			{
				cursorCanvas = value.GetComponentInParent<Canvas>();
			}
		}
	}

	private static Canvas inspectorCanvas;
	private static Transform _inspectorTlc;
	public static Transform inspectorTlc
	{
		get => _inspectorTlc;
		set
		{
			_inspectorTlc = value;
			if (value != null)
			{
				inspectorCanvas = value.GetComponentInParent<Canvas>();
			}
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(PlayerFirstPersonController), nameof(PlayerFirstPersonController.UpdateAimingRaycasts))]
	public static bool UpdateAimingRaycastsPostfix(PlayerFirstPersonController __instance)
	{
		if (SteamVRInputMapper.rightHandObject == null)
		{
			return true;
		}

		var right_hand_transform = SteamVRInputMapper.rightHandObject.transform;
		var forward = -right_hand_transform.up;

		// // Rotate the forward vector 45 degrees down
		// var rotation = Quaternion.Euler(60f, 0f, 0f);
		// forward = rotation * forward;

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
	public static void LateUpdatePostfix(CursorDotUI __instance)
	{
		if (!Player.instance.fpcontroller.hasCamHit || VRCameraManager.mainCamera == null || cursorCanvas == null || inspectorCanvas == null)
		{
			return;
		}

		var anyMenuOpen = UIManager.instance.anyMenuOpen;
		cursorCanvas.enabled = !anyMenuOpen;
		inspectorCanvas.enabled = !anyMenuOpen;
		if (anyMenuOpen)
		{
			return;
		}

		var camHit = Player.instance.fpcontroller.camHit;

		if (cursorTlc != null)
		{
			MathyStuff.PositionCanvasInWorld(cursorTlc.gameObject, VRCameraManager.mainCamera, camHit.point);
		}

		if (inspectorTlc != null)
		{
			MathyStuff.PositionCanvasInWorld(inspectorTlc.gameObject, VRCameraManager.mainCamera, camHit.point);
		}
	}
}
