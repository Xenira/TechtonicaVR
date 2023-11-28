using System;
using System.Linq;
using TechtonicaVR.Patches;
using TechtonicaVR.Patches.Player;
using TechtonicaVR.Patches.UI;
using TechtonicaVR.Patches.Universal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TechtonicaVR;

public interface IPatch
{
    bool Apply();
    bool IsApplied();
}

public class PatchBehaviour : MonoBehaviour
{

    private IPatch[] playerSpringPatches = [
        new LeftHandAttachPatch(),
        new RightHandAttachPatch(),
        new SetDefaultLayerPatch("Pickaxe", true),
        new SetDefaultLayerPatch("Scanner", true),
        new SetDefaultLayerPatch("Spectral Cube (Sparks)", true),
        new SetDefaultLayerPatch("Spectral Cube (Paladin)", true),
        // new AimTransformPatch(),
    ];

    IPatch[] patches = [];

    private float startTime = Time.time;

    public static PatchBehaviour Create()
    {
        var instance = new GameObject(nameof(PatchBehaviour)).AddComponent<PatchBehaviour>();

        return instance;
    }

    void Start()
    {
        patches = playerSpringPatches.Concat([
            new ExecuteAfterPatch(new DisableByNamePatch("Astronaut_LP_ArmsTorso"), playerSpringPatches),
            new NotificationCanvasPatche(),
            new ToolbarUiPatch(),
            new QuestTaskListPatch(),
            new InventoryAndCraftingPatch(),
            new DialoguePopupPatch(),
        ]).ToArray();

        Debug.Log("Hello World!");
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