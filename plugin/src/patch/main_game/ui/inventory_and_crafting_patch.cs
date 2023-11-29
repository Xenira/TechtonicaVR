using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

public class InventoryAndCraftingPatch : GameObjectPatch
{
    public InventoryAndCraftingPatch() : base("Inventory and Crafting Menus")
    {
    }

    protected override bool Apply(GameObject gameObject)
    {
        gameObject.transform.localScale = new Vector3(0.7f, 0.6f, 0.7f);
        return true;
    }
}