using PiUtils.Patches;
using PiVrLoader.Input;
using PiVrLoader.UI;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

class CompassPatch : GameComponentPatch<CompassUI>

{
	protected override bool Apply(CompassUI component)
	{
		var tlc = component.gameObject.transform.GetChild(0);
		if (tlc == null)
		{
			return false;
		}

		var trackedCanvas = tlc.gameObject.AddComponent<HandTrackedCanvas>();
		trackedCanvas.hand = SteamVRInputMapper.rightHandObject.transform;
		trackedCanvas.showDirection = Vector3.right;
		trackedCanvas.offset = new Vector3(0.08f, -0.03f, -0.1f);
		trackedCanvas.showDistance = 0.3f;
		trackedCanvas.rectTransform = tlc.GetChild(0).GetComponent<RectTransform>();

		tlc.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
		for (int i = 0; i < tlc.childCount; i++)
		{
			tlc.GetChild(i).localPosition = Vector3.zero;
		}

		return true;
	}
}
