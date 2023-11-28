using HarmonyLib;

namespace TechtonicaVR.Patches;

[HarmonyPatch]
public class ArmsSpringPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(FirstPersonArmsSpring), nameof(FirstPersonArmsSpring.OnEnable))]
    private static void OnEnable(FirstPersonArmsSpring __instance)
    {
        // __instance.gameObject.SetActive(false);
    }
}