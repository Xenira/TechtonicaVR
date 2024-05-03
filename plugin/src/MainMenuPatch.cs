using System.Linq;
using PiUtils.Patches;
using PiUtils.Util;
using TechtonicaVR.Patches.MainMenu;
using UnityEngine;

namespace TechtonicaVR;

public class MainMenuPatch : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<MainMenuPatch>();

	IPatch[] patches = [
			new CameraOriginPatch(),
				new SpaceBGCameraPatch(),
				new AtLeastOnePatch([new MenuCanvasPatch1080(), new MenuCanvasPatch1440()]),
		];

	private float startTime = Time.time;

	public static MainMenuPatch Create()
	{
		var instance = new GameObject(nameof(MainMenuPatch)).AddComponent<MainMenuPatch>();

		return instance;
	}

	void Start()
	{
		Logger.LogDebug("Hello Main Menu!");
	}

	void Update()
	{
		if (!patches.Where(p => !p.Apply()).Any())
		{
			gameObject.SetActive(false);
		}
	}
}
