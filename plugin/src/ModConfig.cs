using BepInEx.Configuration;
using UnityEngine;

namespace TechtonicaVR;

public class ModConfig
{
	// General
	private static ConfigEntry<bool> modEnabled;

	// Input
	public static ConfigEntry<int> smoothTurnSpeed;

	// Comfort
	public static ConfigEntry<float> snapTurnAngle;
	private static ConfigEntry<bool> vignetteEnabled;
	private static ConfigEntry<bool> vignetteOnTeleport;
	private static ConfigEntry<bool> vignetteOnSmoothLocomotion;
	private static ConfigEntry<bool> vignetteOnSmoothTurn;
	private static ConfigEntry<bool> vignetteOnSnapTurn;

	// Buttons
	public static ConfigEntry<float> longPressTime;

	// Graphics
	public static ConfigEntry<bool> displayBody;

	// UI
	public static ConfigEntry<float> menuSpawnDistance;
	public static ConfigEntry<Vector3> menuScale;
	public static ConfigEntry<Vector3> inventoryAndCraftingMenuScaleOverride;
	public static ConfigEntry<float> menuDownwardOffset;
	public static ConfigEntry<float> menuScrollSpeed;
	public static ConfigEntry<float> menuScrollDeadzone;

	public static void Init(ConfigFile config)
	{
		// General
		modEnabled = config.Bind("General", "Enabled", true, "Enable mod");

		// Input
		smoothTurnSpeed = config.Bind("Input", "Smooth Turn Speed", 90, "Speed of smooth turning");

		// Comfort
		snapTurnAngle = config.Bind("Comfort", "Snap Turn Angle", 30f, "Angle of snap turning");
		vignetteEnabled = config.Bind("Comfort", "Vignette Enabled", false, "Enable vignette");
		vignetteOnTeleport = config.Bind("Comfort", "Vignette On Teleport", true, "Enable vignette on teleport");
		vignetteOnSmoothLocomotion = config.Bind("Comfort", "Vignette On Smooth Locomotion", true, "Enable vignette on smooth locomotion");
		vignetteOnSmoothTurn = config.Bind("Comfort", "Vignette On Smooth Turn", true, "Enable vignette on smooth turn");
		vignetteOnSnapTurn = config.Bind("Comfort", "Vignette On Snap Turn", true, "Enable vignette on snap turn");

		// Buttons
		longPressTime = config.Bind("Buttons", "Long Press Time", 1f, "Time to hold button for long press");

		// Graphics
		displayBody = config.Bind("Graphics", "Display Body", true, "Display player body. If disabled, only hands are visible. Does not affect other players");

		// UI
		menuSpawnDistance = config.Bind("UI", "Menu Spawn Distance", 0.8f, "Distance from head to spawn Menus");
		menuScale = config.Bind("UI", "Menu Scale", new Vector3(0.001f, 0.001f, 0.001f), "Scale of Menus");
		inventoryAndCraftingMenuScaleOverride = config.Bind("UI", "Inventory and Crafting Menu Scale Override", new Vector3(0.001f, 0.0005f, 0.001f), "Scale of Inventory and Crafting Menu. Set to 0 to use Menu Scale");
		menuDownwardOffset = config.Bind("UI", "Menu Downward Offset", 0.2f, "Offset of Menus from head. Needed, as menus sometimes spawn too high.");
		menuScrollSpeed = config.Bind("UI", "Menu Scroll Speed", 0.125f, "Speed of scrolling in menus");
		menuScrollDeadzone = config.Bind("UI", "Menu Scroll Deadzone", 0.35f, "Deadzone of scrolling in menus");
	}

	public static bool ModEnabled()
	{
		return modEnabled.Value;
	}

	// Comfort
	public static bool VignetteEnabled()
	{
		return vignetteEnabled.Value;
	}

	public static bool VignetteOnTeleport()
	{
		return VignetteEnabled() && vignetteOnTeleport.Value;
	}

	public static bool VignetteOnSmoothLocomotion()
	{
		return VignetteEnabled() && vignetteOnSmoothLocomotion.Value;
	}

	public static bool VignetteOnSmoothTurn()
	{
		return VignetteEnabled() && vignetteOnSmoothTurn.Value;
	}

	public static bool VignetteOnSnapTurn()
	{
		return VignetteEnabled() && vignetteOnSnapTurn.Value;
	}
}
