using PiUtils.Util;
using PiVrLoader.VRCamera;
using UnityEngine;

namespace TechtonicaVR.VrPlayer;

public class MainGamePlayer : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<MainGamePlayer>();

	public static MainGamePlayer instance;

	public LightController lightController;

	private void Start()
	{
		if (instance != null)
		{
			Logger.LogWarning("MainGamePlayer already exists! Destroying old instance!");
			Destroy(instance);
		}

		instance = this;

		Logger.LogDebug("Setting up light controller...");
		var headlamps = FindObjectsOfType<Headlamp>();
		headlamps.ForEach(h => h.transform.SetParent(VRCameraManager.mainCamera.transform, false));
		lightController = new LightController(headlamps);
	}
}
