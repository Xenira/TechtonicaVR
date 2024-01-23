using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class SmelterInteractableUi : InventoryInteractableUI
{
	private static PluginLogger Logger = PluginLogger.GetLogger<SmelterInteractableUi>();

	public SmelterInteractableUi(GameObject gameObject) : base(gameObject)
	{
		zIndex = 0.001f;

		var inputResourceSlot = GameObjectFinder.FindChildObjectByName("Input Resource Slot", gameObject)
			.GetComponent<InventoryResourceSlotUI>();
		var outputResourceSlot = GameObjectFinder.FindChildObjectByName("Output Resource Slot", gameObject)
			.GetComponent<InventoryResourceSlotUI>();
		var fuelResourceSlot = GameObjectFinder.FindChildObjectByName("Fuel Resource Slot", gameObject)
			.GetComponent<InventoryResourceSlotUI>();

		interactable = [
			getInteractable(inputResourceSlot),
			getInteractable(outputResourceSlot),
			getInteractable(fuelResourceSlot),
		];
		Logger.LogDebug($"Interactable: {rectTransform.rect}");
	}

	protected override void init()
	{
	}

	private Interactable getInteractable(InventoryResourceSlotUI slot)
	{
		var rectTransform = slot.GetComponent<RectTransform>();
		var rect = rectTransform.rect;

		var originalParent = this.rectTransform.parent;
		rectTransform.parent = this.rectTransform;
		var relativeLocalPosition = rectTransform.localPosition;
		rectTransform.parent = originalParent;

		rect.x += relativeLocalPosition.x;
		rect.y += relativeLocalPosition.y;
		Logger.LogDebug($"Output slot rect: {slot} {rect} {rectTransform.localPosition}");

		return new InteractableBuilder(this, rect, rectTransform.gameObject)
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
