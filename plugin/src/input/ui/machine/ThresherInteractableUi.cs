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
		var rect = getRect(rectTransform);

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
