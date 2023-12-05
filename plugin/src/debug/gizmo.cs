using UnityEngine;

namespace TechtonicaVR.Debug;

public class Gizmo : MonoBehaviour
{
	public Color colorUp = Color.green;
	public Color colorForward = Color.blue;
	public Color colorRight = Color.red;

	private LineRenderer lineUp;
	private LineRenderer lineForward;
	private LineRenderer lineRight;

	public float radius = 0.5f;

	private void Start()
	{
		if (!ModConfig.GizmoEnabled())
		{
			Destroy(this);
			return;
		}

		var upObject = new GameObject("Up");
		upObject.transform.parent = transform;
		lineUp = upObject.AddComponent<LineRenderer>();
		lineUp.startWidth = 0.01f;
		lineUp.endWidth = 0.01f;
		lineUp.positionCount = 2;
		lineUp.material = new Material(Shader.Find("Unlit/Color"))
		{
			color = colorUp
		};

		var forwardObject = new GameObject("Forward");
		forwardObject.transform.parent = transform;
		lineForward = forwardObject.AddComponent<LineRenderer>();
		lineForward.startWidth = 0.01f;
		lineForward.endWidth = 0.01f;
		lineForward.positionCount = 2;
		lineForward.material = new Material(Shader.Find("Unlit/Color"))
		{
			color = colorForward
		};

		var rightObject = new GameObject("Right");
		rightObject.transform.parent = transform;
		lineRight = rightObject.AddComponent<LineRenderer>();
		lineRight.startWidth = 0.01f;
		lineRight.endWidth = 0.01f;
		lineRight.positionCount = 2;
		lineRight.material = new Material(Shader.Find("Unlit/Color"))
		{
			color = colorRight
		};
	}

	private void Update()
	{
		lineUp.SetPosition(0, transform.position);
		lineUp.SetPosition(1, transform.position + transform.up * radius);

		lineForward.SetPosition(0, transform.position);
		lineForward.SetPosition(1, transform.position + transform.forward * radius);

		lineRight.SetPosition(0, transform.position);
		lineRight.SetPosition(1, transform.position + transform.right * radius);
	}
}
