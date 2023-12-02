using BepInEx.Configuration;

namespace TechtonicaVR;

public class ModConfig
{
    // General
    public static ConfigEntry<bool> modEnabled;

    // Input
    public static ConfigEntry<int> smoothTurnSpeed;

    // Buttons
    public static ConfigEntry<float> longPressTime;

    // Debug
    public static ConfigEntry<bool> debugMode;
    public static ConfigEntry<bool> debugLineEnabled;

    public static void Init(ConfigFile config)
    {
        // General
        modEnabled = config.Bind("General", "Enabled", true, "Enable mod");

        // Input
        smoothTurnSpeed = config.Bind("Input", "Smooth Turn Speed", 90, "Speed of smooth turning");

        // Buttons
        longPressTime = config.Bind("Buttons", "Long Press Time", 1f, "Time to hold button for long press");

        // Debug
        debugMode = config.Bind("Debug", "Debug Mode", false, "Enable debug mode");
        debugLineEnabled = config.Bind("Debug", "Debug Line Enabled", false, "Enable debug lines");
    }

    public static bool ModEnabled()
    {
        return modEnabled.Value;
    }

    public static bool DebugLineEnabled()
    {
        return debugMode.Value && debugLineEnabled.Value;
    }
}