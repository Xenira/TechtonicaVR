using BepInEx.Configuration;

namespace TechtonicaVR;

public class ModConfig
{
    // General
    public static ConfigEntry<bool> modEnabled;

    // Debug
    public static ConfigEntry<bool> debugMode;
    public static ConfigEntry<bool> debugLineEnabled;

    public static void Init(ConfigFile config)
    {
        // General
        modEnabled = config.Bind("General", "Enabled", true, "Enable mod");

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