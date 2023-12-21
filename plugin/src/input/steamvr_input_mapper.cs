using UnityEngine;
using Valve.VR;

namespace TechtonicaVR.Input;

public static class SteamVRInputMapper
{
	public static Vector2 MoveAxes { get; private set; }
	public static Vector2 UIAxesPrimary { get; private set; }
	public static Vector2 UIAxesSecondary { get; private set; }

	public static float TurnAxis { get; private set; }

	public static GameObject leftHandObject;
	public static GameObject rightHandObject;

	/// <summary>
	/// (current state, previous state)
	/// </summary>
	public static Button Sprint = new Button(SteamVR_Actions._default.Sprint);
	public static Button Jump = new Button(SteamVR_Actions._default.Jump);
	public static Button Interact = new Button(SteamVR_Actions._default.Interact);
	public static Button Use = new Button(SteamVR_Actions._default.Use);
	public static Button TechTree = new Button(SteamVR_Actions._default.TechTree);
	public static Button Inventory = new Button(SteamVR_Actions._default.Inventory);
	public static Button takeAll = new Button(SteamVR_Actions._default.TakeAll);
	public static Button craft = new Button(SteamVR_Actions._default.Craft);
	public static Button craftFive = new Button(SteamVR_Actions._default.CraftFive);
	public static Button craftAll = new Button(SteamVR_Actions._default.CraftAll);
	public static Button transfer = new Button(SteamVR_Actions._default.Transfer);
	public static Button transferHalf = new Button(SteamVR_Actions._default.TransferHalf);
	public static Button transferAll = new Button(SteamVR_Actions._default.TransferAll);
	public static Button UIPageLeft = new Button(SteamVR_Actions._default.UIPageLeft);
	public static Button UIPageRight = new Button(SteamVR_Actions._default.UIPageRight);
	public static Button UIPageLeftSecondary = new Button(SteamVR_Actions._default.UIPageLeftSecondary);
	public static Button UIPageRightSecondary = new Button(SteamVR_Actions._default.UIPageRightSecondary);
	public static Button UISubmit = new Button(SteamVR_Actions._default.UISubmit);
	public static Button UICancel = new Button(SteamVR_Actions._default.UICancel);
	public static Button HotbarEdit = new Button(SteamVR_Actions._default.ToggleHotbarEdit);
	public static Button HotbarSwapItem = new Button(SteamVR_Actions._default.HotbarSwap);
	public static Button HotbarExitEdit = new Button(SteamVR_Actions._default.ExitHotbarEdit);
	public static Button HotbarClear = new Button(SteamVR_Actions._default.ClearHotbar);
	public static Button UIShortcut1 = new Button(SteamVR_Actions._default.UIShortcut1);
	public static Button UIShortcut2 = new Button(SteamVR_Actions._default.UIShortcut2);
	public static Button SonarZoomIn = new Button(SteamVR_Actions._default.SonarZoomIn);
	public static Button SonarZoomOut = new Button(SteamVR_Actions._default.SonarZoomOut);
	public static Button PauseMenu = new Button(SteamVR_Actions._default.PauseMenu);

	public static Button cycleHotbarLeft = new Button(SteamVR_Actions._default.CycleHotbarLeft);
	public static Button cycleHotbarRight = new Button(SteamVR_Actions._default.CycleHotbarRight);
	public static Button rotateLeft = new Button(SteamVR_Actions._default.RotateBuildingLeft);
	public static Button rotateRight = new Button(SteamVR_Actions._default.RotoateBuildingRight);
	public static Button toggleErase = new Button(SteamVR_Actions._default.ToggleErase);

	public static Button snapTurnLeft = new Button(SteamVR_Actions._default.SnapTurnLeft);
	public static Button snapTurnRight = new Button(SteamVR_Actions._default.SnapTurnRight);
	public static Button teleport = new Button(SteamVR_Actions._default.Teleport);

	public static void MapActions()
	{
		TechtonicaVR.Plugin.Logger.LogInfo("Mapping SteamVR actions...");
		SteamVR_Actions._default.Move.AddOnUpdateListener(HandleSteamVRMove, SteamVR_Input_Sources.Any);
		SteamVR_Actions._default.MenuJoystickPrimary.AddOnUpdateListener(HandleSteamVRMenuJoystickPrimary, SteamVR_Input_Sources.Any);
		SteamVR_Actions._default.MenuJoystickSecondary.AddOnUpdateListener(HandleSteamVRMenuJoystickSecondary, SteamVR_Input_Sources.Any);
		SteamVR_Actions._default.SmoothTurn.AddOnUpdateListener(HandleSteamVRSmoothTurn, SteamVR_Input_Sources.Any);

		SteamVR_Actions._default.PoseLeft.AddOnUpdateListener(SteamVR_Input_Sources.Any, LeftHandUpdate);
		SteamVR_Actions._default.PoseRight.AddOnUpdateListener(SteamVR_Input_Sources.Any, RightHandUpdate);
	}

	public static void UnmapActions()
	{
		TechtonicaVR.Plugin.Logger.LogInfo("Unmapping SteamVR actions...");
		SteamVR_Actions._default.Move.RemoveOnUpdateListener(HandleSteamVRMove, SteamVR_Input_Sources.Any);
		SteamVR_Actions._default.MenuJoystickPrimary.RemoveOnUpdateListener(HandleSteamVRMenuJoystickPrimary, SteamVR_Input_Sources.Any);
		SteamVR_Actions._default.MenuJoystickSecondary.RemoveOnUpdateListener(HandleSteamVRMenuJoystickSecondary, SteamVR_Input_Sources.Any);
		SteamVR_Actions._default.SmoothTurn.RemoveOnUpdateListener(HandleSteamVRSmoothTurn, SteamVR_Input_Sources.Any);

		SteamVR_Actions._default.PoseLeft.RemoveOnUpdateListener(SteamVR_Input_Sources.Any, LeftHandUpdate);
		SteamVR_Actions._default.PoseRight.RemoveOnUpdateListener(SteamVR_Input_Sources.Any, RightHandUpdate);
	}

	private static void HandleSteamVRMove(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
	{
		MoveAxes = axis;
	}

	private static void HandleSteamVRSmoothTurn(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
	{
		TurnAxis = axis.x;
	}

	private static void HandleSteamVRMenuJoystickPrimary(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
	{
		UIAxesPrimary = axis;
	}

	private static void HandleSteamVRMenuJoystickSecondary(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
	{
		UIAxesSecondary = axis;
	}

	private static void LeftHandUpdate(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
	{
		if (leftHandObject == null)
		{
			return;
		}

		leftHandObject.transform.localPosition = fromAction.localPosition;
		leftHandObject.transform.localRotation = fromAction.localRotation;
	}

	private static void RightHandUpdate(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
	{
		if (rightHandObject == null)
		{
			return;
		}

		rightHandObject.transform.localPosition = fromAction.localPosition;
		rightHandObject.transform.localRotation = fromAction.localRotation;
	}
}
