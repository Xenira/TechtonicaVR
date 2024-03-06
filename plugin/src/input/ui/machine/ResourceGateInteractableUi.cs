using System.Linq;
using PiUtils.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class ResourceGateInteractableUi : InventoryInteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<ResourceGateInteractableUi>();

	public ResourceGateInteractableUi(GameObject gameObject) : base(gameObject)
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

	private void onClick(ProductionTerminalUpgradeButton upgradeButton)
	{
		upgradeButton.mouseLeftClickCallback?.Invoke();
	}
}
