using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PiUtils.Util;
using PiVrLoader.Input;
using UnityEngine;

namespace TechtonicaVR.Util.Patch;

public class Transpile
{
	private static PluginLogger Logger = PluginLogger.GetLogger<Transpile>();

	public static MethodInfo getTransformMethod = typeof(Component).GetMethod("get_transform");
	public static MethodInfo getForwardMethod = typeof(Transform).GetMethod("get_forward");

	public static MethodInfo getTransformPatchMethod = typeof(Transpile).GetMethod(nameof(patchedGetTransform));
	public static MethodInfo getForwardPatchMethod = typeof(Transpile).GetMethod(nameof(PatchedGetForward));

	public static IEnumerable<CodeInstruction> reassemblePatch<T>(IEnumerable<CodeInstruction> instructions, int expectedPatchCount = 12)
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

		if (patchCnt != expectedPatchCount)
		{
			Logger.LogError($"[{typeof(T)}]: Patch count mismatch: {patchCnt} != {expectedPatchCount}");
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
