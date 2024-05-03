using PiUtils.Patches;
using PiVrLoader.Input;
using PiVrLoader.UI;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

class MapPatch : GameComponentPatch<MapHUD>

{
	protected override bool Apply(MapHUD component)
	{
		var tlc = component.transform.GetChild(0);
		if (tlc == null)
		{
			return false;
		}

		var trackedCanvas = tlc.gameObject.AddComponent<HandTrackedCanvas>();
		trackedCanvas.hand = SteamVRInputMapper.rightHandObject.transform;
		trackedCanvas.showDirection = Vector3.right;
		trackedCanvas.offset = new Vector3(0.08f, -0.03f, -0.1f);
		trackedCanvas.showDistance = 0.2f;
		// trackedCanvas.noTransform = true;
		trackedCanvas.rectTransform = tlc.GetChild(0).GetComponent<RectTransform>();
		trackedCanvas.transformOverride = new Vector3(100, 200, 0);

		tlc.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
		for (int i = 0; i < tlc.childCount; i++)
		{
			tlc.GetChild(i).localPosition = Vector3.zero;
		}

		return true;
	}
}
