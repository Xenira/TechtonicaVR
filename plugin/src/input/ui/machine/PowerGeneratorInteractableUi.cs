using PiUtils.Util;
using PiVrLoader.Input.Ui;
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
		var rect = getRect(rectTransform);

		interactable = [
			new InteractableBuilder(this, rect, button)
				.withRecalculate(() => getRect(rectTransform))
				.withClick((ui) => onClick())
				.build()
		];
	}

	private void onClick()
	{
		generator.ActivateCrankOnClick();
	}
}
