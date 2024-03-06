using PiUtils.Patches;
using PiUtils.Util;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

public class HudPatch : GameComponentPatch<HUD>
{
	protected override bool Apply(HUD component)
	{
		var buttonMappingsCanvas = GameObjectFinder.FindChildObjectByName("Button Mappings Canvas", component.gameObject);
		if (buttonMappingsCanvas == null)
		{
			return false;
		}

		var encumbranceCanvas = GameObjectFinder.FindChildObjectByName("Encumbered Warning", component.gameObject);
		if (encumbranceCanvas == null)
		{
			return false;
		}

		buttonMappingsCanvas.SetActive(false);

		encumbranceCanvas.transform.localPosition = new Vector3(-200, 250, -500);

		return true;
	}
}
