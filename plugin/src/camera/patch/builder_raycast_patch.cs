using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Plugin.Input;
using UnityEngine;

namespace TechtonicaVR.VRCamera.Patch;

[HarmonyPatch]
public class BuilderRaycastPatch
{
	static MethodInfo raycastMethod = typeof(Physics).GetMethod(nameof(Physics.Raycast), [typeof(Vector3), typeof(Vector3), typeof(RaycastHit).MakeByRefType(), typeof(float), typeof(int), typeof(QueryTriggerInteraction)]);
	static MethodInfo vectorMultiplyMethod = typeof(Vector3).GetMethod("op_Multiply", [typeof(Vector3), typeof(float)]);
	static MethodInfo vectorAddMethod = typeof(Vector3).GetMethod("op_Addition", [typeof(Vector3), typeof(Vector3)]);

	static MethodInfo raycastPatchMethod = typeof(BuilderRaycastPatch).GetMethod(nameof(PatchedRaycast));
	static MethodInfo vectorMultiplyPatchMethod = typeof(BuilderRaycastPatch).GetMethod(nameof(PatchedVectorMultiply));
	static MethodInfo vectorAddPatchMethod = typeof(BuilderRaycastPatch).GetMethod(nameof(PatchedVectorAdd));

	[HarmonyPatch(typeof(PlayerBuilder), nameof(PlayerBuilder.cameraOrigin), MethodType.Getter)]
	[HarmonyPrefix]
	public static bool cameraOriginPrefix(PlayerBuilder __instance, ref Vector3 __result)
	{
		if (SteamVRInputMapper.rightHandObject == null)
		{
			return true;
		}

		__result = SteamVRInputMapper.rightHandObject.transform.position;
		return false;
	}

	[HarmonyPatch(typeof(PlayerBuilder), nameof(PlayerBuilder.cameraDirection), MethodType.Getter)]
	[HarmonyPrefix]
	public static bool cameraDirectionPrefix(PlayerBuilder __instance, ref Vector3 __result)
	{
		if (SteamVRInputMapper.rightHandObject == null)
		{
			return true;
		}

		__result = -SteamVRInputMapper.rightHandObject.transform.up;
		return false;
	}

	[HarmonyPatch(typeof(PlayerBuilder), nameof(PlayerBuilder.DeconstructionUpdate))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> DeconstructionUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
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

		if (patchCnt != 3)
		{
			Plugin.Logger.LogError("Failed to patch PlayerBuilder.DeconstructionUpdate");
		}
	}

	[HarmonyPatch(typeof(PlayerBuilder), nameof(PlayerBuilder.FreeformUpdatePlacement))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> FreeformUpdatePlacementTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		var patchCnt = 0;
		foreach (var instruction in instructions)
		{
			if (instruction.Calls(vectorMultiplyMethod))
			{
				yield return new CodeInstruction(OpCodes.Call, vectorMultiplyPatchMethod);
				patchCnt++;
			}
			else if (instruction.Calls(vectorAddMethod))
			{
				yield return new CodeInstruction(OpCodes.Call, vectorAddPatchMethod);
				patchCnt++;
			}
			else
			{
				yield return instruction;
			}
		}

		if (patchCnt != 2)
		{
			Plugin.Logger.LogError("Failed to patch PlayerBuilder.FreeformUpdatePlacement");
		}
	}

	[HarmonyPatch(typeof(PlayerBuilder), nameof(PlayerBuilder.HasTargetDeconstructable))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> HasTargetDeconstructableUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
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

		if (patchCnt != 1)
		{
			Plugin.Logger.LogError("Failed to patch PlayerBuilder.HasTargetDeconstructable");
		}
	}

	[HarmonyPatch(typeof(PlayerBuilder), nameof(PlayerBuilder.CheckTargetLogisticsObject))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> CheckTargetLogisticsObjectTranspiler(IEnumerable<CodeInstruction> instructions)
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

		if (patchCnt != 1)
		{
			Plugin.Logger.LogError("Failed to patch PlayerBuilder.CheckTargetLogisticsObject");
		}
	}

	public static bool PatchedRaycast(Vector3 _origin, Vector3 _direction, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
	{
		return Physics.Raycast(SteamVRInputMapper.rightHandObject.transform.position, -SteamVRInputMapper.rightHandObject.transform.up, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static Vector3 PatchedVectorMultiply(Vector3 _vector, float multiplier)
	{
		return -SteamVRInputMapper.rightHandObject.transform.up * multiplier;
	}

	public static Vector3 PatchedVectorAdd(Vector3 vector, Vector3 _add)
	{
		return vector + SteamVRInputMapper.rightHandObject.transform.position;
	}

}
