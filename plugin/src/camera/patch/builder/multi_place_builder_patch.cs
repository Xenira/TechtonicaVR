using System.Collections.Generic;
using HarmonyLib;
using TechtonicaVR.Util.Patch;

namespace TechtonicaVR.VRCamera.Patch.Builder;

[HarmonyPatch]
class MultiPlaceBuilderPatch
{
	[HarmonyPatch(typeof(MultiPlaceBuilder), nameof(MultiPlaceBuilder.Reassemble))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> ReassembleTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return Transpile.reassemblePatch<MultiPlaceBuilder>(instructions, 25);
	}
}
