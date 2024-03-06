using System.Linq;
using PiUtils.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui;

public class InventoryInteractableUi : InteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<InventoryInteractableUi>();
	protected ResourceInfo draggedResourceInfo;
	private int draggedResourceCount;

	public InventoryInteractableUi(GameObject gameObject) : base(gameObject)
	{
		init();
	}

	protected virtual void init()
	{
		var inv = transform.gameObject.GetComponentInParent<InventoryGridUI>();
		getInteractables = () => inv.ui.slots.Select(getInteractable).ToList();
	}

	protected Interactable getInteractable(InventoryResourceSlotUI slot, int index)
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

	protected void onHoverEnter(InventoryResourceSlotUI slot)
	{
		slot.mouseEnterCallback?.Invoke();
	}

	protected void onHoverExit(InventoryResourceSlotUI slot)
	{
		slot.mouseExitCallback?.Invoke();
	}

	private void onClick(InventoryResourceSlotUI slot)
	{
		// uiSlot.mouseLeftClickCallback.Invoke();
	}

	protected void onDrag(InventoryResourceSlotUI slot)
	{
		Logger.LogDebug($"Dragged toolbar slot {slot.resourceType?.name}");
		draggedResourceInfo = slot.resourceType;
		draggedResourceCount = slot.resourceQuantity;
		slot.mouseLeftClickCallback();
	}

	protected void onDrop(InteractableUi ui, Interactable target, InventoryResourceSlotUI sourceSlot)
	{
		Logger.LogDebug($"Dropped inventory slot {sourceSlot.resourceType?.name}");
		var droppedResourceInfo = draggedResourceInfo;
		draggedResourceInfo = null;

		if (target == null)
		{
			sourceSlot.mouseLeftClickCallback.Invoke();
			return;
		}

		if (ui is InventoryInteractableUi)
		{
			var targetSlot = target.gameObject.GetComponent<InventoryResourceSlotUI>();
			if (targetSlot == null)
			{
				Logger.LogDebug($"Target {target.gameObject.name} is not a ToolbarSlotUI");
				return;
			}

			targetSlot.mouseLeftClickCallback.Invoke();
			MouseCursorBuffer.instance.TryCancel();
			return;
		}

		if (ui is ToolbarInteractableUi)
		{
			target.receiveDrop(target.ui, droppedResourceInfo);
			sourceSlot.mouseLeftClickCallback.Invoke();
			return;
		}

		onCancelDrag(sourceSlot);
	}

	protected void onCancelDrag(InventoryResourceSlotUI slot)
	{
		slot.mouseLeftClickCallback.Invoke();
		draggedResourceInfo = null;
	}

	protected void onAcceptsDrop(AcceptDropEventArgs args)
	{
		if (args.source.ui is not InventoryInteractableUi)
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

	protected void onReceiveDrop(object sourceObject, InventoryResourceSlotUI slot)
	{

		var resourceInfo = sourceObject as ResourceInfo;
		if (resourceInfo == null)
		{
			Logger.LogError($"Received drop of {sourceObject} which is not a ResourceInfo");
			return;
		}
	}
}
