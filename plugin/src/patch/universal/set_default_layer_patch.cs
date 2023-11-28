using HarmonyLib;
using UnityEngine;

namespace TechtonicaVR.Patches.Universal;

[HarmonyPatch]
public class SetDefaultLayerPatch : GameObjectPatch
{
    private bool applyToChildren;
    public SetDefaultLayerPatch(string gameObjectName, bool applyToChildren) : base(gameObjectName)
    {
        this.applyToChildren = applyToChildren;
    }

    protected override bool Apply(GameObject gameObject)
    {
        if (applyToChildren)
        {
            SetChildLayer(gameObject, 0);
        }
        else
        {
            gameObject.layer = 0;
        }
        return true;
    }

    public static void SetChildLayer(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            SetChildLayer(child.gameObject, layer);
        }
    }
}
