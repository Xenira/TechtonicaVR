using TechtonicaVR.Input;
using TechtonicaVR.UI;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

class OmniseekerPatch : GameComponentPatch<OmniseekerUI>

{
	protected override bool Apply(OmniseekerUI component)
	{
		var tlc = component.transform;
		if (tlc == null)
		{
			return false;
		}

		var trackedCanvas = tlc.gameObject.AddComponent<HandTrackedCanvas>();
		trackedCanvas.hand = SteamVRInputMapper.rightHandObject.GetComponentInChildren<Scanner>()?.transform;
		trackedCanvas.showDirection = (Vector3.forward + Vector3.left).normalized;
		trackedCanvas.offset = new Vector3(0.08f, -0.03f, -0.1f);
		trackedCanvas.showDistance = 0.3f;
		trackedCanvas.rectTransform = tlc.GetChild(0).GetComponent<RectTransform>();
		trackedCanvas.transformOverride = new Vector3(100, 450, 550);

		tlc.localScale = new Vector3(-0.1f, 0.1f, 0.1f);

		return true;
	}
}
