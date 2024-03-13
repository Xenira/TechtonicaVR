using System.Collections;
using PiUtils.Util;
using UnityEngine;

namespace TechtonicaVR.Assets;

class AssetLoader
{
	private static PluginLogger Logger = PluginLogger.GetLogger<AssetLoader>();

	public static PiUtils.Assets.AssetLoader assetLoader;

	public static GameObject LeftHandBase;
	public static GameObject RightHandBase;
	public static GameObject Vignette;

	public static AudioClip TeleportGo;
	public static AudioClip TeleportPointerStart;
	public static AudioClip TeleportPointerLoop;
	public static AudioClip SnapTurn;

	public static Material TeleportPointerMat;

	public static IEnumerator Load()
	{
		var playerBundle = assetLoader.LoadBundle("player");
		LeftHandBase = assetLoader.LoadAsset<GameObject>(playerBundle, "hands/left_hand.prefab");
		RightHandBase = assetLoader.LoadAsset<GameObject>(playerBundle, "hands/right_hand.prefab");
		Vignette = assetLoader.LoadAsset<GameObject>(playerBundle, "comfort/vignette.prefab");

		var steamVrBundle = assetLoader.LoadBundle("steamvr");
		TeleportPointerMat = assetLoader.LoadAsset<Material>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Materials/TeleportPointer.mat");

		TeleportGo = assetLoader.LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Sounds/TeleportGo.wav");
		TeleportPointerStart = assetLoader.LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Sounds/TeleportPointerStart.wav");
		TeleportPointerLoop = assetLoader.LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Sounds/TeleportPointerLoop.wav");
		SnapTurn = assetLoader.LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/SnapTurn/snapturn_go_01.wav");

		yield break;
	}
}
