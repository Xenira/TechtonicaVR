using System.IO;
using BepInEx;
using UnityEngine;

namespace TechtonicaVR.Assets;

class AssetLoader
{
    private const string assetPath = "techtonica_vr/assets";

    public static GameObject LeftHandBase;
    public static GameObject RightHandBase;

    public AssetLoader()
    {
        var bundle = LoadBundle("player");
        LeftHandBase = LoadAsset<GameObject>(bundle, "hands/left_hand.prefab");
        RightHandBase = LoadAsset<GameObject>(bundle, "hands/right_hand.prefab");
        // LeftHandBase = LoadAsset<GameObject>(bundle, "SteamVR/Prefabs/vr_glove_left_model_slim.prefab");
        // RightHandBase = LoadAsset<GameObject>(bundle, "SteamVR/Prefabs/vr_glove_right_model_slim.prefab");

    }

    private T LoadAsset<T>(AssetBundle bundle, string prefabName) where T : UnityEngine.Object
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
            AssetBundle.LoadFromFile(Path.Combine(Paths.PluginPath, Path.Combine(assetPath, assetName)));
        if (bundle == null)
        {
            Plugin.Logger.LogError($"Failed to load AssetBundle {assetName}");
            return null;
        }

        return bundle;
    }
}