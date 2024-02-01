using System.Collections.Generic;
using System.Linq;
using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui;

public class CraftingInteractableUi : InteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<CraftingInteractableUi>();

	private RecipePageUI recipePage;
	private Transform tabsTransform;

	public CraftingInteractableUi(GameObject gameObject) : base(gameObject)
	{
		tabsTransform = GameObjectFinder.FindChildObjectByName("Tabs", gameObject).transform;
		recipePage = gameObject.GetComponent<RecipePageUI>();
		getInteractables = () => getTabs().Concat(getRecipes()).ToList();
	}

	private IEnumerable<Interactable> getTabs()
	{
		for (var i = 0; i < tabsTransform.childCount; i++)
		{
			var tab = tabsTransform.GetChild(i).gameObject;
			yield return new InteractableBuilder(this, getRect(tab.GetComponent<RectTransform>()), tab)
				.withClick((_ui) => onTabClick(tab))
				.withHoverEnter((_ui) => onTabHoverEnter(tab))
				.withHoverExit((_ui) => onTabHoverExit(tab))
				.build();
		}
	}

	protected void onTabHoverEnter(GameObject tab)
	{
		var mouseInteraction = tab.GetComponent<MouseInteraction>();
		if (mouseInteraction == null)
		{
			return;
		}
		mouseInteraction.mouseEnterCallback.Invoke();
	}

	protected void onTabHoverExit(GameObject tab)
	{
		var mouseInteraction = tab.GetComponent<MouseInteraction>();
		if (mouseInteraction == null)
		{
			return;
		}
		mouseInteraction.mouseExitCallback.Invoke();
	}

	private void onTabClick(GameObject tab)
	{
		var mouseInteraction = tab.GetComponent<MouseInteraction>();
		if (mouseInteraction == null)
		{
			return;
		}
		mouseInteraction.mouseLeftClickCallback.Invoke();
		rebuildInteractables();
	}

	private IEnumerable<Interactable> getRecipes()
	{
		foreach (var recipe in recipePage.gameObject.GetComponentsInChildren<RecipeSlotUI>())
		{
			yield return new InteractableBuilder(this, getRect(recipe.GetComponent<RectTransform>()), recipe.gameObject)
				.withClick((_ui) => onRecipeClick(recipe))
				.withHoverEnter((_ui) => onRecipeHoverEnter(recipe))
				.withHoverExit((_ui) => onRecipeHoverExit(recipe))
				.build();
		}
	}

	protected void onRecipeHoverEnter(RecipeSlotUI slot)
	{
		slot.mouseEnterCallback.Invoke();
	}

	protected void onRecipeHoverExit(RecipeSlotUI slot)
	{
		slot.mouseExitCallback.Invoke();
	}

	private void onRecipeClick(RecipeSlotUI slot)
	{
		slot.mouseLeftClickCallback.Invoke();
	}
}
