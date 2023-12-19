using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace TechtonicaVR;

public class ModConfig
{
	// General
	public static ConfigEntry<bool> modEnabled;

	// Input
	public static ConfigEntry<int> smoothTurnSpeed;
	public static ConfigEntry<float> snapTurnAngle;
	public static ConfigEntry<float> teleportRange;

	// Buttons
	public static ConfigEntry<float> longPressTime;

	// Graphics
	public static ConfigEntry<int> targetFPS;

	// UI
	public static ConfigEntry<float> menuSpawnDistance;
	public static ConfigEntry<Vector3> menuScale;
	public static ConfigEntry<Vector3> inventoryAndCraftingMenuScaleOverride;
	public static ConfigEntry<float> menuDownwardOffset;

	// Debug
	public static ConfigEntry<bool> debugMode;
	public static ConfigEntry<bool> gizmoEnabled;
	public static ConfigEntry<bool> debugLineEnabled;

	public static void Init(ConfigFile config)
	{
		// General
		modEnabled = config.Bind("General", "Enabled", true, "Enable mod");

		// Input
		smoothTurnSpeed = config.Bind("Input", "Smooth Turn Speed", 90, "Speed of smooth turning");
		snapTurnAngle = config.Bind("Input", "Snap Turn Angle", 30f, "Angle of snap turning");
		teleportRange = config.Bind("Input", "Teleport Range", 12f, "Range of teleporting");

		// Buttons
		longPressTime = config.Bind("Buttons", "Long Press Time", 1f, "Time to hold button for long press");

		// Graphics
		targetFPS = config.Bind("Graphics", "Target FPS", 144, "Target FPS. No effect right now, but will be used in the future, once I figure out how to do it");

		// UI
		menuSpawnDistance = config.Bind("UI", "Menu Spawn Distance", 0.8f, "Distance from head to spawn Menus");
		menuScale = config.Bind("UI", "Menu Scale", new Vector3(0.2f, 0.2f, 0.2f), "Scale of Menus");
		inventoryAndCraftingMenuScaleOverride = config.Bind("UI", "Inventory and Crafting Menu Scale Override", new Vector3(0.2f, 0.1f, 0.2f), "Scale of Inventory and Crafting Menu. Set to 0 to use Menu Scale");
		menuDownwardOffset = config.Bind("UI", "Menu Downward Offset", 0.2f, "Offset of Menus from head. Needed, as menus sometimes spawn too high.");

		// Debug
		debugMode = config.Bind("Debug", "Debug Mode", false, "Enable debug mode");
		gizmoEnabled = config.Bind("Debug", "Gizmo Enabled", false, "Enable gizmos");
		debugLineEnabled = config.Bind("Debug", "Debug Line Enabled", false, "Enable debug lines");
	}

	public static bool ModEnabled()
	{
		return modEnabled.Value;
	}

	public static bool GizmoEnabled()
	{
		return debugMode.Value && gizmoEnabled.Value;
	}

	public static bool DebugLineEnabled()
	{
		return debugMode.Value && debugLineEnabled.Value;
	}
}
