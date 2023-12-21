using System.Linq;
using TechtonicaVR.Patches;
using TechtonicaVR.Patches.MainGame.Player;
using TechtonicaVR.Patches.MainGame.UI;
using TechtonicaVR.Patches.Universal;
using UnityEngine;

namespace TechtonicaVR;

public interface IPatch
{
	bool Apply();
	bool IsApplied();
}

public class MainGamePatch : MonoBehaviour
{

	private IPatch[] playerSpringPatches = [
			new LeftHandAttachPatch(),
				new RightHandAttachPatch(),
				new SetDefaultLayerPatch("Right Hand Attach", true),
				new SetDefaultLayerPatch("Left Hand Attach", true),
				new SetDefaultLayerPatch("Terrain Manipulator Black Hole", true),
				new SetDefaultLayerPatch("Terrain Manipulator Black Hole (Active)", true),
				new SetDefaultLayerPatch("Terrain Manipulator Black Hole (On Destroy)", true),
		];

	IPatch[] patches = [];

	private float startTime = Time.time;

	public static MainGamePatch Create()
	{
		var instance = new GameObject(nameof(MainGamePatch)).AddComponent<MainGamePatch>();

		return instance;
	}

	void Start()
	{
		patches = playerSpringPatches.Concat([
				new ExecuteAfterPatch(new DisableByNamePatch("Astronaut_LP_ArmsTorso"), playerSpringPatches),
						new NotificationCanvasPatche(),
						new ToolbarUiPatch(),
						new QuestTaskListPatch(),
						new DialoguePopupPatch(),
						new DisableComponentPatch<OutlinePostProcess>(),
						new CursorCanvasPatch(),
						new InspectorCanvasPatch(),
						new CompassPatch(),
						new MapPatch(),
						new CraftingQueuePatch(),
						new PlayerArrowPatch(),
				]).ToArray();

		Plugin.Logger.LogDebug("Hello World!");
	}

	void Update()
	{
		patches = patches.Where(p => !p.Apply()).ToArray();

		if (!patches.Any())
		{
			gameObject.SetActive(false);
		}
	}
}
