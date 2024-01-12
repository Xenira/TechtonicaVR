using System.Linq;
using FluffyUnderware.DevTools.Extensions;
using TechtonicaVR.Util;
using UnityEngine;
using UnityEngine.UI;

namespace TechtonicaVR.Input.Ui;

public class InventoryInteractableUI : InteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<InventoryInteractableUI>();
	private ResourceInfo draggedResourceInfo;
	private int draggedResourceCount;
	private Transform viewport;

	public InventoryInteractableUI(GameObject gameObject) : base(gameObject)
	{
		var inv = transform.gameObject.GetComponentInParent<InventoryGridUI>();
		interactable = inv.ui.slots.Select(getInteractable).ToList();
		scrollRect = inv.GetComponent<ScrollRect>();
	}

	private Interactable getInteractable(InventoryResourceSlotUI slot, int index)
	{
		var rectTransform = slot.GetComponent<RectTransform>();
		var rect = rectTransform.rect;
		viewport = rectTransform.gameObject.GetComponentInParent<CanvasRenderer>().transform;

		rect.x += rectTransform.localPosition.x;
		rect.y += rectTransform.localPosition.y;

		return new InteractableBuilder(this, rect, rectTransform.gameObject)
			// .withClick((ui) => onClick(uiSlot))
			.withIsHit((point) => rect.Contains(point - rectTransform.parent.parent.localPosition.ToVector2() - rectTransform.parent.parent.parent.localPosition.ToVector2()))
			.withDrag(() => draggedResourceInfo ?? slot.resourceType,
				(ui) => onDrag(slot),
				(ui, source, target) => onDrop(ui, target, slot),
				(ui) => onCancelDrag(slot))
			.withDrop(onAcceptsDrop, (ui, source) => onReceiveDrop(source, slot))
			.withHoverEnter((ui) => onHoverEnter(slot))
			.withHoverExit((ui) => onHoverExit(slot))
			.build();
	}

	private void onHoverEnter(InventoryResourceSlotUI slot)
	{
		slot.mouseEnterCallback.Invoke();
	}

	private void onHoverExit(InventoryResourceSlotUI slot)
	{
		slot.mouseExitCallback.Invoke();
	}

	private void onClick(InventoryResourceSlotUI slot)
	{
		// uiSlot.mouseLeftClickCallback.Invoke();
	}

	private void onDrag(InventoryResourceSlotUI slot)
	{
		Logger.LogDebug($"Dragged toolbar slot {slot.resourceType?.name}");
		draggedResourceInfo = slot.resourceType;
		draggedResourceCount = slot.resourceQuantity;
		slot.mouseLeftClickCallback();
	}

	private void onDrop(InteractableUi ui, Interactable target, InventoryResourceSlotUI sourceSlot)
	{
		Logger.LogDebug($"Dropped toolbar slot {sourceSlot.resourceType?.name}");
		var droppedResourceInfo = draggedResourceInfo;
		draggedResourceInfo = null;

		if (target == null)
		{
			sourceSlot.mouseLeftClickCallback.Invoke();
			return;
		}

		if (ui is InventoryInteractableUI)
		{
			var targetSlot = target.gameObject.GetComponent<InventoryResourceSlotUI>();
			if (targetSlot == null)
			{
				Logger.LogDebug($"Target {target.gameObject.name} is not a ToolbarSlotUI");
				return;
			}

			targetSlot.mouseLeftClickCallback.Invoke();
			return;
		}

		if (ui is ToolbarInteractableUI)
		{
			target.receiveDrop(target.ui, droppedResourceInfo);
			sourceSlot.mouseLeftClickCallback.Invoke();
			return;
		}

		onCancelDrag(sourceSlot);
	}

	private void onCancelDrag(InventoryResourceSlotUI slot)
	{
		slot.mouseLeftClickCallback.Invoke();
		draggedResourceInfo = null;
	}

	private void onAcceptsDrop(AcceptDropEventArgs args)
	{
		if (args.source.ui is not InventoryInteractableUI)
		{
			return;
		}

		var resourceInfo = args.source?.GetObjectCallback() as ResourceInfo;
		if (resourceInfo == null)
		{
			return;
		}

		args.accept |= true;
	}

	private void onReceiveDrop(object sourceObject, InventoryResourceSlotUI slot)
	{
		var resourceInfo = sourceObject as ResourceInfo;
		if (resourceInfo == null)
		{
			Logger.LogError($"Received drop of {sourceObject} which is not a ResourceInfo");
			return;
		}
	}
}
