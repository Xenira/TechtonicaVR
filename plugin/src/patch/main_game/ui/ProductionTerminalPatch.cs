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

		tlc.localPosition = Vector3.zero;
		return true;
	}
}
