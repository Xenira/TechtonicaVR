using System.Collections.Generic;
using HarmonyLib;
using TechtonicaVR.Util;
using TechtonicaVR.VRCamera;
using UnityEngine;

namespace TechtonicaVR.UI.Patch;

[HarmonyPatch]
public class UIMenuPatch
{
	private static Dictionary<UIMenu, WorldPositionedCanvas> cache = new Dictionary<UIMenu, WorldPositionedCanvas>();
	private static Vector3 lastCamOrigin = Vector3.zero;
	private static Vector3 lastPosition = Vector3.zero;

	[HarmonyPatch(typeof(UIMenu), nameof(UIMenu.Start))]
	[HarmonyPostfix]
	public static void StartPostfix(UIMenu __instance)
	{
		Plugin.Logger.LogDebug($"Attaching world position behaviour to: {__instance.name}");

		var tlc = GameObjectFinder.FindChildObjectByName("Top Level Container", __instance.gameObject) ?? GameObjectFinder.FindParentObjectByName("Top Level Container", __instance.gameObject) ?? __instance.gameObject;
		var blurs = GameObjectFinder.FindChildObjectsByName("BG Blur", __instance.transform.root.gameObject);

		foreach (var blur in blurs)
		{
			destroyBlur(blur);
		}

		tlc.transform.localScale = ModConfig.menuScale.Value;

		var tracked_menu = tlc.AddComponent<WorldPositionedCanvas>();
		tracked_menu.menu = __instance;

		cache[__instance] = tracked_menu;
	}

	[HarmonyPatch(typeof(UIMenu), nameof(UIMenu.OnOpen))]
	[HarmonyPostfix]
	public static void OnOpenPostfix(UIMenu __instance)
	{
		Plugin.Logger.LogInfo($"UIMenu.OnOpenPostfix: {__instance.name}");
		WorldPositionedCanvas tracked_menu;
		if (!cache.TryGetValue(__instance, out tracked_menu))
		{
			Plugin.Logger.LogError($"UIMenu.OnOpenPostfix: tracked_menu is null");
			return;
		}

		if (__instance.name == "Inventory and Crafting Menu" && ModConfig.inventoryAndCraftingMenuScaleOverride.Value != Vector3.zero)
		{
			tracked_menu.transform.localScale = ModConfig.inventoryAndCraftingMenuScaleOverride.Value;
		}
		else
		{
			tracked_menu.transform.localScale = ModConfig.menuScale.Value;
		}

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
		Plugin.Logger.LogInfo($"UIMenu.OnClosePostfix: {__instance.name}");

		WorldPositionedCanvas tracked_menu;
		if (!cache.TryGetValue(__instance, out tracked_menu))
		{
			Plugin.Logger.LogError($"UIMenu.OnClosePostfix: tracked_menu is null");
			return;
		}

		if (UIManager._instance.carouselUI.isOpen)
		{
			return;
		}

		tracked_menu.target = Vector3.zero;
		lastPosition = Vector3.zero;
		lastCamOrigin = Vector3.zero;
	}

	[HarmonyPatch(typeof(FHG_Utils), nameof(FHG_Utils.ToggleAlpha))]
	[HarmonyPrefix]
	public static bool ToggleAlphaPrefix(CanvasGroup me, bool isOn)
	{
		return me != null;
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
			Plugin.Logger.LogDebug($"Deregistering component disabler: {disableBehaviour.targetBehaviour}");
			UIManager._instance.DeregisterComponentDisabler(disableBehaviour.targetBehaviour);
		}

		Object.Destroy(blur);
	}
}
