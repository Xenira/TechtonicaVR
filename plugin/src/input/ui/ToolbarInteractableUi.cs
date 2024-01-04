using System.Linq;
using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui;

public class ToolbarInteractableUI : InteractableUi
{
	private static Logger<ToolbarInteractableUI> Logger = new Logger<ToolbarInteractableUI>();

	new protected void Start()
	{
		base.Start();

		var uiSlots = gameObject.GetComponentsInChildren<ToolbarSlotUI>();
		interactable = uiSlots.Select(getInteractable).ToList();
	}

	private Interactable getInteractable(ToolbarSlotUI uiSlot)
	{
		var rectTransform = uiSlot.GetComponent<RectTransform>();
		var rect = rectTransform.rect;
		rect.x += rectTransform.localPosition.x;
		rect.y += rectTransform.localPosition.y;
		return new Interactable(rect, (ui) => onClick(ui, uiSlot));
	}

	private void onClick(InteractableUi ui, ToolbarSlotUI uiSlot)
	{
		Logger.LogDebug($"Clicked toolbar slot {uiSlot.slotIndex} {uiSlot.primaryToolbar}");
		Player.instance.toolbar._primaryToolbarSelected = uiSlot.primaryToolbar;
		Player.instance.toolbar.SelectHotBarSlot(uiSlot.slotIndex);
	}
}
