using PiUtils.Patches;
using PiVrLoader.Input;
using PiVrLoader.UI;
using TechtonicaVR.Input.Ui;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

class ToolbarUiPatch : GameComponentPatch<ToolbarUI>

{
	protected override bool Apply(ToolbarUI component)
	{
		var trackedCanvas = component.gameObject.AddComponent<HandTrackedCanvas>();
		trackedCanvas.hand = SteamVRInputMapper.leftHandObject.transform;
		trackedCanvas.showDirection = -Vector3.right;
		trackedCanvas.offset = new Vector3(-0.08f, 0.06f, -0.1f);
		trackedCanvas.showDistance = 0.3f;
		trackedCanvas.noTransform = true;

		component.transform.parent.localScale = new Vector3(-0.1f, 0.1f, 0.05f);
		for (int i = 0; i < component.transform.childCount; i++)
		{
			component.transform.GetChild(i).localPosition = Vector3.zero;
		}

		return true;
	}

}

class PrimaryToolbarPatch : GameObjectPatch
{
	public PrimaryToolbarPatch() : base("Primary Toolbar")
	{
	}

	protected override bool Apply(GameObject gameObject)
	{
		new ToolbarInteractableUi(gameObject);
		return true;
	}
}

class SecondaryToolbarPatch : GameObjectPatch
{
	public SecondaryToolbarPatch() : base("Secondary Toolbar")
	{
	}

	protected override bool Apply(GameObject gameObject)
	{
		new ToolbarInteractableUi(gameObject);
		return true;
	}
}
