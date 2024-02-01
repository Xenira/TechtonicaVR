using System.Linq;
using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui;

public class ToolbarInteractableUi : InteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<ToolbarInteractableUi>();
	private ResourceInfo draggedResourceInfo;

	public ToolbarInteractableUi(GameObject gameObject) : base(gameObject)
	{
		var uiSlots = gameObject.GetComponentsInChildren<ToolbarSlotUI>();
		interactable = uiSlots.Select(getInteractable).ToList();
	}

	private Interactable getInteractable(ToolbarSlotUI uiSlot)
	{
		var rectTransform = uiSlot.GetComponent<RectTransform>();
		var rect = rectTransform.rect;
		rect.x += rectTransform.localPosition.x;
		rect.y += rectTransform.localPosition.y;
		return new InteractableBuilder(this, rect, rectTransform.gameObject)
			.withClick((ui) => onClick(uiSlot))
			.withDrag(() => draggedResourceInfo ?? getSlotResource(uiSlot),
				(ui) => onDrag(uiSlot),
				(ui, source, target) => onDrop(ui, target, uiSlot),
				(ui) => onCancelDrag(ui, uiSlot))
			.withDrop(onAcceptsDrop, (ui, source) => onReceiveDrop(source, uiSlot))
			.build();
	}


	private void onClick(ToolbarSlotUI uiSlot)
	{
		Logger.LogDebug($"Clicked toolbar slot {uiSlot.slotIndex} {uiSlot.primaryToolbar}");
		selectSlot(uiSlot);
	}

	private void onDrag(ToolbarSlotUI uiSlot)
	{
		Logger.LogDebug($"Dragged toolbar slot {uiSlot.slotIndex} {uiSlot.primaryToolbar}");
		draggedResourceInfo = getSlotResource(uiSlot);
		setSlot(uiSlot, null);
	}

	private void onDrop(InteractableUi ui, Interactable target, ToolbarSlotUI sourceSlot)
	{
		Logger.LogDebug($"Dropped toolbar slot {sourceSlot.slotIndex} {sourceSlot.primaryToolbar}");
		if (target == null)
		{
			setSlot(sourceSlot, null);
			draggedResourceInfo = null;
			return;
		}

		ResourceInfo targetResourceInfo = null;
		if (ui is ToolbarInteractableUi)
		{
			var targetSlot = target.gameObject.GetComponent<ToolbarSlotUI>();
			if (targetSlot == null)
			{
				Logger.LogDebug($"Target {target.gameObject.name} is not a ToolbarSlotUI");
				return;
			}

			targetResourceInfo = getSlotResource(targetSlot);
		}

		setSlot(sourceSlot, targetResourceInfo);
		target.receiveDrop(ui, draggedResourceInfo);
		draggedResourceInfo = null;
	}

	private void onCancelDrag(InteractableUi ui, ToolbarSlotUI uiSlot)
	{
		setSlot(uiSlot, draggedResourceInfo);
		draggedResourceInfo = null;
	}

	private void onAcceptsDrop(AcceptDropEventArgs args)
	{
		var resourceInfo = args.source?.GetObjectCallback() as ResourceInfo;
		if (resourceInfo == null)
		{
			return;
		}

		args.accept |= Player.instance.toolbar.ToolbarFilter(resourceInfo);
	}

	private void onReceiveDrop(object sourceObject, ToolbarSlotUI slot)
	{
		var resourceInfo = sourceObject as ResourceInfo;
		if (resourceInfo == null)
		{
			Logger.LogError($"Received drop of {sourceObject} which is not a ResourceInfo");
			return;
		}

		setSlot(slot, resourceInfo);
		selectSlot(slot);
	}

	private void setSlot(ToolbarSlotUI uiSlot, ResourceInfo resourceInfo)
	{
		Player.instance.toolbar.SetToolbarKey(uiSlot.slotIndex, getToolbarIndex(uiSlot), resourceInfo, false);
		Player.instance.toolbar.Refresh();
	}

	private void selectSlot(ToolbarSlotUI uiSlot)
	{
		Player.instance.toolbar._primaryToolbarSelected = uiSlot.primaryToolbar;
		Player.instance.toolbar.SelectHotBarSlot(uiSlot.slotIndex);
	}

	private int getToolbarIndex(ToolbarSlotUI slot)
	{
		return slot.primaryToolbar ? Player.instance.toolbar.selectedToolbar : Player.instance.toolbar.secondaryToolbar;
	}

	private ResourceInfo getSlotResource(ToolbarSlotUI slot)
	{
		return slot.setResource;
	}
}
