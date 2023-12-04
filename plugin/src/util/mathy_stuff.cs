using UnityEngine;
using Valve.VR;

namespace TechtonicaVR.Util;

public class MathyStuff
{
	public static void PositionCanvasInWorld(Camera cam, Vector3 point, GameObject tlc)
	{
		Vector3 screenPoint = cam.WorldToScreenPoint(point);
		Vector3 canvasPoint = tlc.transform.parent.GetComponent<Canvas>().worldCamera.ScreenToWorldPoint(screenPoint);
		tlc.transform.position = canvasPoint;
	}

	public static Matrix4x4 HmdMatrix44ToMatrix4x4(HmdMatrix44_t hmdMatrix)
	{
		Matrix4x4 matrix = new Matrix4x4();

		matrix[0, 0] = hmdMatrix.m0;
		matrix[0, 1] = hmdMatrix.m1;
		matrix[0, 2] = hmdMatrix.m2;
		matrix[0, 3] = hmdMatrix.m3;
		matrix[1, 0] = hmdMatrix.m4;
		matrix[1, 1] = hmdMatrix.m5;
		matrix[1, 2] = hmdMatrix.m6;
		matrix[1, 3] = hmdMatrix.m7;
		matrix[2, 0] = hmdMatrix.m8;
		matrix[2, 1] = hmdMatrix.m9;
		matrix[2, 2] = hmdMatrix.m10;
		matrix[2, 3] = hmdMatrix.m11;
		matrix[3, 0] = hmdMatrix.m12;
		matrix[3, 1] = hmdMatrix.m13;
		matrix[3, 2] = hmdMatrix.m14;
		matrix[3, 3] = hmdMatrix.m15;

		return matrix;
	}
}
