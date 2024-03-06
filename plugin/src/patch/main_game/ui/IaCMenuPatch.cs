using PiUtils.Patches;
using PiUtils.Util;
using TechtonicaVR.Input.Ui;
using TechtonicaVR.UI;
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

		var craftingCanvas = GameObjectFinder.FindChildObjectByName("Crafting Canvas", gameObject);
		if (craftingCanvas == null)
		{
			return false;
		}

		var iac = GameObject.FindObjectOfType<InventoryAndCraftingUI>();
		if (iac == null)
		{
			return false;
		}

		var allButtonMappings = GameObjectFinder.FindChildObjectByName("All Button Mappings", iac.gameObject);
		if (allButtonMappings != null)
		{
			allButtonMappings.SetActive(false);
		}

		var inventoryAnchor = new GameObject("Inventory UI Anchor");
		inventoryAnchor.transform.parent = iac.transform;
		inventoryAnchor.transform.localPosition = new Vector3(-616, -600, 0);
		inventoryAnchor.transform.localRotation = Quaternion.identity;
		inventoryAnchor.transform.localScale = new Vector3(1.1f, 2.35f, 1);

		var playerInventory = inventoryCanvas.GetComponentInChildren<InventoryGridUI>();

		var menu = new InventoryInteractableUi(playerInventory.gameObject);
		menu.transform = inventoryAnchor.transform;
		menu.menu = new UIMenuWrapper(iac);
		menu.OnEnterEvent += () =>
		{
			iac.inventoryHasFocus = true;
			iac.Refresh();
		};

		var craftingAnchor = new GameObject("Crafting UI Anchor");
		craftingAnchor.transform.parent = iac.transform;
		craftingAnchor.transform.localPosition = new Vector3(1065, 1270, 0);
		craftingAnchor.transform.localRotation = Quaternion.identity;
		craftingAnchor.transform.localScale = new Vector3(1.1f, 2.35f, 1);

		var craftingObject = gameObject.GetComponentInChildren<RecipePageUI>().gameObject;
		var crafting = new CraftingInteractableUi(craftingObject);
		crafting.transform = craftingAnchor.transform;
		crafting.menu = new UIMenuWrapper(iac);
		crafting.OnEnterEvent += () =>
		{
			iac.inventoryHasFocus = false;
			iac.Refresh();
		};

		return true;
	}
}
