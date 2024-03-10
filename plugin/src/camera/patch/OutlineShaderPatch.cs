using HarmonyLib;
using UnityEngine;

namespace TechtonicaVR.VRCamera.Patch;

[HarmonyPatch]
public static class OutlineShaderPatch
{
    [HarmonyPatch(typeof(OutlinePostProcess), nameof(OutlinePostProcess.SetTargetOutlineable), [typeof(bool)])]
    [HarmonyPrefix]
    public static bool StartPostfix()
    {
        return false;
    }

    [HarmonyPatch(typeof(OutlinePostProcess), nameof(OutlinePostProcess.SetTargetOutlineable), [typeof(IStreamedMachineInstance), typeof(bool), typeof(bool)])]
    [HarmonyPrefix]
    public static bool StartPostfixOverload()
    {
        return false;
    }
}