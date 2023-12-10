using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Plugin.Input;
using UnityEngine;

namespace TechtonicaVR.VRCamera.Patch;

[HarmonyPatch]
public class BuilderReassemblePatch
{
	private static MethodInfo getTransformMethod = typeof(Component).GetMethod("get_transform");
	private static MethodInfo getForwardMethod = typeof(Transform).GetMethod("get_forward");

	private static MethodInfo getTransformPatchMethod = typeof(BuilderReassemblePatch).GetMethod(nameof(patchedGetTransform));
	private static MethodInfo getForwardPatchMethod = typeof(BuilderReassemblePatch).GetMethod(nameof(PatchedGetForward));

	[HarmonyPatch(typeof(FloorBuilder), nameof(FloorBuilder.Reassemble))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> ReassembleTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return reassemblePatch(instructions);
	}

	[HarmonyPatch(typeof(ResearchCoreBuilder), nameof(ResearchCoreBuilder.Reassemble))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> ResearchCoreReassembleTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return reassemblePatch(instructions);
	}

	[HarmonyPatch(typeof(StairsBuilder), nameof(StairsBuilder.Reassemble))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> StairsBuilderTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return reassemblePatch(instructions);
	}

	private static IEnumerable<CodeInstruction> reassemblePatch(IEnumerable<CodeInstruction> instructions)
	{
		var patchCnt = 0;
		foreach (var instruction in instructions)
		{
			if (instruction.Calls(getTransformMethod))
			{
				yield return new CodeInstruction(OpCodes.Pop);
				yield return new CodeInstruction(OpCodes.Call, getTransformPatchMethod);
				patchCnt++;
			}
			else if (instruction.Calls(getForwardMethod))
			{
				yield return new CodeInstruction(OpCodes.Pop);
				yield return new CodeInstruction(OpCodes.Call, getForwardPatchMethod);
				patchCnt++;
			}
			else
			{
				yield return instruction;
			}
		}

		if (patchCnt != 12)
		{
			Plugin.Logger.LogError($"ReassembleTranspiler: Patch count mismatch: {patchCnt} != 12");
		}
	}

	public static Transform patchedGetTransform()
	{
		return SteamVRInputMapper.rightHandObject.transform;
	}

	public static Vector3 PatchedGetForward()
	{
		return -SteamVRInputMapper.rightHandObject.transform.up;
	}
}
