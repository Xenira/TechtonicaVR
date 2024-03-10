using System.Linq;
using PiUtils.Patches;
using PiUtils.Util;
using TechtonicaVR.Patches.LoadingScreen;
using UnityEngine;

namespace TechtonicaVR;

public class LoadingScreenPatch : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<LoadingScreenPatch>();

	IPatch[] patches = [
			new LoadingScreenCanvasPatch(),
		];

	private float startTime = Time.time;

	public static LoadingScreenPatch Create()
	{
		var instance = new GameObject(nameof(LoadingScreenPatch)).AddComponent<LoadingScreenPatch>();

		return instance;
	}

	void Start()
	{
		Logger.LogDebug("Hello Loading Screen!");
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
