using PiUtils.Patches;
using UnityEngine;

namespace TechtonicaVR.Patches.MainMenu;

public class MenuCanvasPatch : GameObjectPatch
{
	public MenuCanvasPatch() : base("Canvas")
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

		return true;
	}
}
