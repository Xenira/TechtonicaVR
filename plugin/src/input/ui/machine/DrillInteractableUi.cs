using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class DrillInteractableUi : InventoryInteractableUI
{
	private static PluginLogger Logger = PluginLogger.GetLogger<DrillInteractableUi>();

	public DrillInteractableUi(GameObject gameObject) : base(gameObject)
	{
		zIndex = 0.001f;

		var outputResourceSlot = GameObjectFinder.FindChildObjectByName("Output Resource Slot", gameObject)
			.GetComponent<InventoryResourceSlotUI>();
		var fuelResourceSlot = GameObjectFinder.FindChildObjectByName("Fuel Resource Slot", gameObject)
			.GetComponent<InventoryResourceSlotUI>();

		interactable = [
			getInteractable(outputResourceSlot),
			getInteractable(fuelResourceSlot, new Vector2(0, 95)),
		];
		Logger.LogDebug($"Interactable: {rectTransform.rect}");
	}

	protected override void init()
	{
	}

	private Interactable getInteractable(InventoryResourceSlotUI slot, Vector2 offset = default)
	{
		var rectTransform = slot.GetComponent<RectTransform>();
		var rect = rectTransform.rect;

		var originalParent = rectTransform.parent;
		rectTransform.parent = this.rectTransform;
		var relativeLocalPosition = rectTransform.localPosition;
		rectTransform.parent = originalParent;

		rect.x += relativeLocalPosition.x + offset.x;
		rect.y += relativeLocalPosition.y + offset.y;
		Logger.LogDebug($"Slot rect: {slot} {rect} {relativeLocalPosition} {rectTransform.localPosition}");

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
