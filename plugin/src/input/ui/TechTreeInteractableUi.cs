using System.Linq;
using UnityEngine;

namespace TechtonicaVR.Input.Ui;

public class TechTreeInteractableUi : InteractableUi
{
	private TechTreeUI techTreeUi;
	public TechTreeInteractableUi(TechTreeUI ui, GameObject gameObject) : base(gameObject)
	{
		techTreeUi = ui;
		getInteractables = () => gameObject.GetComponentsInChildren<TechTreeCategoryButton>()
			.Select(getCategoryButtonInteractable)
			.Concat(gameObject.GetComponentsInChildren<TechTreeNode>()
				.Select(getTechTreeNodeInteractable)
				.Where(interactable => interactable != null))
			.ToList();
	}

	private Interactable getTechTreeNodeInteractable(TechTreeNode node)
	{
		if (node.myUnlock.category != techTreeUi.gridUI.curCategory)
		{
			return null;
		}

		var rect = node.GetComponent<RectTransform>();
		return new InteractableBuilder(this, getRect(rect), node.gameObject)
			.withClick((ui) => node.mouseLeftClickCallback?.Invoke())
			.withMouseDown(() => node.mouseDownCallback?.Invoke())
			.withMouseUp(() => node.mouseUpCallback?.Invoke())
			.withHoverEnter((ui) => node.mouseEnterCallback?.Invoke())
			.withHoverExit((ui) => node.mouseExitCallback?.Invoke())
			.build();
	}

	private Interactable getCategoryButtonInteractable(TechTreeCategoryButton button)
	{
		var rect = button.GetComponent<RectTransform>();
		return new InteractableBuilder(this, getRect(rect), button.gameObject)
			.withClick((ui) =>
			{
				button.mouseLeftClickCallback?.Invoke();
				ui.refresh();
			})
			.withHoverEnter((ui) => button.mouseEnterCallback?.Invoke())
			.withHoverExit((ui) => button.mouseExitCallback?.Invoke())
			.build();
	}
}
