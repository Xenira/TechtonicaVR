using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TechtonicaVR.Input;
using UnityEngine;

namespace TechtonicaVR.VRCamera.Patch.Builder;

[HarmonyPatch]
public class PlayerInspectorPatch
{
	private const int EXPECTED_INSPECTOR_UPDATE_PATCH_COUNT = 1;

	static MethodInfo raycastMethod = typeof(Physics).GetMethod(nameof(Physics.Raycast), [typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(float), typeof(int)]);
	static MethodInfo raycastPatchMethod = typeof(PlayerInspectorPatch).GetMethod(nameof(PatchedRaycast));

	[HarmonyPatch(typeof(PlayerInspector), nameof(PlayerInspector.LateUpdate))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> InspectorUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		var patchCnt = 0;
		foreach (var instruction in instructions)
		{
			if (instruction.Calls(raycastMethod))
			{
				yield return new CodeInstruction(OpCodes.Call, raycastPatchMethod);
				patchCnt++;
			}
			else
			{
				yield return instruction;
			}
		}

		if (patchCnt != EXPECTED_INSPECTOR_UPDATE_PATCH_COUNT)
		{
			Plugin.Logger.LogError($"[PlayerInspector.LateUpdate] Patch count mismatch: {patchCnt} != {EXPECTED_INSPECTOR_UPDATE_PATCH_COUNT}");
		}
	}

	public static bool PatchedRaycast(Vector3 _origin, Vector3 _direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
	{
		if (SteamVRInputMapper.rightHandObject == null)
		{
			hitInfo = default;
			return false;
		}

		return Physics.Raycast(SteamVRInputMapper.rightHandObject.transform.position, -SteamVRInputMapper.rightHandObject.transform.up, out hitInfo, maxDistance, layerMask);
	}

}
