using TechtonicaVR.Util;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace TechtonicaVR.Input.Ui.Machine;

public class TransitDepotInteractableUi : InventoryInteractableUI
{
	private static PluginLogger Logger = PluginLogger.GetLogger<TransitDepotInteractableUi>();

	public TransitDepotInteractableUi(GameObject gameObject) : base(gameObject)
	{
		getInteractables = () => gameObject.GetComponentsInChildren<TransitSystemResourceSlotUI>().Select(getInteractable).Inspect((i) =>
		{
			var scrollRect = i.gameObject.GetComponentInParent<ScrollRect>();
			if (scrollRect != null)
			{
				i.mask = scrollRect.viewport;
			}
		}).ToList();
	}

	protected override void init()
	{
	}
}
