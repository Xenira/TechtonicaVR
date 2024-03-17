using System.Linq;
using PiUtils.Objects.Behaviours;
using PiUtils.Util;
using TechtonicaVR.Input;
using TechtonicaVR.VRCamera;
using UnityEngine;
using Valve.VR;

namespace TechtonicaVR.VrPlayer;

public class LightController
{
	private static PluginLogger Logger = PluginLogger.GetLogger<LightController>();
	private Headlamp[] headlamps;
	private GameObject backpackLight;
	private Interactable headlampInteractable;
	private Interactable backpackInteractable;

	public LightController(Headlamp[] headlamps)
	{

		SteamVRInputMapper.Use.ButtonReleased += OnUseButtonReleased;

		this.headlamps = headlamps.Where(h => h.enabled).ToArray();

		headlampInteractable = AddHeadlampInteractable();
	}

	public void SetBackpackLight(GameObject light)
	{
		Logger.LogDebug("Setting backpack light...");
		backpackLight = light;

		var interactableObject = new GameObject("BackpackLightInteractable");
		interactableObject.transform.SetParent(light.transform.parent, false);
		interactableObject.transform.localPosition = new Vector3(0.1837f, 0.9103f, 0.4041f);
		interactableObject.transform.localRotation = Quaternion.Euler(337.1403f, 3.5846f, 277.2928f);

		backpackInteractable = interactableObject.AddComponent<Interactable>();
		backpackInteractable.interactionTransform = VRCameraManager.rightHandObject.transform;
		backpackInteractable.OnEnter += () => SteamVRInputMapper.PlayVibration(SteamVR_Input_Sources.RightHand, 0.3f);

		headlamps?.ForEach(lamp => lamp.TurnOff());
	}

	private void OnUseButtonReleased(object sender, SteamVR_Input_Sources source)
	{
		if (headlampInteractable?.isHovered == true)
		{
			ToggleHeadlamps();
			return;
		}

		if (backpackInteractable?.isHovered == true)
		{
			ToggleBackpackLight();
		}
	}

	public void ToggleHeadlamps(bool group = true)
	{
		Logger.LogDebug("Toggling headlamp...");

		headlamps?.ForEach(lamp => lamp.ToggleLight());
	}

	public void ToggleBackpackLight(bool group = true)
	{
		Logger.LogDebug("Toggling backpack light...");

		backpackLight?.SetActive(!backpackLight.activeSelf);
	}

	private Interactable AddHeadlampInteractable()
	{
		Logger.LogDebug("Adding headlamp interactable...");
		if (headlamps == null)
		{
			Logger.LogDebug("No headlamp found");
			return null;
		}

		var firstHeadlamp = headlamps.FirstOrDefault();
		if (firstHeadlamp == null)
		{
			Logger.LogDebug("First headlamp not found");
			return null;
		}

		var interactableObject = new GameObject("HeadlampInteractable");
		interactableObject.transform.SetParent(firstHeadlamp.transform.parent, false);
		interactableObject.transform.localPosition = new Vector3(-0.0324f, 0.1657f, -0.0179f);
		interactableObject.transform.localRotation = Quaternion.Euler(336.0304f, 355.1733f, 353.0185f);

		var interactable = interactableObject.gameObject.AddComponent<Interactable>();
		interactable.interactionTransform = VRCameraManager.rightHandObject.transform;
		interactable.OnEnter += () => SteamVRInputMapper.PlayVibration(SteamVR_Input_Sources.RightHand, 0.3f);

		return interactable;
	}
}
