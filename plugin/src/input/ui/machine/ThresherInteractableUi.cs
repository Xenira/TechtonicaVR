using System.Linq;
using PiUtils.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class ThresherInteractableUi : InventoryInteractableUi
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
}
