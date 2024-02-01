using TechtonicaVR.Input.Ui;

namespace TechtonicaVR.Patches.MainGame.UI;

public class PlayerInventoryUIPatch : GameComponentPatch<PlayerInventoryUI>
{
	protected override bool Apply(PlayerInventoryUI component)
	{
		var playerInventory = component.gameObject.GetComponentInChildren<InventoryGridUI>();
		if (playerInventory == null)
		{
			return false;
		}

		new InventoryInteractableUi(playerInventory.gameObject);

		return true;
	}
}
