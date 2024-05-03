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

	public static IEnumerator Load()
	{
		var playerBundle = assetLoader.LoadBundle("player");
		LeftHandBase = assetLoader.LoadAsset<GameObject>(playerBundle, "hands/left_hand.prefab");
		RightHandBase = assetLoader.LoadAsset<GameObject>(playerBundle, "hands/right_hand.prefab");

		yield break;
	}
}
