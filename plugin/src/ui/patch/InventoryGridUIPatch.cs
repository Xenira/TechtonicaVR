using HarmonyLib;

namespace TechtonicaVR.UI.Patch;

[Harmony]
public class InventoryGridUIPatch
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(BaseResourceGridUI.SlotsSetup), nameof(BaseResourceGridUI.SlotsSetup.shiftCount), MethodType.Getter)]
	public static void shiftCountPrefix(BaseResourceGridUI.SlotsSetup __instance)
	{
		__instance._shiftCount = (__instance.rotateSlots ? (__instance.scrollContent.anchoredPosition.y / __instance.spacing.y).Abs().FloorToInt() : 0);
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(BaseResourceGridUI), nameof(BaseResourceGridUI.ResetToTop))]
	public static bool ResetToTopPrefix(BaseResourceGridUI __instance)
	{
		__instance.slotsSetup.ResetScrollToTop();
		return false;
	}
}
