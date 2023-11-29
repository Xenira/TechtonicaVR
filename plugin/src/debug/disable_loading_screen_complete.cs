using HarmonyLib;

namespace TechtonicaVR.Debug;

[HarmonyPatch]
public class DisableLoadingScreenComplete
{
    [HarmonyPatch(typeof(LoadingUI), "Update")]
    [HarmonyPrefix]
    public static bool Update()
    {
        // Set to false to disable loading screen skipping
        return true;
    }
}