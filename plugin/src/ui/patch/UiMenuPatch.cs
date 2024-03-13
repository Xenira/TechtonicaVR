using HarmonyLib;
using PiUtils.Debug;
using PiUtils.Util;
using TechtonicaVR.Input.Ui;
using TechtonicaVR.Input.Ui.Machine;
using TechtonicaVR.VRCamera;
using UnityEngine;
using UnityEngine.UI;

namespace TechtonicaVR.UI.Patch;

[HarmonyPatch]
public class UIMenuPatch
{
	private static PluginLogger Logger = PluginLogger.GetLogger<UIMenuPatch>();

	private static GameObject worldAnchor;

	private static Vector3 lastCamOrigin = Vector3.zero;
	private static Vector3 lastPosition = Vector3.zero;

	[HarmonyPatch(typeof(UIMenu), nameof(UIMenu.Start))]
	[HarmonyPostfix]
	public static void StartPostfix(UIMenu __instance)
	{
		Logger.LogDebug($"Attaching world position behaviour to: {__instance.name}");

		GameObject tlc = findTlc(__instance.gameObject);
		var blurs = GameObjectFinder.FindChildObjectsByName("BG Blur", __instance.transform.root.gameObject);

		foreach (var blur in blurs)
		{
			destroyBlur(blur);
		}

		var canvas = tlc.GetComponentInParent<Canvas>();
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.gameObject.layer = 0;
		canvas.transform.SetParent(getWorldAnchor().transform);
		canvas.transform.localPosition = Vector3.zero;
		canvas.transform.localRotation = Quaternion.identity;

		if (__instance is InventoryAndCraftingUI)
		{
			canvas.transform.localScale = ModConfig.inventoryAndCraftingMenuScaleOverride.Value;
		}
		else
		{
			canvas.GetComponent<CanvasScaler>().enabled = false;
			canvas.transform.localScale = ModConfig.menuScale.Value;
		}

		AsyncGameObject.TimeoutFrames(() => addInteractableUi(__instance), 1);
		disableButtonPrompts(__instance);
	}

	private static void addInteractableUi(UIMenu instance)
	{
		if (instance is GeneratorUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Content Container", instance.gameObject);
			new PowerGeneratorInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is SmelterUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Container", instance.gameObject);
			new SmelterInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is DrillUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Container", instance.gameObject);
			new DrillInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is ThresherUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Container", instance.gameObject);
			new ThresherInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is AssemblerUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Container", instance.gameObject);
			var sidebar = GameObjectFinder.FindChildObjectByName("visible sidebar area", instance.gameObject);
			new AssemblerInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
			new AssemblerRecipeSelectInteractableUi(sidebar)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is FilterInserterUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Filter Options Container", instance.gameObject);
			new FilterInserterInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is PlanterUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Content Container", instance.gameObject);
			new PlanterInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is ResourceGateUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Container", instance.gameObject);
			new ResourceGateInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is ProductionTerminalMenu)
		{
			var container = GameObjectFinder.FindChildObjectByName("Container", instance.gameObject);
			new ProductionTerminalInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is TransitDepotUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Transit Depot Container ", instance.gameObject);
			new TransitDepotInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is TechTreeUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Main Tech Tree Container", instance.gameObject);
			new TechTreeInteractableUi(instance as TechTreeUI, container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is RepairableElevatorUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Container", instance.gameObject);
			new RepairableElevatorInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
		else if (instance is BlastSmelterUI)
		{
			var container = GameObjectFinder.FindChildObjectByName("Container", instance.gameObject);
			new BlastSmelterInteractableUi(container)
			{
				menu = new UIMenuWrapper(instance)
			};
		}
	}

	private static void disableButtonPrompts(UIMenu __instance)
	{
		var mkbButtonPrompts = __instance.gameObject.GetComponentsInChildren<MKBInventoryButtonPrompts>();
		foreach (var buttonPrompt in mkbButtonPrompts)
		{
			buttonPrompt.gameObject.SetActive(false);
		}

		var controllerButtonPrompts = __instance.gameObject.GetComponentsInChildren<ControllerInventoryButtonPrompts>();
		foreach (var buttonPrompt in controllerButtonPrompts)
		{
			buttonPrompt.gameObject.SetActive(false);
		}

		var simpleMkbButtonPrompts = GameObjectFinder.FindChildObjectsByName("MKB Inventory Button Prompts", __instance.gameObject);
		foreach (var buttonPrompt in simpleMkbButtonPrompts)
		{
			buttonPrompt.gameObject.SetActive(false);
		}

		var simpleControllerButtonPrompts = GameObjectFinder.FindChildObjectsByName("Controller Button Prompts", __instance.gameObject);
		foreach (var buttonPrompt in simpleControllerButtonPrompts)
		{
			buttonPrompt.gameObject.SetActive(false);
		}

		var simpleInventoryButtonPrompts = GameObjectFinder.FindChildObjectsByName("Inventory Button Prompts", __instance.gameObject);
		foreach (var buttonPrompt in simpleInventoryButtonPrompts)
		{
			buttonPrompt.gameObject.SetActive(false);
		}
	}

	private static GameObject findTlc(GameObject gameObject)
	{
		return GameObjectFinder.FindChildObjectByName("Top Level Container", gameObject) ?? GameObjectFinder.FindParentObjectByName("Top Level Container", gameObject) ?? gameObject;
	}

	[HarmonyPatch(typeof(UIMenu), nameof(UIMenu.OnOpen))]
	[HarmonyPostfix]
	public static void OnOpenPostfix(UIMenu __instance)
	{
		Logger.LogInfo($"UIMenu.OnOpenPostfix: {__instance.name}");

		// Do not update if menu is part of carousel
		if (UIManager._instance.carouselUI.isOpen && lastPosition != Vector3.zero)
		{
			return;
		}

		lastPosition = VRCameraManager.mainCamera.transform.position + VRCameraManager.mainCamera.transform.forward * ModConfig.menuSpawnDistance.Value + Vector3.down * ModConfig.menuDownwardOffset.Value;
		lastCamOrigin = VRCameraManager.mainCamera.transform.position;

		setWorldAnchor(lastPosition, lastCamOrigin);
	}

	[HarmonyPatch(typeof(UIMenu), nameof(UIMenu.OnClose))]
	[HarmonyPostfix]
	public static void OnClosePostfix(UIMenu __instance)
	{
		Logger.LogInfo($"UIMenu.OnClosePostfix: {__instance.name}");

		if (UIManager._instance.carouselUI.isOpen)
		{
			return;
		}

		lastPosition = Vector3.zero;
		lastCamOrigin = Vector3.zero;

		getWorldAnchor().transform.position = Vector3.zero;
	}

	[HarmonyPatch(typeof(FHG_Utils), nameof(FHG_Utils.ToggleAlpha))]
	[HarmonyPrefix]
	public static bool ToggleAlphaPrefix(CanvasGroup me, bool isOn)
	{
		return me != null;
	}

	[HarmonyPatch(typeof(PlayerInventoryUI), nameof(PlayerInventoryUI.Start))]
	[HarmonyPostfix]
	public static void PlayerInventoryUIStartPostfix(PlayerInventoryUI __instance)
	{
		Logger.LogDebug($"Attaching world position behaviour to: {__instance.name}");
		var canvas = __instance.myCanvas;
		canvas.gameObject.layer = 0;
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.transform.SetParent(getWorldAnchor().transform, true);
		canvas.transform.localPosition = Vector3.zero;
		canvas.transform.localRotation = Quaternion.identity;
		canvas.transform.localScale = ModConfig.menuScale.Value;
	}

	[HarmonyPatch(typeof(RecipePickerUI), nameof(RecipePickerUI.Start))]
	[HarmonyPostfix]
	public static void RecipePickerUIStartPostfix(RecipePickerUI __instance)
	{
		Logger.LogDebug($"Attaching world position behaviour to: {__instance.name}");

		var blurs = GameObjectFinder.FindChildObjectsByName("BG Blur", __instance.transform.root.gameObject);

		foreach (var blur in blurs)
		{
			destroyBlur(blur);
		}

		var canvas = __instance.GetComponentInParent<Canvas>();
		canvas.gameObject.layer = 0;
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.transform.SetParent(getWorldAnchor().transform, true);
		canvas.transform.localPosition = Vector3.zero;
		canvas.transform.localScale = ModConfig.menuScale.Value;

		AsyncGameObject.TimeoutFrames(() =>
		{
			var container = GameObjectFinder.FindChildObjectByName("Container", __instance.gameObject);
			new RecipePickerInteractableUi(container)
			{
				menu = new RecipePickerMenuWrapper(__instance)
			};
		}, 10);
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

	private static GameObject getWorldAnchor()
	{
		if (worldAnchor == null)
		{
			worldAnchor = new GameObject("UI World Anchor");
			Object.DontDestroyOnLoad(worldAnchor);
			worldAnchor.AddComponent<Gizmo>();
		}

		return worldAnchor;
	}

	private static void setWorldAnchor(Vector3 position, Vector3 camOrigin)
	{
		var anchor = getWorldAnchor();
		anchor.transform.position = position;
		anchor.transform.LookAt(2 * anchor.transform.position - camOrigin);
	}
}
