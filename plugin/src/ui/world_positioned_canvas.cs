using TechtonicaVR.Util;
using TechtonicaVR.VRCamera;
using UnityEngine;

namespace TechtonicaVR.UI;

public class WorldPositionedCanvas : MonoBehaviour
{
	public UIMenu menu;

	public Vector3 target;
	public Vector3 camOrigin;

	private void Start()
	{
		if (menu == null)
		{
			Plugin.Logger.LogError("WorldPositionedCanvas: menu is null");
			Destroy(this);
		}
	}

	private void Update()
	{
		if (target == Vector3.zero || !menu.isOpen)
		{
			return;
		}

		menu.myCanvas.renderMode = RenderMode.ScreenSpaceCamera;

		var cam = VRCameraManager.mainCamera;
		MathyStuff.PositionCanvasInWorld(gameObject, cam, target, camOrigin);
	}
}
