using System.Collections.Generic;
using HarmonyLib;
using TechtonicaVR.Util.Patch;

namespace TechtonicaVR.VRCamera.Patch.Builder;

[HarmonyPatch]
class ConveyorBuilderPatch
{
	[HarmonyPatch(typeof(ConveyorBuilder), nameof(ConveyorBuilder.GetTargetEndPt))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> ReassembleTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return Transpile.reassemblePatch<ConveyorBuilder>(instructions, 5);
	}
}
