using Plugin.Input;
using TechtonicaVR.Debug;
using TechtonicaVR.UI;
using TechtonicaVR.VRCamera;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

class QuestTaskListPatch : GameComponentPatch<QuestUI>

{
	protected override bool Apply(QuestUI component)
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
		tlc.GetChild(0).localPosition = Vector3.zero;

		var debugLine = tlc.gameObject.AddComponent<DebugLine>();
		debugLine.start = trackedCanvas.hand;
		debugLine.end = tlc;

		return true;
	}
}
