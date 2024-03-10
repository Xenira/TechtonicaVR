using System.Collections.Generic;
using HarmonyLib;
using TechtonicaVR.Util.Patch;

namespace TechtonicaVR.VRCamera.Patch.Builder;

[HarmonyPatch]
public class BuilderReassemblePatch
{
	private const int EXPECTED_PATCH_COUNT = 12;



	[HarmonyPatch(typeof(FloorBuilder), nameof(FloorBuilder.Reassemble))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> ReassembleTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return Transpile.reassemblePatch<FloorBuilder>(instructions);
	}

	[HarmonyPatch(typeof(ResearchCoreBuilder), nameof(ResearchCoreBuilder.Reassemble))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> ResearchCoreReassembleTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return Transpile.reassemblePatch<ResearchCoreBuilder>(instructions);
	}

	[HarmonyPatch(typeof(StairsBuilder), nameof(StairsBuilder.Reassemble))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> StairsBuilderTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		return Transpile.reassemblePatch<StairsBuilder>(instructions);
	}
}
