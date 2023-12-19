using TechtonicaVR.VRCamera;
using UnityEngine;

namespace TechtonicaVR.UI;

class HandTrackedCanvas : MonoBehaviour
{
	public Transform hand;
	public Vector3 offset = Vector3.zero;
	public Vector3 showDirection;
	public Canvas canvas;
	public RectTransform rectTransform;
	public Transform rectTransformOverride;
	public Vector3 transformOverride = Vector3.zero;
	public Vector3 tlcLocalPosition = Vector3.zero;

	public bool noTransform = false;
	public float showDistance;

	private void Start()
	{
		var anchor = new GameObject("UIAnchor");
		anchor.transform.parent = hand;
		anchor.transform.localPosition = offset;

		canvas = gameObject.transform.parent.GetComponentInParent<Canvas>();
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.gameObject.layer = 0;

		if (rectTransform == null)
		{
			rectTransform = gameObject.GetComponentInChildren<RectTransform>();
		}

		canvas.gameObject.transform.parent = anchor.transform;
		canvas.gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localPosition = tlcLocalPosition;
	}

	private void Update()
	{
		if (!ShouldShow())
		{
			canvas.enabled = false;
			return;
		}

		canvas.enabled = true;
		canvas.gameObject.transform.LookAt(VRCameraManager.mainCamera.transform);

		if (noTransform)
		{
			return;
		}

		var rect = rectTransform.rect;
		var transform = rectTransformOverride != null ? rectTransformOverride : rectTransform.transform;
		var localPosition = transformOverride != Vector3.zero ? transformOverride : new Vector3(-rect.width / 2, rect.height, 0);
		transform.localPosition = localPosition;
	}

	private bool ShouldShow()
	{
		var direction = hand.TransformDirection(showDirection);
		return 1 - direction.y <= showDistance;
	}
}
