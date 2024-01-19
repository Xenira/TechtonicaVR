using TechtonicaVR.Input.Ui;
using TechtonicaVR.UI;
using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

public class IaCMenuPatch : GameObjectPatch
{
	public IaCMenuPatch() : base("Inventory and Crafting Menus")
	{
	}

	protected override bool Apply(GameObject gameObject)
	{
		var inventoryCanvas = GameObjectFinder.FindChildObjectByName("Inventory Canvas", gameObject);
		if (inventoryCanvas == null)
		{
			return false;
		}

		var iac = GameObject.FindObjectOfType<InventoryAndCraftingUI>();
		if (iac == null)
		{
			return false;
		}

		var inventoryAnchor = new GameObject("Inventory UI Anchor");
		inventoryAnchor.transform.parent = iac.transform;
		inventoryAnchor.transform.localPosition = new Vector3(-616, -600, 0);
		inventoryAnchor.transform.localRotation = Quaternion.identity;
		inventoryAnchor.transform.localScale = new Vector3(1.1f, 2.35f, 1);

		var playerInventory = inventoryCanvas.GetComponentInChildren<InventoryGridUI>();

		var menu = new InventoryInteractableUI(playerInventory.gameObject);
		menu.transform = inventoryAnchor.transform;
		menu.menu = new UIMenuWrapper(iac);
		menu.OnEnterEvent += () =>
		{
			iac.inventoryHasFocus = true;
			iac.Refresh();
		};

		return true;
	}
}
