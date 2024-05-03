using System.Linq;
using PiUtils.Util;
using PiVrLoader.Input.Ui;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class BlastSmelterInteractableUi : InventoryInteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<BlastSmelterInteractableUi>();

	public BlastSmelterInteractableUi(GameObject gameObject) : base(gameObject)
	{
		zIndex = 0.001f;

		getInteractables = () => gameObject.GetComponentsInChildren<InventoryResourceSlotUI>().Select((slot) => getInteractable(slot)).ToList();
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
