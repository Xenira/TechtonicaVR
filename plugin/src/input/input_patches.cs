using System.Linq;
using HarmonyLib;
using Plugin.Input;
using UnityEngine;
using Valve.VR;

namespace TechtonicaVR.Input;

// Action: Move Horizontal (0)
// Action: Move Vertical (1)
// Action: Look Horizontal (2)
// Action: Look Vertical (3)
// Action: Jump (4)
// Action: Interact (5)
// Action: Pause (9)
// Action: Sprint (16)
// Action: Rotate Left (19)
// Action: Rotate Clockwise (38)
// Action: Rotate Counterclockwise (42)
// Action: Toggle Erase (43)
// Action: Crafting Menu (45)
// Action: Erase (46)
// Action: Cycle Hotbar Left (52)
// Action: Cycle Hotbar Right (97)
// Action: Cycle Toolbar Up (71)
// Action: Cycle Toolbar Down (98)
// Action: UI Horizontal Secondary (57)
// Action: UI Horizontal Primary (61)
// Action: UI Vertical Secondary (58)
// Action: UI Vertical Primary (62)
// Action: UI Submit (59)
// Action: UI Cancel (60)
// Action: KB CancelMenu (134)
// Action: KB PowerMenu (135)
// Action: KB TechTree (136)
// Action: UI Shortcut 1 (63)
// Action: UI Shortcut 2 (64)
// Action: UIPageLeft (65)
// Action: UIPageRight (66)
// Action: UI Shortcut 3 (67)
// Action: Use (69)
// Action: Put Away (70)
// Action: Hotbar 0 (72)
// Action: Hotbar 9 (81)
// Action: Hotbar 8 (80)
// Action: Hotbar 7 (79)
// Action: Hotbar 6 (78)
// Action: Hotbar 5 (77)
// Action: Hotbar 4 (76)
// Action: Hotbar 3 (75)
// Action: Hotbar 2 (74)
// Action: Hotbar 1 (73)
// Action: Third Person View (82)
// Action: Craft (83)
// Action: Craft Five (84)
// Action: Craft All (85)
// Action: Clear Queue (86)
// Action: Transfer (87)
// Action: Drop (88)
// Action: Transfer Single (89)
// Action: Transfer Half (90)
// Action: Transfer All (91)
// Action: Take All Shortcut (92)
// Action: Sort Inventory (133)
// Action: Rotate Horizontal (93)
// Action: Rotate Vertical (94)
// Action: Zoom In (95)
// Action: Zoom Out (96)
// Action: Shortcuts Menu (99)
// Action: Open Mole Menu (100)
// Action: Apply and Close Mole Menu (101)
// Action: View Map (103)
// Action: Overhead Toggle Vertical Movement (105)
// Action: Toggle Perspective (106)
// Action: Edit Shotcut (107)
// Action: Open Hotbar Selection (108)
// Action: Swap Variation (109)
// Action: Strafe Up (110)
// Action: Strafe Down (111)
// Action: Toggle Overlay Mode (112)
// Action: Cycle Forward (113)
// Action: Cycle Backward (114)
// Action: Hotbar B 0 (115)
// Action: Hotbar B 1 (116)
// Action: Hotbar B 2 (117)
// Action: Hotbar B 3 (118)
// Action: Hotbar B 4 (119)
// Action: Hotbar B 5 (120)
// Action: Hotbar B 6 (121)
// Action: Hotbar B 7 (122)
// Action: Hotbar B 8 (123)
// Action: Hotbar B 9 (124)
// Action: Change Top Toolbar (125)
// Action: Change Bottom Toolbar (126)
// Action: Hotbar Edit 1 (127)
// Action: Hotbar Edit 2 (128)
// Action: Hotbar Edit Single (129)
// Action: Clear Shortcut (130)
// Action: Lock Toolbar (131)
// Action: Exit Hotbar (132)
// Action: MouseLeftClick (137)
// Action: MouseRightClick (138)
// Action: KB DatabankMenu (139)
// Action: KB Log Menu (140)
// Action: KB Journal Menu (141)
// Action: KB Map (150)
// Action: Quick Access (142)
// Action: UIPageLeftSecondary (143)
// Action: UIPageRightSecondary (144)
// Action: Frequency Select (145)
// Action: Frequency Step (146)
// Action: Frequency Pan (147)
// Action: UI Toggle Menu (151)

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
        if (!__instance.m_IsGrounded)
        {
            return;
        }

        var delta = Time.deltaTime;
        var horizontalRotation = SteamVRInputMapper.TurnAxis * delta * 1f;
        __instance.gameObject.transform.rotation = Quaternion.Euler(0, __instance.gameObject.transform.rotation.eulerAngles.y + horizontalRotation, 0);
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

    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonTimedPressDown), [typeof(int), typeof(float), typeof(float)])]
    // static bool GetButtonTimedPressDown(ref bool __result, int actionId, float time, float expireIn)
    // {
    //     var state = MapButtonState(actionId);
    //     if (state != null)
    //     {
    //         __result = state.IsTimedPressDown(time, expireIn);
    //         return false;
    //     }

    //     return true;
    // }

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

    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.LookAxes), MethodType.Getter)]
    // static bool GetLookAxes(InputHandler __instance, ref Vector2 __result)
    // {
    //     if (__instance.playerInputBlocked || __instance.playerInputBlockedOverride || __instance.playerAimStickBlocked)
    //     {
    //         __result = Vector2.zero;
    //     }
    //     else
    //     {
    //         __result = new Vector2(SteamVRInputMapper.TurnAxis, 0);
    //     }

    //     return false;
    // }

    private static Button MapButtonState(int actionId)
    {
        switch (actionId)
        {
            case RewiredConsts.Action.Jump:
                return SteamVRInputMapper.Jump;
            case RewiredConsts.Action.Interact:
            case RewiredConsts.Action.Use:
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
            case RewiredConsts.Action.Cycle_Hotbar_Left:
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
            case RewiredConsts.Action.Cycle_Hotbar_Right:
                return SteamVRInputMapper.cycleHotbarRight;
            case RewiredConsts.Action.KB_TechTree:
                return SteamVRInputMapper.TechTree;
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