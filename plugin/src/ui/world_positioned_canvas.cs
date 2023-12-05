using TechtonicaVR.Util;
using TechtonicaVR.VRCamera;
using UnityEngine;

namespace TechtonicaVR.UI;

public class WorldPositionedCanvas : MonoBehaviour
{
	public UIMenu menu;
	public PlayerInventoryUI playerInventoryUI;

	public Vector3 target;
	public Vector3 camOrigin;
	public Vector3 scale;

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

		var cam = VRCameraManager.mainCamera;
		if (menu.isOpen)
		{
			MathyStuff.PositionCanvasInWorld(gameObject, cam, target, camOrigin);
		}

		if (playerInventoryUI != null && playerInventoryUI.isOpen)
		{
			MathyStuff.PositionCanvasInWorld(playerInventoryUI.gameObject.transform.GetChild(0).gameObject, cam, target, camOrigin);
		}
	}
}
