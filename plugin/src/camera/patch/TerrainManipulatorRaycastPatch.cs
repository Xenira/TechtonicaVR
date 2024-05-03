using HarmonyLib;
using PiVrLoader.Input;
using UnityEngine;

namespace TechtonicaVR.VRCamera.Patch;

[HarmonyPatch]
public class TerrainManipulatorRaycastPatch
{
	[HarmonyPatch(typeof(TerrainManipulator_TunnelMode), nameof(TerrainManipulator_TunnelMode.GetCenterPoint))]
	[HarmonyPrefix]
	public static void GetCenterPointPrefix(TerrainManipulator_TunnelMode __instance)
	{
		if (!isReady())
		{
			return;
		}

		UpdateStillCam();
	}

	[HarmonyPatch(typeof(TerrainManipulator_FlattenMode), nameof(TerrainManipulator_FlattenMode.GetCenterPoint))]
	[HarmonyPrefix]
	public static void GetCenterPointPrefix(TerrainManipulator_FlattenMode __instance)
	{
		if (!isReady())
		{
			return;
		}

		UpdateStillCam();
	}

	private static void UpdateStillCam()
	{
		Transform leftHandTransform = SteamVRInputMapper.leftHandObject.transform;
		Vector3 forward = -leftHandTransform.up;
		Ray ray = new Ray(leftHandTransform.position, forward);

		bool targetsTerrain = Physics.Raycast(ray, out RaycastHit hit, 15f, Player.instance.builder.defaultMask, QueryTriggerInteraction.Ignore);

		Player.instance.stillCam.transform.LookAt(targetsTerrain ? hit.point : ray.GetPoint(15f));
	}

	private static bool isReady()
	{
		return Player.instance != null && Player.instance.stillCam != null && Player.instance.builder != null && SteamVRInputMapper.leftHandObject != null;
	}
}
