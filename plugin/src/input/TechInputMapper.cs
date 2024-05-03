using PiUtils.Util;
using PiVrLoader.Input;
using TTIK.Ik.FingerTracking;
using TTIK.Network;
using UnityEngine;
using Valve.VR;

namespace TechtonicaVR.Input;

public static class TechInputMapper
{
	private static PluginLogger Logger = new PluginLogger(typeof(SteamVRInputMapper));

	public static Vector2 UIAxesPrimary { get; private set; }
	public static Vector2 UIAxesSecondary { get; private set; }


	/// <summary>
	/// (current state, previous state)
	/// </summary>
	public static Button Sprint = new Button(SteamVR_Actions.default_Sprint);
	public static Button Jump = new Button(SteamVR_Actions._default.Jump);
	public static Button Interact = new Button(SteamVR_Actions._default.Interact, SteamVRInputMapper.DefaultState);
	public static Button Use = new Button(SteamVR_Actions._default.Use, SteamVRInputMapper.DefaultState);
	public static Button TechTree = new Button(SteamVR_Actions._default.TechTree);
	public static Button Inventory = new Button(SteamVR_Actions._default.Inventory);
	public static Button takeAll = new Button(SteamVR_Actions._default.TakeAll);
	public static Button craft = new Button(SteamVR_Actions._default.Craft);
	public static Button craftFive = new Button(SteamVR_Actions._default.CraftFive);
	public static Button craftAll = new Button(SteamVR_Actions._default.CraftAll);
	public static Button transfer = new Button(SteamVR_Actions._default.Transfer);
	public static Button transferHalf = new Button(SteamVR_Actions._default.TransferHalf);
	public static Button transferAll = new Button(SteamVR_Actions._default.TransferAll);
	public static Button UIPageLeft = new Button(SteamVR_Actions._default.UIPageLeft, SteamVRInputMapper.DefaultState);
	public static Button UIPageRight = new Button(SteamVR_Actions._default.UIPageRight, SteamVRInputMapper.DefaultState);
	public static Button UIPageLeftSecondary = new Button(SteamVR_Actions._default.UIPageLeftSecondary);
	public static Button UIPageRightSecondary = new Button(SteamVR_Actions._default.UIPageRightSecondary);
	public static Button UISubmit = new Button(SteamVR_Actions._default.UISubmit);
	public static Button UICancel = new Button(SteamVR_Actions._default.UICancel);
	public static Button HotbarEdit = new Button(SteamVR_Actions._default.ToggleHotbarEdit);
	public static Button HotbarSwapItem = new Button(SteamVR_Actions._default.HotbarSwap);
	public static Button HotbarExitEdit = new Button(SteamVR_Actions._default.ExitHotbarEdit);
	public static Button HotbarClear = new Button(SteamVR_Actions._default.ClearHotbar);
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

	public static Button Variant = new Button(SteamVR_Actions._default.Variant);

	// UI
	public static Button UIClick = new Button(SteamVR_Actions.UI.Click, SteamVRInputMapper.UiState);
	public static Button UIShortcut1 = new Button(SteamVR_Actions._default.UIShortcut1);
	public static Button UIShortcut2 = new Button(SteamVR_Actions._default.UIShortcut2);

	// IK
	// Not used until we have FBT
	// public static Button IKCalibrate = new Button(SteamVR_Actions.IK.Calibrate);

	public static Button Grab = new Button(SteamVR_Actions._default.GrabGrip);

	public static void MapActions()
	{
		Logger.LogInfo("Mapping SteamVR actions...");
		UIClick.action.actionSet.Activate();

		// Not used until we have FBT
		// IKCalibrate.action.actionSet.Activate();
		SteamVR_Actions._default.MenuJoystickPrimary.AddOnUpdateListener(HandleSteamVRMenuJoystickPrimary, SteamVR_Input_Sources.Any);
		SteamVR_Actions._default.MenuJoystickSecondary.AddOnUpdateListener(HandleSteamVRMenuJoystickSecondary, SteamVR_Input_Sources.Any);

		SteamVR_Actions._default.SkeletonLeftHand.AddOnChangeListener(UpdateLeftSkeleton);
		SteamVR_Actions._default.SkeletonRightHand.AddOnChangeListener(UpdateRightSkeleton);
	}

	private static void UpdateLeftSkeleton(SteamVR_Action_Skeleton skeleton)
	{
		UpdateSkeleton(HandType.Left, skeleton);
	}

	private static void UpdateRightSkeleton(SteamVR_Action_Skeleton skeleton)
	{
		UpdateSkeleton(HandType.Right, skeleton);
	}

	private static void UpdateSkeleton(HandType left, SteamVR_Action_Skeleton skeleton)
	{
		if (NetworkIkPlayer.localInstance == null)
		{
			return;
		}

		var curls = skeleton.GetFingerCurls();
		for (var i = 0; i < curls.Length; i++)
		{
			NetworkIkPlayer.localInstance.UpdateFingerCurl(left, (FingerType)i, curls[i]);
		}
	}

	public static void UnmapActions()
	{
		Logger.LogInfo("Unmapping SteamVR actions...");
		SteamVR_Actions._default.MenuJoystickPrimary.RemoveOnUpdateListener(HandleSteamVRMenuJoystickPrimary, SteamVR_Input_Sources.Any);
		SteamVR_Actions._default.MenuJoystickSecondary.RemoveOnUpdateListener(HandleSteamVRMenuJoystickSecondary, SteamVR_Input_Sources.Any);

		SteamVR_Actions._default.SkeletonLeftHand.RemoveOnChangeListener(UpdateLeftSkeleton);
		SteamVR_Actions._default.SkeletonRightHand.RemoveOnChangeListener(UpdateRightSkeleton);
	}

	private static void HandleSteamVRMenuJoystickPrimary(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
	{
		UIAxesPrimary = axis;
	}

	private static void HandleSteamVRMenuJoystickSecondary(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
	{
		UIAxesSecondary = axis;
	}
}
