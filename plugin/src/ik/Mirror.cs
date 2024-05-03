using UnityEngine;

namespace TechtonicaVR.Ik;

public class Mirror : MonoBehaviour
{
	private Camera mirrorCamera;
	private GameObject mirrorObject;

	private void Start()
	{
		mirrorObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

		mirrorObject.transform.parent = transform;
		mirrorObject.transform.localPosition = new Vector3(0, 0, 1);
		mirrorObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
		mirrorObject.transform.localScale = new Vector3(2, 2, 1);
		mirrorObject.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Texture"));

		mirrorCamera = new GameObject("Mirror Camera").AddComponent<Camera>();
		mirrorCamera.transform.parent = transform;
		mirrorCamera.transform.localPosition = new Vector3(0, 0, 0);
		mirrorCamera.transform.localRotation = Quaternion.identity;
		mirrorCamera.transform.localScale = new Vector3(1, 1, 1);
		mirrorCamera.targetTexture = new RenderTexture(1024, 1024, 24);
		mirrorCamera.clearFlags = CameraClearFlags.SolidColor;
		mirrorCamera.backgroundColor = Color.black;
		mirrorCamera.nearClipPlane = 0.01f;
		mirrorCamera.farClipPlane = 1000f;
		mirrorCamera.depth = -1;

		mirrorObject.GetComponent<Renderer>().material.mainTexture = mirrorCamera.targetTexture;
	}

	private void Update()
	{
		Vector3 cameraOffset = Camera.main.transform.position - transform.position;
		mirrorCamera.transform.position = transform.position - cameraOffset;
		mirrorCamera.transform.LookAt(transform.position);
	}
}
