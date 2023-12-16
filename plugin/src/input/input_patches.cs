using System.Linq;
using HarmonyLib;
using Plugin.Input;
using TechtonicaVR.VRCamera;
using UnityEngine;
using Valve.VR;

namespace TechtonicaVR.Input;

[HarmonyPatch]
class InputPatches
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(ControlDefines), nameof(ControlDefines.UpdateCurrentController))]
	static bool GetUseController()
	{
		FlowManager.instance.useController = true;
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
			return false;
		}
		else if (xAxisActionId == RewiredConsts.Action.UI_Horizontal_Primary && yAxisActionId == RewiredConsts.Action.UI_Vertical_Primary)
		{
			__result = SteamVRInputMapper.UIAxesPrimary;
			return false;
		}
		else if (xAxisActionId == RewiredConsts.Action.UI_Horizontal_Secondary && yAxisActionId == RewiredConsts.Action.UI_Vertical_Secondary)
		{
			__result = SteamVRInputMapper.UIAxesSecondary;
			return false;
		}
		else
		{
			Plugin.Logger.LogDebug($"Unknown Rewired axis action IDs: {xAxisActionId}, {yAxisActionId}. Using default Rewired input.");
			return true;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(PlayerFirstPersonController), nameof(PlayerFirstPersonController.Move))]
	static void Move(PlayerFirstPersonController __instance)
	{
		if (!__instance.m_IsGrounded || InputHandler.instance.playerInputBlocked || InputHandler.instance.playerInputBlockedOverride && VRCameraManager.mainCamera != null)
		{
			return;
		}

		var delta = Time.deltaTime;
		var horizontalRotation = SteamVRInputMapper.TurnAxis * delta * ModConfig.smoothTurnSpeed.Value;
		__instance.gameObject.transform.RotateAround(VRCameraManager.mainCamera.transform.position, Vector3.up, horizontalRotation);
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
			Plugin.Logger.LogDebug($"Unknown Rewired button down action ID: {actionId}. Using default Rewired input.");
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
			Plugin.Logger.LogDebug($"Unknown Rewired button up action ID: {actionId}. Using default Rewired input.");
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
			Plugin.Logger.LogDebug($"Unknown Rewired button action ID: {actionId}. Using default Rewired input.");
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(InputHandler), nameof(InputHandler.AnyKeyPressed), MethodType.Getter)]
	static bool AnyKeyPressed(ref bool __result)
	{
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
				return SteamVRInputMapper.Jump;
			case RewiredConsts.Action.Use:
				return SteamVRInputMapper.Use;
			case RewiredConsts.Action.Interact:
			case RewiredConsts.Action.MouseLeftClick:
				return SteamVRInputMapper.Interact;
			case RewiredConsts.Action.Sprint:
				return SteamVRInputMapper.Sprint;
			case RewiredConsts.Action.Rotate_Counterclockwise:
				return SteamVRInputMapper.rotateLeft;
			case RewiredConsts.Action.Rotate_Clockwise:
				return SteamVRInputMapper.rotateRight;
			case RewiredConsts.Action.Crafting_Menu:
				return SteamVRInputMapper.Inventory;
			case RewiredConsts.Action.Toggle_Erase:
				return SteamVRInputMapper.toggleErase;
			case RewiredConsts.Action.Cycle_Backward:
				return SteamVRInputMapper.cycleHotbarLeft;
			case RewiredConsts.Action.UIPageLeft:
				return SteamVRInputMapper.UIPageLeft;
			case RewiredConsts.Action.UIPageRight:
				return SteamVRInputMapper.UIPageRight;
			case RewiredConsts.Action.UIPageLeftSecondary:
				return SteamVRInputMapper.UIPageLeftSecondary;
			case RewiredConsts.Action.UIPageRightSecondary:
				return SteamVRInputMapper.UIPageRightSecondary;
			case RewiredConsts.Action.UI_Submit:
				return SteamVRInputMapper.UISubmit;
			case RewiredConsts.Action.UI_Cancel:
				return SteamVRInputMapper.UICancel;
			case RewiredConsts.Action.Hotbar_Edit_Single:
				return SteamVRInputMapper.HotbarEdit;
			case RewiredConsts.Action.Lock_Toolbar:
			case RewiredConsts.Action.Edit_Shotcut:
				return SteamVRInputMapper.HotbarSwapItem;
			case RewiredConsts.Action.Exit_Hotbar:
				return SteamVRInputMapper.HotbarExitEdit;
			case RewiredConsts.Action.Clear_Shortcut:
				return SteamVRInputMapper.HotbarClear;
			case RewiredConsts.Action.UI_Shortcut_1:
				return SteamVRInputMapper.UIShortcut1;
			case RewiredConsts.Action.UI_Shortcut_2:
				return SteamVRInputMapper.UIShortcut2;
			case RewiredConsts.Action.Craft:
				return SteamVRInputMapper.craft;
			case RewiredConsts.Action.Craft_Five:
				return SteamVRInputMapper.craftFive;
			case RewiredConsts.Action.Craft_All:
				return SteamVRInputMapper.craftAll;
			case RewiredConsts.Action.Transfer:
				return SteamVRInputMapper.transfer;
			case RewiredConsts.Action.Transfer_Half:
				return SteamVRInputMapper.transferHalf;
			case RewiredConsts.Action.Transfer_All:
				return SteamVRInputMapper.transferAll;
			case RewiredConsts.Action.Take_All_Shortcut:
				return SteamVRInputMapper.takeAll;
			case RewiredConsts.Action.Cycle_Forward:
				return SteamVRInputMapper.cycleHotbarRight;
			case RewiredConsts.Action.KB_TechTree:
				return SteamVRInputMapper.TechTree;
			case RewiredConsts.Action.Pause:
				return SteamVRInputMapper.PauseMenu;
			default:
				// Plugin.Logger.LogDebug($"Unknown Rewired button action ID: {actionId}. Using default Rewired input.");
				return null;
		}
	}

	private static Button[] AllButtons()
	{
		return [
			SteamVRInputMapper.Jump,
			SteamVRInputMapper.Interact,
			SteamVRInputMapper.Sprint,
			SteamVRInputMapper.Inventory,
			SteamVRInputMapper.TechTree,
			SteamVRInputMapper.cycleHotbarLeft,
			SteamVRInputMapper.takeAll,
			SteamVRInputMapper.cycleHotbarRight,
			SteamVRInputMapper.rotateLeft,
			SteamVRInputMapper.rotateRight,
			SteamVRInputMapper.UIPageLeft,
			SteamVRInputMapper.UIPageRight,
			SteamVRInputMapper.UIPageLeftSecondary,
			SteamVRInputMapper.UIPageRightSecondary,
			SteamVRInputMapper.UISubmit,
			SteamVRInputMapper.UICancel,
			SteamVRInputMapper.craft,
			SteamVRInputMapper.craftFive,
			SteamVRInputMapper.craftAll,
			SteamVRInputMapper.transfer,
			SteamVRInputMapper.transferHalf,
			SteamVRInputMapper.transferAll
		];
	}
}
