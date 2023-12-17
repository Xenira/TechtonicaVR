using HarmonyLib;

namespace TechtonicaVR.VRCamera.Patch;

[HarmonyPatch]
class PlayerFirstPersonControllerPatch
{
	[HarmonyPatch(typeof(PlayerFirstPersonController), nameof(PlayerFirstPersonController.AutoCrouch))]
	[HarmonyPrefix]
	static bool StartPatch()
	{
		return true;
	}
}
