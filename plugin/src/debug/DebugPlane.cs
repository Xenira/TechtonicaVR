using UnityEngine;

namespace TechtonicaVR.Debug;

public class DebugPlane : MonoBehaviour
{
	private GameObject plane;

	private void Start()
	{
		if (!ModConfig.DebugEnabled())
		{
			Destroy(this);
			return;
		}

		plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		plane.GetComponent<MeshCollider>().enabled = false;
		var meshRenderer = plane.AddComponent<MeshRenderer>();
		var material = new Material(Shader.Find("Unlit/Color"));
		material.SetColor("_Color", Color.red);
		meshRenderer.material = material;

		plane.AddComponent<Gizmo>();
	}

	public void DrawPlane(Vector3 position, Vector3 normal, float size = 1.0f)
	{
		plane.transform.position = position;
		plane.transform.up = normal;
		plane.transform.localScale = new Vector3(size, 1, size);
	}
}
