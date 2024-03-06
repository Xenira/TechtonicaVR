using PiUtils.Patches;
using TechtonicaVR.Input;
using TechtonicaVR.UI;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

class CraftingQueuePatch : GameComponentPatch<CraftingQueueGrid>

{
	protected override bool Apply(CraftingQueueGrid component)
	{
		var tlc = component.gameObject.transform.parent;
		if (tlc == null)
		{
			return false;
		}

		var trackedCanvas = tlc.gameObject.AddComponent<HandTrackedCanvas>();
		trackedCanvas.hand = SteamVRInputMapper.leftHandObject.transform;
		trackedCanvas.showDirection = -Vector3.right;
		trackedCanvas.offset = new Vector3(-0.08f, 0.06f, -0.1f);
		trackedCanvas.showDistance = 0.3f;
		trackedCanvas.noTransform = true;
		trackedCanvas.tlcLocalPosition = new Vector3(-17.7141f, 17.7794f, 0f);
		tlc.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
		for (int i = 0; i < tlc.childCount; i++)
		{
			tlc.GetChild(i).localPosition = Vector3.zero;
		}

		return true;
	}
}
