using System.Linq;
using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class ProductionTerminalInteractableUi : InventoryInteractableUI
{
	private static PluginLogger Logger = PluginLogger.GetLogger<ProductionTerminalInteractableUi>();

	public ProductionTerminalInteractableUi(GameObject gameObject) : base(gameObject)
	{
		zIndex = 0.001f;

		interactable = gameObject.GetComponentsInChildren<InventoryResourceSlotUI>().Select(getInteractable).ToList();

		var upgradeButton = gameObject.GetComponentInChildren<ProductionTerminalUpgradeButton>();
		var upgradeButtonInteractable = new InteractableBuilder(this, getRect(upgradeButton.GetComponent<RectTransform>()), upgradeButton.gameObject)
			.withClick((ui) => onClick(upgradeButton), () => upgradeButton.canUpgrade)
			.withHoverEnter((ui) => upgradeButton.mouseEnterCallback?.Invoke())
			.withHoverExit((ui) => upgradeButton.mouseExitCallback?.Invoke())
			.build();
		interactable.Add(upgradeButtonInteractable);
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
	private void onClick(ProductionTerminalUpgradeButton upgradeButton)
	{
		upgradeButton.mouseLeftClickCallback?.Invoke();
	}
}
