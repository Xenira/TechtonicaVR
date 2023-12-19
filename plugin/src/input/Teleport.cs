using TechtonicaVR.Assets;
using TechtonicaVR.VRCamera;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace TechtonicaVR.Input;

class Teleport : MonoBehaviour
{
	private TeleportArc teleportArc;
	private bool teleporting = false;
	private RaycastHit hitPoint;
	private float teleportRange = 12f;

	// Audio is currently not working
	private AudioSource audioSource;
	private AudioSource loopAudioSource;
	private AudioClip teleportGo;
	private AudioClip teleportPointerStart;

	public static Teleport Create()
	{
		var instance = new GameObject(nameof(Teleport)).AddComponent<Teleport>();

		return instance;
	}

	private void Awake()
	{
		teleportRange = ModConfig.teleportRange.Value;

		teleportArc = gameObject.AddComponent<TeleportArc>();
		teleportArc.material = Instantiate(AssetLoader.TeleportPointerMat);
		teleportArc.traceLayerMask = Player.instance.builder.defaultMask;

		VRCameraManager.mainCamera.gameObject.AddComponent<AudioListener>();

		// Audio is currently not working
		audioSource = new GameObject("Teleport Audio Src").AddComponent<AudioSource>();
		audioSource.transform.parent = VRCameraManager.mainCamera.transform;
		audioSource.transform.localPosition = Vector3.zero;
		audioSource.playOnAwake = false;

		loopAudioSource = new GameObject("Teleport Loop Audio Src").AddComponent<AudioSource>();
		loopAudioSource.transform.parent = VRCameraManager.mainCamera.transform;
		loopAudioSource.transform.localPosition = Vector3.zero;
		loopAudioSource.clip = AssetLoader.TeleportPointerLoop;
		loopAudioSource.loop = true;
		loopAudioSource.playOnAwake = false;

		teleportGo = AssetLoader.TeleportGo;
		teleportPointerStart = AssetLoader.TeleportPointerStart;
	}

	private void Update()
	{
		if (SteamVRInputMapper.teleport.IsPressed())
		{
			teleporting = true;
			teleportArc.Show();
			audioSource.PlayOneShot(teleportPointerStart);
			loopAudioSource.Play();
		}

		if (teleporting)
		{
			UpdateTeleport();
		}

		if (SteamVRInputMapper.teleport.IsReleased())
		{
			loopAudioSource.Stop();

			teleporting = false;
			teleportArc.Hide();
			TeleportToHitPoint();
		}
	}

	private void UpdateTeleport()
	{
		teleportArc.SetArcData(SteamVRInputMapper.rightHandObject.transform.position, -SteamVRInputMapper.rightHandObject.transform.up * teleportRange, true, false);
		teleportArc.DrawArc(out hitPoint);

		if (hitPoint.collider != null)
		{
			teleportArc.SetColor(Color.green);
		}
		else
		{
			teleportArc.SetColor(Color.red);
		}
	}

	private void TeleportToHitPoint()
	{
		if (hitPoint.collider == null)
		{
			return;
		}

		audioSource.PlayOneShot(teleportGo);
		Player.instance.fpcontroller.transform.position = hitPoint.point;
	}

	private void OnDisable()
	{
		teleportArc.Hide();
		teleporting = false;
	}

}
