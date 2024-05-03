using System.Linq;
using PiUtils.Util;
using PiVrLoader.Input.Ui;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class AssemblerInteractableUi : InventoryInteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<AssemblerInteractableUi>();

	public AssemblerInteractableUi(GameObject gameObject) : base(gameObject)
	{
		zIndex = 0.001f;

		interactable = GameObjectFinder.FindChildObjectByName("Container", gameObject).GetComponentsInChildren<InventoryResourceSlotUI>().Select(getInteractable).ToList();
	}

	protected override void init()
	{
	}
}

public class AssemblerRecipeSelectInteractableUi : InventoryInteractableUi
{
	public AssemblerRecipeSelectInteractableUi(GameObject gameObject) : base(gameObject)
	{
		var recipeSelector = gameObject.GetComponentInChildren<InventoryResourceSlotUI>();
		interactable = [
			new InteractableBuilder(this, getRect(recipeSelector.GetComponent<RectTransform>()), recipeSelector.gameObject)
				.withClick((ui) => onClick(recipeSelector))
				.withHoverEnter((ui) => onHoverEnter(recipeSelector))
				.withHoverExit((ui) => onHoverExit(recipeSelector))
				.build()
		];
	}

	protected override void init()
	{
	}

	private void onClick(InventoryResourceSlotUI recipeSelector)
	{
		recipeSelector.mouseLeftClickCallback.Invoke();
	}
}
