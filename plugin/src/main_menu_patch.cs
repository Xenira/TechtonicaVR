using System;
using System.Linq;
using TechtonicaVR.Patches.MainMenu;
using UnityEngine;

namespace TechtonicaVR;

public class MainMenuPatch : MonoBehaviour
{

    IPatch[] patches = [
        new CameraOriginPatch(),
        new SpaceBGCameraPatch(),
        new MenuCanvasPatch(),
    ];

    private float startTime = Time.time;

    public static MainMenuPatch Create()
    {
        var instance = new GameObject(nameof(MainMenuPatch)).AddComponent<MainMenuPatch>();

        return instance;
    }

    void Start()
    {
        Debug.Log("Hello Main Menu!");
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