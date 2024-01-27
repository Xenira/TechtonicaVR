using System.Linq;
using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class ThresherInteractableUi : InventoryInteractableUI
{
	private static PluginLogger Logger = PluginLogger.GetLogger<ThresherInteractableUi>();

	public ThresherInteractableUi(GameObject gameObject) : base(gameObject)
	{
		zIndex = 0.001f;

		interactable = gameObject.GetComponentsInChildren<InventoryResourceSlotUI>().Select(getInteractable).ToList();
	}

	protected override void init()
	{
	}

	private Interactable getInteractable(InventoryResourceSlotUI slot)
	{
		var rectTransform = slot.GetComponent<RectTransform>();
		var rect = rectTransform.rect;

		var originalParent = rectTransform.parent;
		rectTransform.parent = this.rectTransform;
		var relativeLocalPosition = rectTransform.localPosition;
		rectTransform.parent = originalParent;

		rect.x += relativeLocalPosition.x;
		rect.y += relativeLocalPosition.y;
		Logger.LogDebug($"Slot rect: {slot} {rect} {rectTransform.localPosition}");

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
