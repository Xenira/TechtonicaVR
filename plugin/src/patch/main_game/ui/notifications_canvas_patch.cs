using TechtonicaVR.Input;
using TechtonicaVR.UI;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

class NotificationCanvasPatche : GameComponentPatch<NotificationUI>

{
	protected override bool Apply(NotificationUI component)
	{
		var tlc = component.gameObject.transform.GetChild(0);
		if (tlc == null)
		{
			return false;
		}

		var trackedCanvas = tlc.gameObject.AddComponent<HandTrackedCanvas>();
		trackedCanvas.hand = SteamVRInputMapper.leftHandObject.transform;
		trackedCanvas.showDirection = Vector3.right;
		trackedCanvas.offset = new Vector3(0.08f, -0.03f, -0.1f);
		trackedCanvas.showDistance = 0.2f;
		trackedCanvas.rectTransform = tlc.GetChild(0).GetComponent<RectTransform>();

		tlc.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
		for (int i = 0; i < tlc.childCount; i++)
		{
			tlc.GetChild(i).localPosition = Vector3.zero;
		}

		return true;
	}
}
