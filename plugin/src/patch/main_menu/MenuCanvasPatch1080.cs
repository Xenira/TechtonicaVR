using PiUtils.Patches;
using PiUtils.Util;
using UnityEngine;

namespace TechtonicaVR.Patches.MainMenu;

public class MenuCanvasPatch1080 : GameObjectPatch
{
	private static PluginLogger Logger = PluginLogger.GetLogger<MenuCanvasPatch1080>();
	public MenuCanvasPatch1080() : base("Canvas 1080")
	{
	}

	protected override bool Apply(GameObject gameObject)
	{
		var origin = GameObject.Find("Main Camera (origin)");
		if (origin == null)
		{
			return false;
		}

		gameObject.transform.parent = origin.transform;
		gameObject.transform.localPosition = new Vector3(5.5982f, 0.7818f, 5.9599f);
		gameObject.transform.rotation = Quaternion.Euler(0, 40.4374f, 0);
		gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

		var canvas = gameObject.GetComponent<Canvas>();
		canvas.renderMode = RenderMode.WorldSpace;

		Logger.LogWarning("MenuCanvasPatch1080 applied");

		return true;
	}
}
