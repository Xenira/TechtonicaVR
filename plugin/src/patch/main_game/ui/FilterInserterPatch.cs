using TechtonicaVR.Input.Ui.Machine;
using TechtonicaVR.Util;

namespace TechtonicaVR.Patches.MainGame.UI;

public class FilterInserterPatch : GameComponentPatch<FilterInserterUI>
{
	protected override bool Apply(FilterInserterUI component)
	{
		new FilterInserterInteractableUi(GameObjectFinder.FindChildObjectByName("Filter Options Container", component.gameObject));
		return true;
	}
}
