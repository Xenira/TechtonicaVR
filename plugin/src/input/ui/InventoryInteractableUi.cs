using System.Linq;
using PiUtils.Util;
using PiVrLoader.Input.Ui;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TechtonicaVR.Input.Ui;

public class InventoryInteractableUi : InteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<InventoryInteractableUi>();
	protected ResourceInfo draggedResourceInfo;
	private InventoryNavigator chestUi;
	private int draggedResourceCount;

	public InventoryInteractableUi(GameObject gameObject, InventoryNavigator chestUi = null) : base(gameObject)
	{
		this.chestUi = chestUi;
		init();
	}

	protected virtual void init()
	{
		var inv = transform.gameObject.GetComponentInChildren<InventoryGridUI>() ?? transform.gameObject.GetComponentInParent<InventoryGridUI>();
		getInteractables = () => inv.ui.slots.Select(getInteractable)
			.Concat(transform.gameObject.GetComponentsInChildren<UnityEngine.UI.Button>().Select(getButtonInteractable))
			.ToList();
	}

	protected Interactable getInteractable(InventoryResourceSlotUI slot, int index)
	{
		var rectTransform = slot.GetComponent<RectTransform>();
		var rect = getRect(rectTransform);

		return new InteractableBuilder(this, rect, rectTransform.gameObject)
			.withRecalculate(() => getRect(rectTransform))
			.withClick((_ui) => onClick(slot), () => isClickable())
			.withDrag(() => draggedResourceInfo ?? slot.resourceType,
				(_ui) => onDrag(slot),
				(ui, _source, target) => onDrop(ui, target, slot),
				(_ui) => onCancelDrag(slot))
			.withDrop(onAcceptsDrop, (_ui, source) => onReceiveDrop(source, slot))
			.withHoverEnter((_ui) => onHoverEnter(slot))
			.withHoverExit((_ui) => onHoverExit(slot))
			.build();
	}

	protected Interactable getButtonInteractable(UnityEngine.UI.Button button, int index)
	{
		var rectTransform = button.GetComponent<RectTransform>();
		var rect = getRect(rectTransform);

		return new InteractableBuilder(this, rect, rectTransform.gameObject)
			.withRecalculate(() => getRect(rectTransform))
			.withClick((_ui) => onButtonClick(button))
			.withHoverEnter((_ui) => onButtonHoverEnter(button))
			.withHoverExit((_ui) => onButtonHoverExit(button))
			.build();
	}

	protected void onHoverEnter(InventoryResourceSlotUI slot)
	{
		slot.mouseEnterCallback?.Invoke();
	}

	protected void onButtonHoverEnter(UnityEngine.UI.Button button)
	{
		button.OnPointerEnter(new PointerEventData(EventSystem.current));
	}

	protected void onHoverExit(InventoryResourceSlotUI slot)
	{
		slot.mouseExitCallback?.Invoke();
	}

	protected void onButtonHoverExit(UnityEngine.UI.Button button)
	{
		button.OnPointerExit(new PointerEventData(EventSystem.current));
	}

	private void onClick(InventoryResourceSlotUI slot)
	{
		if (!isSettingLimit())
		{
			return;
		}
		slot.mouseLeftClickCallback?.Invoke();
	}

	private bool isClickable()
	{
		return isSettingLimit();
	}

	private void onButtonClick(UnityEngine.UI.Button button)
	{
		button.onClick?.Invoke();
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

	private bool isSettingLimit()
	{
		if (chestUi == null)
		{
			return false;
		}

		return chestUi.settingLimit;
	}
}
