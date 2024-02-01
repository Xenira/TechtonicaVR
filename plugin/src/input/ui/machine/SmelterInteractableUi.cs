using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class SmelterInteractableUi : InventoryInteractableUi
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
