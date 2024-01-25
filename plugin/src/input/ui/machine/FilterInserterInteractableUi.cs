using TechtonicaVR.Util;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace TechtonicaVR.Input.Ui.Machine;

public class FilterInserterInteractableUi : InteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<FilterInserterInteractableUi>();

	public FilterInserterInteractableUi(GameObject gameObject) : base(gameObject)
	{
		interactable = gameObject.GetComponentsInChildren<ResourceSlotUI>().Select(getInteractable).ToList();
	}

	private Interactable getInteractable(ResourceSlotUI slot, int index)
	{
		var rectTransform = slot.GetComponent<RectTransform>();
		var rect = rectTransform.rect;

		var scrollView = slot.GetComponentInParent<ScrollRect>();
		if (scrollView != null)
		{
			// Get the scroll position
			var scrollPosition = new Vector2(scrollView.horizontalNormalizedPosition, scrollView.verticalNormalizedPosition);
			var scrollOffset = new Vector2(scrollPosition.x * scrollView.content.rect.width, scrollPosition.y * scrollView.content.rect.height);
			var scrollTransformation = rectTransform.InverseTransformPoint(scrollView.content.TransformPoint(scrollOffset));

			// Adjust the local position by the scroll position
			rect.x += scrollTransformation.x + rectTransform.localPosition.x;
			rect.y += scrollTransformation.y + rectTransform.localPosition.y;
		}

		var relativeLocalPosition = ObjectPosition.addLocalPositions(slot.transform, 4);

		rect.x += relativeLocalPosition.x;
		rect.y += relativeLocalPosition.y;
		Logger.LogDebug($"Slot rect: {slot} {rect} {relativeLocalPosition} {rectTransform.localPosition}");

		return new InteractableBuilder(this, rect, rectTransform.gameObject)
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
