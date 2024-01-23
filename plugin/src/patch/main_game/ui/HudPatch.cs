using TechtonicaVR.Util;

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

		buttonMappingsCanvas.SetActive(false);
		return true;
	}
}
