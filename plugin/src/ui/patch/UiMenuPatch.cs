using System.Collections.Generic;
using HarmonyLib;
using TechtonicaVR.Util;
using TechtonicaVR.VRCamera;
using UnityEngine;

namespace TechtonicaVR.UI.Patch;

[HarmonyPatch]
public class UIMenuPatch
{
	private static PluginLogger Logger = PluginLogger.GetLogger<UIMenuPatch>();

	private static Dictionary<UIMenu, WorldPositionedCanvas> cache = new Dictionary<UIMenu, WorldPositionedCanvas>();
	private static WorldPositionedCanvas recipePickerUi;

	private static Vector3 lastCamOrigin = Vector3.zero;
	private static Vector3 lastPosition = Vector3.zero;

	[HarmonyPatch(typeof(UIMenu), nameof(UIMenu.Start))]
	[HarmonyPostfix]
	public static void StartPostfix(UIMenu __instance)
	{
		Logger.LogDebug($"Attaching world position behaviour to: {__instance.name}");

		var tlc = GameObjectFinder.FindChildObjectByName("Top Level Container", __instance.gameObject) ?? GameObjectFinder.FindParentObjectByName("Top Level Container", __instance.gameObject) ?? __instance.gameObject;
		var blurs = GameObjectFinder.FindChildObjectsByName("BG Blur", __instance.transform.root.gameObject);

		foreach (var blur in blurs)
		{
			destroyBlur(blur);
		}

		var tracked_menu = tlc.AddComponent<WorldPositionedCanvas>();
		tracked_menu.menu = new UIMenuWrapper(__instance);
		if (__instance.name == "Inventory and Crafting Menu" && ModConfig.inventoryAndCraftingMenuScaleOverride.Value != Vector3.zero)
		{
			tracked_menu.scale = ModConfig.inventoryAndCraftingMenuScaleOverride.Value;
		}
		else
		{
			tracked_menu.scale = ModConfig.menuScale.Value;
		}

		cache[__instance] = tracked_menu;
	}

	[HarmonyPatch(typeof(UIMenu), nameof(UIMenu.OnOpen))]
	[HarmonyPostfix]
	public static void OnOpenPostfix(UIMenu __instance)
	{
		Logger.LogInfo($"UIMenu.OnOpenPostfix: {__instance.name}");
		WorldPositionedCanvas tracked_menu;
		if (!cache.TryGetValue(__instance, out tracked_menu))
		{
			Logger.LogError($"UIMenu.OnOpenPostfix: tracked_menu is null");
			return;
		}

		tracked_menu.transform.localScale = tracked_menu.scale;

		// Do not update if menu is part of carousel
		if (UIManager._instance.carouselUI.isOpen && lastPosition != Vector3.zero)
		{
			tracked_menu.target = lastPosition;
			tracked_menu.camOrigin = lastCamOrigin;
			return;
		}

		lastPosition = VRCameraManager.mainCamera.transform.position + VRCameraManager.mainCamera.transform.forward * ModConfig.menuSpawnDistance.Value + Vector3.down * ModConfig.menuDownwardOffset.Value;
		lastCamOrigin = VRCameraManager.mainCamera.transform.position;

		tracked_menu.target = lastPosition;
		tracked_menu.camOrigin = lastCamOrigin;
	}

	[HarmonyPatch(typeof(UIMenu), nameof(UIMenu.OnClose))]
	[HarmonyPostfix]
	public static void OnClosePostfix(UIMenu __instance)
	{
		Logger.LogInfo($"UIMenu.OnClosePostfix: {__instance.name}");

		WorldPositionedCanvas tracked_menu;
		if (!cache.TryGetValue(__instance, out tracked_menu))
		{
			Logger.LogWarning($"UIMenu.OnClosePostfix: tracked_menu is null (Can be ignored on game start)");
			return;
		}

		if (UIManager._instance.carouselUI.isOpen)
		{
			return;
		}

		tracked_menu.target = Vector3.zero;
		tracked_menu.playerInventoryUI = null;
		lastPosition = Vector3.zero;
		lastCamOrigin = Vector3.zero;
	}

	[HarmonyPatch(typeof(FHG_Utils), nameof(FHG_Utils.ToggleAlpha))]
	[HarmonyPrefix]
	public static bool ToggleAlphaPrefix(CanvasGroup me, bool isOn)
	{
		return me != null;
	}

	[HarmonyPatch(typeof(PlayerInventoryUI), nameof(PlayerInventoryUI.Open))]
	[HarmonyPostfix]
	public static void PlayerInventoryUIOpenPostfix(PlayerInventoryUI __instance, RectTransform playerInventoryRefXfm, bool shouldCycle)
	{
		Logger.LogInfo($"PlayerInventoryUI.OpenPostfix: {__instance.name}");
		var worldPositionedCanvas = playerInventoryRefXfm.GetComponentInParent<WorldPositionedCanvas>();
		if (worldPositionedCanvas == null)
		{
			Logger.LogError($"PlayerInventoryUI.OpenPostfix: worldPositionedCanvas is null");
			return;
		}

		var tlc = __instance.gameObject.transform.GetChild(0);
		tlc.transform.localScale = worldPositionedCanvas.scale;

		worldPositionedCanvas.playerInventoryUI = __instance;
	}

	[HarmonyPatch(typeof(RecipePickerUI), nameof(RecipePickerUI.Start))]
	[HarmonyPostfix]
	public static void RecipePickerUIStartPostfix(RecipePickerUI __instance)
	{
		Logger.LogDebug($"Attaching world position behaviour to: {__instance.name}");

		var tlc = GameObjectFinder.FindChildObjectByName("Top Level Container", __instance.gameObject) ?? GameObjectFinder.FindParentObjectByName("Top Level Container", __instance.gameObject) ?? __instance.gameObject;
		var blurs = GameObjectFinder.FindChildObjectsByName("BG Blur", __instance.transform.root.gameObject);

		foreach (var blur in blurs)
		{
			destroyBlur(blur);
		}

		var tracked_menu = tlc.AddComponent<WorldPositionedCanvas>();
		tracked_menu.menu = new BehaviourMenu();

		if (__instance.name == "Inventory and Crafting Menu" && ModConfig.inventoryAndCraftingMenuScaleOverride.Value != Vector3.zero)
		{
			tracked_menu.scale = ModConfig.inventoryAndCraftingMenuScaleOverride.Value;
		}
		else
		{
			tracked_menu.scale = ModConfig.menuScale.Value;
		}

		recipePickerUi = tracked_menu;
	}

	[HarmonyPatch(typeof(RecipePickerUI), nameof(RecipePickerUI.Open))]
	[HarmonyPostfix]
	public static void RecipePickerUIOpenPostfix(RecipePickerUI __instance)
	{
		Logger.LogInfo($"RecipePickerUI.OpenPostfix: {__instance.name}");
		var worldPositionedCanvas = recipePickerUi;
		if (worldPositionedCanvas == null)
		{
			Logger.LogError($"RecipePickerUI.OpenPostfix: worldPositionedCanvas is null");
			return;
		}

		var tlc = __instance.gameObject.transform.GetChild(0);
		tlc.transform.localScale = worldPositionedCanvas.scale;

		worldPositionedCanvas.target = lastPosition;
		worldPositionedCanvas.camOrigin = lastCamOrigin;
		((BehaviourMenu)recipePickerUi.menu).open = true;
	}

	[HarmonyPatch(typeof(RecipePickerUI), nameof(RecipePickerUI.Close))]
	[HarmonyPostfix]
	public static void RecipePickerUIClosePostfix(RecipePickerUI __instance)
	{
		Logger.LogInfo($"RecipePickerUI.ClosePostfix: {__instance.name}");
		var worldPositionedCanvas = recipePickerUi;
		if (worldPositionedCanvas == null)
		{
			Logger.LogError($"RecipePickerUI.ClosePostfix: worldPositionedCanvas is null");
			return;
		}

		worldPositionedCanvas.target = Vector3.zero;
		worldPositionedCanvas.playerInventoryUI = null;
		((BehaviourMenu)recipePickerUi.menu).open = false;
	}

	private static void destroyBlur(GameObject blur)
	{
		if (blur == null)
		{
			return;
		}

		var disableBehaviour = blur.GetComponent<DisableWhenNoMenuOpen>();
		if (disableBehaviour != null)
		{
			Logger.LogDebug($"Deregistering component disabler: {disableBehaviour.targetBehaviour}");
			UIManager._instance.DeregisterComponentDisabler(disableBehaviour.targetBehaviour);
		}

		Object.Destroy(blur);
	}
}
