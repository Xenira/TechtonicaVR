using System.Linq;
using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class PlanterInteractableUi : InventoryInteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<PlanterInteractableUi>();

	public PlanterInteractableUi(GameObject gameObject) : base(gameObject)
	{
		zIndex = 0.001f;

		interactable = gameObject.GetComponentsInChildren<InventoryResourceSlotUI>().Select(getInteractable).ToList();
	}

	protected override void init()
	{
	}
}
