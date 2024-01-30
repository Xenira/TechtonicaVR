using HarmonyLib;

namespace TechtonicaVR.UI.Patch;

[Harmony]
public class HologramPreviewManagerPatch
{
	[HarmonyPatch(typeof(HologramPreviewManager), nameof(HologramPreviewManager.Update))]
	[HarmonyPrefix]
	public static bool UpdatePrefix(HologramPreviewManager __instance)
	{
		return (!UIManager.instance?.anyMenuOpen) ?? true;
	}
}
