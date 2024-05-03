using System.Linq;
using HarmonyLib;
using PiUtils.Util;
using PiVrLoader.Input;
using PiVrLoader.VRCamera;
using UnityEngine;
using Valve.VR;

namespace TechtonicaVR.Input;

[HarmonyPatch]
class InputPatches
{
	private static PluginLogger Logger = PluginLogger.GetLogger<InputPatches>();

	[HarmonyPrefix]
	[HarmonyPatch(typeof(ControlDefines), nameof(ControlDefines.UpdateCurrentController))]
	static bool GetUseController()
	{
		FlowManager.instance.useController = true;
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(AltInputHandler), nameof(AltInputHandler.isUsingMouse), MethodType.Getter)]
	static bool GetIsUsingMouse(ref bool __result)
	{
		__result = false;
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(InputHandler), nameof(InputHandler.SetVibration))]
	static bool SetVibration(float leftIntensity, float rightIntensity)
	{
		SteamVR_Actions._default.Haptic.Execute(0, Time.deltaTime, 1f / 60f, leftIntensity, SteamVR_Input_Sources.LeftHand);
		SteamVR_Actions._default.Haptic.Execute(0, Time.deltaTime, 1f / 60f, rightIntensity, SteamVR_Input_Sources.RightHand);
		return false;
	}


	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetAxis2D), [typeof(int), typeof(int)])]
	static bool GetAxis2D(ref Vector2 __result, int xAxisActionId, int yAxisActionId)
	{
		return handleAxisInput(ref __result, xAxisActionId, yAxisActionId);
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetAxis2DRaw), [typeof(int), typeof(int)])]
	static bool GetAxis2DRaw(ref Vector2 __result, int xAxisActionId, int yAxisActionId)
	{
		return handleAxisInput(ref __result, xAxisActionId, yAxisActionId);
	}

	private static bool handleAxisInput(ref Vector2 __result, int xAxisActionId, int yAxisActionId)
	{
		if (xAxisActionId == RewiredConsts.Action.Move_Horizontal && yAxisActionId == RewiredConsts.Action.Move_Vertical)
		{
			__result = SteamVRInputMapper.MoveAxes;
			if (ModConfig.VignetteOnSmoothLocomotion())
			{
				Vignette.instance?.Show(SteamVRInputMapper.MoveAxes.magnitude);
			}
			return false;
		}
		else if (xAxisActionId == RewiredConsts.Action.UI_Horizontal_Primary && yAxisActionId == RewiredConsts.Action.UI_Vertical_Primary)
		{
			__result = TechInputMapper.UIAxesPrimary;
			return false;
		}
		else if (xAxisActionId == RewiredConsts.Action.UI_Horizontal_Secondary && yAxisActionId == RewiredConsts.Action.UI_Vertical_Secondary)
		{
			__result = TechInputMapper.UIAxesSecondary;
			return false;
		}
		else if (xAxisActionId == RewiredConsts.Action.Rotate_Horizontal && yAxisActionId == RewiredConsts.Action.Rotate_Vertical)
		{
			__result = TechInputMapper.UIAxesSecondary;
			return false;
		}
		else
		{
			Logger.LogDebug($"Unknown Rewired axis action IDs: {xAxisActionId}, {yAxisActionId}. Using default Rewired input.");
			return true;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(PlayerFirstPersonController), nameof(PlayerFirstPersonController.Move))]
	static void Move(PlayerFirstPersonController __instance)
	{
		if ((!__instance.m_IsGrounded && !__instance.hoverPackActive) || InputHandler.instance.playerInputBlocked || InputHandler.instance.playerInputBlockedOverride && VRCameraManager.mainCamera != null)
		{
			return;
		}

		Turn(__instance);
	}

	private static void Turn(PlayerFirstPersonController __instance)
	{
		if (VRCameraManager.mainCamera == null)
		{
			return;
		}

		if (TechInputMapper.snapTurnLeft.IsReleased())
		{
			ExecuteSnapTurn(__instance, -1);
			return;
		}

		if (TechInputMapper.snapTurnRight.IsReleased())
		{
			ExecuteSnapTurn(__instance, 1);
			return;
		}

		var horizontalRotation = SteamVRInputMapper.TurnAxis * Time.deltaTime * ModConfig.smoothTurnSpeed.Value;
		if (ModConfig.VignetteOnSmoothTurn())
		{
			Vignette.instance?.Show(Mathf.Abs(SteamVRInputMapper.TurnAxis));
		}
		__instance.gameObject.transform.RotateAround(VRCameraManager.mainCamera.transform.position, Vector3.up, horizontalRotation);
	}

	private static void ExecuteSnapTurn(PlayerFirstPersonController __instance, float direction)
	{
		if (ModConfig.VignetteOnSnapTurn())
		{
			Vignette.instance.OneShot(() => SnapTurn.Turn(__instance.gameObject, direction * ModConfig.snapTurnAngle.Value));
		}
		else
		{
			SnapTurn.Turn(__instance.gameObject, direction * ModConfig.snapTurnAngle.Value);
		}

		return;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonDown), [typeof(int)])]
	static bool GetButtonDown(ref bool __result, int actionId)
	{
		var state = MapButtonState(actionId);
		if (state != null)
		{
			__result = state.IsPressed();
			return false;
		}

		return true;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonDown), [typeof(int)])]
	static void GetButtonDown(int actionId, ref bool __result, bool __runOriginal)
	{
		if (__runOriginal && __result)
		{
			Logger.LogDebug($"Unknown Rewired button down action ID: {actionId}. Using default Rewired input.");
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonUp), [typeof(int)])]
	static bool GetButtonUp(ref bool __result, int actionId)
	{
		var state = MapButtonState(actionId);
		if (state != null)
		{
			__result = state.IsReleased();
			return false;
		}

		return true;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonUp), [typeof(int)])]
	static void GetButtonUp(int actionId, ref bool __result, bool __runOriginal)
	{
		if (__runOriginal && __result)
		{
			Logger.LogDebug($"Unknown Rewired button up action ID: {actionId}. Using default Rewired input.");
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonTimedPress), [typeof(int), typeof(float)])]
	static bool GetButtonTimedPress(ref bool __result, int actionId, float time)
	{
		var state = MapButtonState(actionId);
		if (state != null)
		{
			__result = state.IsTimedPress(time);
			return false;
		}

		return true;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonLongPressDown), [typeof(int)])]
	static bool GetButtonLongPressDown(ref bool __result, int actionId)
	{
		var state = MapButtonState(actionId);
		if (state != null)
		{
			__result = state.IsTimedPressDown(ModConfig.longPressTime.Value);
			return false;
		}

		return true;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonTimedPressDown), [typeof(int), typeof(float)])]
	static bool GetButtonTimedPressDown(ref bool __result, int actionId, float time)
	{
		var state = MapButtonState(actionId);
		if (state != null)
		{
			__result = state.IsTimedPressDown(time);
			return false;
		}

		return true;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonTimedPressUp), [typeof(int), typeof(float)])]
	static bool GetButtonTimedPressUp(ref bool __result, int actionId, float time)
	{
		var state = MapButtonState(actionId);
		if (state != null)
		{
			__result = state.IsTimedPressUp(time);
			return false;
		}

		return true;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonTimedPressUp), [typeof(int), typeof(float), typeof(float)])]
	static bool GetButtonTimedPressUp(ref bool __result, int actionId, float time, float expireIn)
	{
		var state = MapButtonState(actionId);
		if (state != null)
		{
			__result = state.IsTimedPressUp(time, expireIn);
			return false;
		}

		return true;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButton), [typeof(int)])]
	static bool GetButton(ref bool __result, int actionId)
	{
		var state = MapButtonState(actionId);
		if (state != null)
		{
			__result = state.IsDown();
			return false;
		}

		return true;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButton), [typeof(int)])]
	static void GetButton(int actionId, ref bool __result, bool __runOriginal)
	{
		if (__runOriginal && __result)
		{
			Logger.LogDebug($"Unknown Rewired button action ID: {actionId}. Using default Rewired input.");
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(InputHandler), nameof(InputHandler.AnyInputPressed), MethodType.Getter)]
	static bool AnyKeyPressed(ref bool __result)
	{
		if (InputHandler.instance.uiInputBlocked)
		{
			__result = false;
			return false;
		}

		GetAnyButtonDown(ref __result);
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetAnyButton))]
	static bool GetAnyButton(ref bool __result)
	{
		__result = AllButtons().Any(state => state.IsDown());
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetAnyButtonDown))]
	static bool GetAnyButtonDown(ref bool __result)
	{
		__result = AllButtons().Any(state => state.IsPressed());
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetAnyButtonUp))]
	static bool GetAnyButtonUp(ref bool __result)
	{
		__result = AllButtons().Any(state => state.IsReleased());
		return false;
	}

	private static Button MapButtonState(int actionId)
	{
		switch (actionId)
		{
			case RewiredConsts.Action.Jump:
				return TechInputMapper.Jump;
			case RewiredConsts.Action.Use:
				return TechInputMapper.Use;
			case RewiredConsts.Action.Interact:
			case RewiredConsts.Action.MouseLeftClick:
				return TechInputMapper.Interact;
			case RewiredConsts.Action.Sprint:
				return TechInputMapper.Sprint;
			case RewiredConsts.Action.Rotate_Counterclockwise:
				return TechInputMapper.rotateLeft;
			case RewiredConsts.Action.Rotate_Clockwise:
				return TechInputMapper.rotateRight;
			case RewiredConsts.Action.Crafting_Menu:
				return TechInputMapper.Inventory;
			case RewiredConsts.Action.Toggle_Erase:
				return TechInputMapper.toggleErase;
			case RewiredConsts.Action.Cycle_Backward:
				return TechInputMapper.cycleHotbarLeft;
			case RewiredConsts.Action.UIPageLeft:
				return TechInputMapper.UIPageLeft;
			case RewiredConsts.Action.UIPageRight:
				return TechInputMapper.UIPageRight;
			case RewiredConsts.Action.UIPageLeftSecondary:
				return TechInputMapper.UIPageLeftSecondary;
			case RewiredConsts.Action.UIPageRightSecondary:
				return TechInputMapper.UIPageRightSecondary;
			case RewiredConsts.Action.UI_Submit:
				return TechInputMapper.UISubmit;
			case RewiredConsts.Action.UI_Cancel:
				return TechInputMapper.UICancel;
			case RewiredConsts.Action.Hotbar_Edit_Single:
				return TechInputMapper.HotbarEdit;
			case RewiredConsts.Action.Lock_Toolbar:
			case RewiredConsts.Action.Edit_Shotcut:
				return TechInputMapper.HotbarSwapItem;
			case RewiredConsts.Action.Exit_Hotbar:
				return TechInputMapper.HotbarExitEdit;
			case RewiredConsts.Action.Clear_Shortcut:
				return TechInputMapper.HotbarClear;
			case RewiredConsts.Action.UI_Shortcut_1:
				return TechInputMapper.UIShortcut1;
			case RewiredConsts.Action.UI_Shortcut_2:
				return TechInputMapper.UIShortcut2;
			case RewiredConsts.Action.Zoom_In:
				return TechInputMapper.SonarZoomIn;
			case RewiredConsts.Action.Zoom_Out:
				return TechInputMapper.SonarZoomOut;
			case RewiredConsts.Action.Craft:
				return TechInputMapper.craft;
			case RewiredConsts.Action.Craft_Five:
				return TechInputMapper.craftFive;
			case RewiredConsts.Action.Craft_All:
				return TechInputMapper.craftAll;
			case RewiredConsts.Action.Transfer:
				return TechInputMapper.transfer;
			case RewiredConsts.Action.Transfer_Half:
				return TechInputMapper.transferHalf;
			case RewiredConsts.Action.Transfer_All:
				return TechInputMapper.transferAll;
			case RewiredConsts.Action.Take_All_Shortcut:
				return TechInputMapper.takeAll;
			case RewiredConsts.Action.Cycle_Forward:
				return TechInputMapper.cycleHotbarRight;
			case RewiredConsts.Action.KB_TechTree:
				return TechInputMapper.TechTree;
			case RewiredConsts.Action.Pause:
				return TechInputMapper.PauseMenu;
			case RewiredConsts.Action.Swap_Variation:
				return TechInputMapper.Variant;
			default:
				// Plugin.Logger.LogDebug($"Unknown Rewired button action ID: {actionId}. Using default Rewired input.");
				return null;
		}
	}

	private static Button[] AllButtons()
	{
		return [
			TechInputMapper.Jump,
			TechInputMapper.Interact,
			TechInputMapper.Sprint,
			TechInputMapper.Inventory,
			TechInputMapper.TechTree,
			TechInputMapper.cycleHotbarLeft,
			TechInputMapper.takeAll,
			TechInputMapper.cycleHotbarRight,
			TechInputMapper.rotateLeft,
			TechInputMapper.rotateRight,
			TechInputMapper.UIPageLeft,
			TechInputMapper.UIPageRight,
			TechInputMapper.UIPageLeftSecondary,
			TechInputMapper.UIPageRightSecondary,
			TechInputMapper.UISubmit,
			TechInputMapper.UICancel,
			TechInputMapper.craft,
			TechInputMapper.craftFive,
			TechInputMapper.craftAll,
			TechInputMapper.transfer,
			TechInputMapper.transferHalf,
			TechInputMapper.transferAll,
			TechInputMapper.HotbarEdit,
			TechInputMapper.HotbarSwapItem,
			TechInputMapper.HotbarExitEdit,
			TechInputMapper.HotbarClear,
			TechInputMapper.UIShortcut1,
			TechInputMapper.UIShortcut2,
			TechInputMapper.SonarZoomIn,
			TechInputMapper.SonarZoomOut,
			TechInputMapper.PauseMenu,
			TechInputMapper.Variant,
		];
	}
}
