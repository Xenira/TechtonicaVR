using HarmonyLib;
using UnityEngine;

namespace TechtonicaVR.VRCamera.Patch;

[HarmonyPatch]
class PlayerFirstPersonControllerPatch
{
	[HarmonyPatch(typeof(PlayerFirstPersonController), nameof(PlayerFirstPersonController.AutoCrouch))]
	[HarmonyPrefix]
	static bool AutoCrouchPatch()
	{
		return true;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(PlayerFirstPersonController), nameof(PlayerFirstPersonController.UpdateThirdPersonView))]
	public static bool UpdateThirdPersonView()
	{
		if (Player.instance.networkedPlayer && VRCameraManager.mainCamera != null)
		{
			Player.instance.networkedPlayer.display.transform.position = Player.instance.transform.position;
			Player.instance.networkedPlayer.display.transform.rotation = Quaternion.Euler(0f, VRCameraManager.mainCamera.transform.rotation.eulerAngles.y, 0f);
		}
		return false;
	}
}
