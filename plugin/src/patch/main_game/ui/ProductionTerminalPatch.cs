using PiUtils.Patches;
using PiUtils.Util;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

public class ProductionTerminalPatch : GameComponentPatch<ProductionTerminalMenu>
{
	protected override bool Apply(ProductionTerminalMenu component)
	{
		var tlc = component.transform.GetChild(0);
		if (tlc == null)
		{
			return false;
		}

		var celebration = tlc.GetComponentInChildren<ProductionTerminalCelebration>();
		if (celebration == null)
		{
			return false;
		}

		var celebrationText = GameObjectFinder.FindChildObjectByName("VFX_AnimatedScaled_Text", celebration.gameObject);
		if (celebrationText == null)
		{
			return false;
		}

		foreach (var child in celebration.GetComponentsInChildren<Transform>())
		{
			child.gameObject.layer = 0;
		}
		celebrationText.transform.localPosition += new Vector3(0, 0, -250);

		tlc.localPosition = Vector3.zero;
		return true;
	}
}
