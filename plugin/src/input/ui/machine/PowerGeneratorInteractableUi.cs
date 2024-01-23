using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Input.Ui.Machine;

public class PowerGeneratorInteractableUi : InteractableUi
{
	private static PluginLogger Logger = PluginLogger.GetLogger<PowerGeneratorInteractableUi>();

	private GeneratorUI generator;
	public PowerGeneratorInteractableUi(GameObject gameObject) : base(gameObject)
	{
		generator = gameObject.GetComponentInParent<GeneratorUI>();

		var button = GameObjectFinder.FindChildObjectByName("Controller Activate Crank CG", gameObject);
		var rectTransform = button.GetComponent<RectTransform>();
		var rect = rectTransform.rect;

		Logger.LogDebug($"Button rect: {rect} {rectTransform.localPosition}");
		rect.x += rectTransform.localPosition.x;
		rect.y = 260; // TODO: Find a way to get this value dynamically

		interactable = [
			new InteractableBuilder(this, rect, button)
				.withClick((ui) => onClick())
				.build()
		];
	}

	private void onClick()
	{
		generator.ActivateCrankOnClick();
	}
}
