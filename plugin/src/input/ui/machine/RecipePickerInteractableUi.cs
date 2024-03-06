using UnityEngine;
using System.Linq;
using PiUtils.Util;

namespace TechtonicaVR.Input.Ui.Machine;

public class RecipePickerInteractableUi : InteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<RecipePickerInteractableUi>();

	public RecipePickerInteractableUi(GameObject gameObject) : base(gameObject)
	{
		zIndex = -0.001f;
		interactable = gameObject.GetComponentsInChildren<ResourceSlotUI>().Select(getInteractable).ToList();
	}

	private Interactable getInteractable(ResourceSlotUI slot, int index)
	{
		Logger.LogDebug($"Slot: {slot} {slot.gameObject.name} {slot.gameObject.transform.localPosition}");
		var rectTransform = slot.GetComponent<RectTransform>();
		var rect = getRect(rectTransform);
		Logger.LogDebug($"Slot rect: {slot} {rect} {rectTransform.localPosition}");

		return new InteractableBuilder(this, rect, rectTransform.gameObject)
			.withRecalculate(() => getRect(rectTransform))
			.withClick((ui) => onClick(slot))
			.withHoverEnter((ui) => onHoverEnter(slot))
			.withHoverExit((ui) => onHoverExit(slot))
			.build();
	}

	private void onClick(ResourceSlotUI slot)
	{
		slot.mouseLeftClickCallback.Invoke();
	}

	private void onHoverEnter(ResourceSlotUI slot)
	{
		slot.mouseEnterCallback.Invoke();
	}

	private void onHoverExit(ResourceSlotUI slot)
	{
		slot.mouseExitCallback.Invoke();
	}
}
