using TechtonicaVR.Util;
using TechtonicaVR.VRCamera;
using UnityEngine;

namespace TechtonicaVR.UI;

public class WorldPositionedCanvas : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<WorldPositionedCanvas>();

	public Menu menu;
	public PlayerInventoryUI playerInventoryUI;

	public Vector3 target;
	public Vector3 camOrigin;
	public Vector3 scale;

	private void Start()
	{
		if (menu == null)
		{
			Logger.LogError("WorldPositionedCanvas: menu is null");
			Destroy(this);
		}
	}

	private void Update()
	{
		var isOpen = menu.isOpen();
		if (target == Vector3.zero || !isOpen)
		{
			return;
		}

		var cam = VRCameraManager.mainCamera;
		if (isOpen)
		{
			MathyStuff.PositionCanvasInWorld(gameObject, cam, target, camOrigin);
		}

		if (playerInventoryUI != null && playerInventoryUI.isOpen)
		{
			MathyStuff.PositionCanvasInWorld(playerInventoryUI.gameObject.transform.GetChild(0).gameObject, cam, target, camOrigin);
		}
	}
}
