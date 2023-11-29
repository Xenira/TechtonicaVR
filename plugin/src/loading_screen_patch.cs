using System;
using System.Linq;
using TechtonicaVR.Patches.LoadingScreen;
using UnityEngine;

namespace TechtonicaVR;

public class LoadingScreenPatch : MonoBehaviour
{

    IPatch[] patches = [
        new LoadingScreenCanvasPatch(),
    ];

    private float startTime = Time.time;

    public static LoadingScreenPatch Create()
    {
        var instance = new GameObject(nameof(LoadingScreenPatch)).AddComponent<LoadingScreenPatch>();

        return instance;
    }

    void Start()
    {
        Plugin.Logger.LogDebug("Hello Loading Screen!");
    }

    void Update()
    {
        patches = patches.Where(p => !p.Apply()).ToArray();

        if (!patches.Any())
        {
            gameObject.SetActive(false);
        }
    }
}