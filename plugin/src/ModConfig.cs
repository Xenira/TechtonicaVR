using BepInEx.Configuration;
using UnityEngine;

namespace TechtonicaVR;

public class ModConfig
{
	// General
	private static ConfigEntry<bool> modEnabled;

	// Input
	public static ConfigEntry<int> smoothTurnSpeed;
	public static ConfigEntry<bool> laserUiOnly;
	public static ConfigEntry<Color> laserColor;
	public static ConfigEntry<Color> laserClickColor;
	public static ConfigEntry<Color> laserValidColor;
	public static ConfigEntry<Color> laserInvalidColor;
	public static ConfigEntry<float> laserThickness;
	public static ConfigEntry<float> laserClickThicknessMultiplier;

	// Comfort
	public static ConfigEntry<float> snapTurnAngle;
	public static ConfigEntry<float> teleportRange;
	private static ConfigEntry<bool> vignetteEnabled;
	private static ConfigEntry<bool> vignetteOnTeleport;
	private static ConfigEntry<bool> vignetteOnSmoothLocomotion;
	private static ConfigEntry<bool> vignetteOnSmoothTurn;
	private static ConfigEntry<bool> vignetteOnSnapTurn;
	public static ConfigEntry<Color> vignetteColor;
	public static ConfigEntry<float> vignetteIntensity;
	public static ConfigEntry<float> vignetteSmoothness;
	public static ConfigEntry<float> vignetteFadeSpeed;

	// Buttons
	public static ConfigEntry<float> clickTime;
	public static ConfigEntry<float> longPressTime;

	// Graphics
	public static ConfigEntry<int> targetFPS;

	// UI
	public static ConfigEntry<float> menuSpawnDistance;
	public static ConfigEntry<Vector3> menuScale;
	public static ConfigEntry<Vector3> inventoryAndCraftingMenuScaleOverride;
	public static ConfigEntry<float> menuDownwardOffset;

	// Debug
	private static ConfigEntry<bool> debugMode;
	private static ConfigEntry<bool> gizmoEnabled;
	private static ConfigEntry<bool> debugLineEnabled;

	public static void Init(ConfigFile config)
	{
		// General
		modEnabled = config.Bind("General", "Enabled", true, "Enable mod");

		// Input
		smoothTurnSpeed = config.Bind("Input", "Smooth Turn Speed", 90, "Speed of smooth turning");
		laserUiOnly = config.Bind("Input", "Laser UI Only", true, "Only use laser for UI");
		laserColor = config.Bind("Input", "Laser Color", Color.cyan, "Color of laser");
		laserClickColor = config.Bind("Input", "Laser Click Color", Color.blue, "Color of laser when clicking");
		laserValidColor = config.Bind("Input", "Laser Hover Color", Color.green, "Color of laser when hovering");
		laserInvalidColor = config.Bind("Input", "Laser Invalid Color", Color.red, "Color of laser when hovering over invalid object");
		laserThickness = config.Bind("Input", "Laser Thickness", 0.002f, "Thickness of laser");
		laserClickThicknessMultiplier = config.Bind("Input", "Laser Click Thickness Multiplier", 2f, "Thickness multiplier of laser when clicking");

		// Comfort
		snapTurnAngle = config.Bind("Comfort", "Snap Turn Angle", 30f, "Angle of snap turning");
		teleportRange = config.Bind("Comfort", "Teleport Range", 12f, "Range of teleporting");
		vignetteEnabled = config.Bind("Comfort", "Vignette Enabled", false, "Enable vignette");
		vignetteOnTeleport = config.Bind("Comfort", "Vignette On Teleport", true, "Enable vignette on teleport");
		vignetteOnSmoothLocomotion = config.Bind("Comfort", "Vignette On Smooth Locomotion", true, "Enable vignette on smooth locomotion");
		vignetteOnSmoothTurn = config.Bind("Comfort", "Vignette On Smooth Turn", true, "Enable vignette on smooth turn");
		vignetteOnSnapTurn = config.Bind("Comfort", "Vignette On Snap Turn", true, "Enable vignette on snap turn");
		vignetteColor = config.Bind("Comfort", "Vignette Color", new Color(0, 0, 0, 1f), "Color of vignette");
		vignetteIntensity = config.Bind("Comfort", "Vignette Intensity", 0.5f, "Intensity of vignette");
		vignetteSmoothness = config.Bind("Comfort", "Vignette Smoothness", 0.15f, "Smoothness of vignette");
		vignetteFadeSpeed = config.Bind("Comfort", "Vignette Fade Speed", 3f, "Fade speed of vignette");

		// Buttons
		clickTime = config.Bind("Buttons", "Click Time", 0.2f, "Speed for clicking. Higher values make it easier to click");
		longPressTime = config.Bind("Buttons", "Long Press Time", 1f, "Time to hold button for long press");

		// Graphics
		targetFPS = config.Bind("Graphics", "Target FPS", 144, "Target FPS. No effect right now, but will be used in the future, once I figure out how to do it");

		// UI
		menuSpawnDistance = config.Bind("UI", "Menu Spawn Distance", 0.8f, "Distance from head to spawn Menus");
		menuScale = config.Bind("UI", "Menu Scale", new Vector3(0.001f, 0.001f, 0.001f), "Scale of Menus");
		inventoryAndCraftingMenuScaleOverride = config.Bind("UI", "Inventory and Crafting Menu Scale Override", new Vector3(0.001f, 0.0005f, 0.001f), "Scale of Inventory and Crafting Menu. Set to 0 to use Menu Scale");
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

	// Debug
	public static bool GizmoEnabled()
	{
		return debugMode.Value && gizmoEnabled.Value;
	}

	public static bool DebugEnabled()
	{
		return debugMode.Value;
	}

	public static bool DebugLineEnabled()
	{
		return debugMode.Value && debugLineEnabled.Value;
	}
}
