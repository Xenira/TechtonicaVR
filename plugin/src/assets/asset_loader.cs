using System.Collections;
using System.IO;
using BepInEx;
using UnityEngine;

namespace TechtonicaVR.Assets;

class AssetLoader
{
	private const string assetPath = "techtonica_vr/assets";

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
		var playerBundle = LoadBundle("player");
		LeftHandBase = LoadAsset<GameObject>(playerBundle, "hands/left_hand.prefab");
		RightHandBase = LoadAsset<GameObject>(playerBundle, "hands/right_hand.prefab");
		Vignette = LoadAsset<GameObject>(playerBundle, "comfort/vignette.prefab");

		var steamVrBundle = LoadBundle("steamvr");
		TeleportPointerMat = LoadAsset<Material>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Materials/TeleportPointer.mat");

		TeleportGo = LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Sounds/TeleportGo.wav");
		TeleportPointerStart = LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Sounds/TeleportPointerStart.wav");
		TeleportPointerLoop = LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/Teleport/Sounds/TeleportPointerLoop.wav");
		SnapTurn = LoadAsset<AudioClip>(steamVrBundle, "SteamVR/InteractionSystem/SnapTurn/snapturn_go_01.wav");

		yield break;
	}

	private static T LoadAsset<T>(AssetBundle bundle, string prefabName) where T : Object
	{
		var asset = bundle.LoadAsset<T>($"Assets/{prefabName}");
		if (asset)
			return asset;
		else
		{
			Plugin.Logger.LogError($"Failed to load asset {prefabName}");
			return null;
		}

	}

	private static AssetBundle LoadBundle(string assetName)
	{
		var bundle =
				AssetBundle.LoadFromFile(GetAssetPath(assetName));
		if (bundle == null)
		{
			Plugin.Logger.LogError($"Failed to load AssetBundle {assetName}");
			return null;
		}

		return bundle;
	}

	private static string GetAssetPath(string assetName)
	{
		return Path.Combine(Paths.PluginPath, Path.Combine(assetPath, assetName));
	}
}
