using TechtonicaVR.Input.Ui;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

public class StorageInventoryUIPatch : GameObjectPatch
{
	public StorageInventoryUIPatch() : base("Storage Menu")
	{
	}

	protected override bool Apply(GameObject component)
	{
		var storageInventory = component.GetComponentInChildren<InventoryGridUI>();
		if (storageInventory == null)
		{
			return false;
		}

		new InventoryInteractableUI(storageInventory.gameObject);

		return true;
	}
}
