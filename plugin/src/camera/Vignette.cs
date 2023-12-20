using System;
using System.Collections;
using TechtonicaVR.Assets;
using UnityEngine;

namespace TechtonicaVR.VRCamera;

class Vignette : MonoBehaviour
{
	public static Vignette instance;

	private Material vignetteMat;

	private float vignetteCurrent = 0f;
	private float vignetteTarget = 0f;
	private float startTime = 0f;

	private int oneShots = 0;

	private float frameMaxIntensity = 1f;

	public static Vignette Create()
	{
		instance = new GameObject(nameof(Vignette)).AddComponent<Vignette>();

		return instance;
	}

	private void Awake()
	{
		var camera = VRCameraManager.mainCamera;
		gameObject.transform.parent = camera.transform;
		gameObject.transform.localPosition = new Vector3(0, 0, 1);
		gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		gameObject.transform.localRotation = Quaternion.identity;

		var vignette = Instantiate(AssetLoader.Vignette);
		vignette.transform.parent = gameObject.transform;
		vignette.transform.localPosition = Vector3.zero;
		vignette.transform.localRotation = Quaternion.identity;


		vignetteMat = vignette.GetComponent<MeshRenderer>().material;
		vignetteMat.SetFloat("_Blur", ModConfig.vignetteSmoothness.Value);
		vignetteMat.SetColor("_VignetteColor", ModConfig.vignetteColor.Value);
	}

	public float Show(float intensity = 1f)
	{
		var target = intensity * ModConfig.vignetteIntensity.Value;
		if (oneShots > 0 && target < vignetteTarget || intensity < frameMaxIntensity)
		{
			return target < vignetteCurrent ? 0 : GetRemainingTime(target);
		}

		frameMaxIntensity = intensity;

		vignetteTarget = target;
		startTime = Time.time;
		return GetRemainingTime(target);
	}

	public void OneShot(Action callback, float intensity = 1f)
	{
		oneShots++;
		var time = Show(intensity);
		StartCoroutine(OneShotCoroutine(callback, time));
	}

	private IEnumerator OneShotCoroutine(Action callback, float time)
	{
		yield return new WaitForSeconds(time);
		callback();
		if (--oneShots == 0)
		{
			Hide();
		}
	}

	public void Hide()
	{
		vignetteTarget = 0f;
		startTime = Time.time;
	}

	private void Update()
	{
		if (vignetteCurrent != vignetteTarget)
		{
			var direction = vignetteCurrent > vignetteTarget ? -1 : 1;
			vignetteCurrent = Mathf.Clamp(vignetteCurrent + Time.deltaTime * ModConfig.vignetteFadeSpeed.Value * direction, Mathf.Min(vignetteCurrent, vignetteTarget), Mathf.Max(vignetteCurrent, vignetteTarget));
			vignetteMat.SetFloat("_Radius", vignetteCurrent);
		}
	}

	private void LateUpdate()
	{
		frameMaxIntensity = 0f;
	}

	private float GetRemainingTime(float target)
	{
		return Mathf.Abs(vignetteCurrent - target) / ModConfig.vignetteFadeSpeed.Value;
	}
}
