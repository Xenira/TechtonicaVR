using System.Linq;
using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class AssemblerInteractableUi : InventoryInteractableUI
{
	private static PluginLogger Logger = PluginLogger.GetLogger<AssemblerInteractableUi>();

	public AssemblerInteractableUi(GameObject gameObject) : base(gameObject)
	{
		zIndex = 0.001f;

		interactable = GameObjectFinder.FindChildObjectByName("Container", gameObject).GetComponentsInChildren<InventoryResourceSlotUI>().Select(getInteractable).ToList();

		Logger.LogDebug($"Interactable: {rectTransform.rect}");
	}



	protected override void init()
	{
	}

	private Interactable getInteractable(InventoryResourceSlotUI slot)
	{
		var rectTransform = slot.GetComponent<RectTransform>();
		var rect = getRect(rectTransform);
		Logger.LogDebug($"Slot rect: {slot} {rect} {rectTransform.localPosition}");

		return new InteractableBuilder(this, rect, rectTransform.gameObject)
			.withRecalculate(() => getRect(rectTransform))
			.withDrag(() => draggedResourceInfo ?? slot.resourceType,
				(ui) => onDrag(slot),
				(ui, source, target) => onDrop(ui, target, slot),
				(ui) => onCancelDrag(slot))
			.withDrop(onAcceptsDrop, (ui, source) => onReceiveDrop(source, slot))
			.withHoverEnter((ui) => onHoverEnter(slot))
			.withHoverExit((ui) => onHoverExit(slot))
			.build();
	}
}

public class AssemblerRecipeSelectInteractableUi : InventoryInteractableUI
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
