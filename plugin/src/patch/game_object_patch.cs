using TechtonicaVR.Util;
using UnityEngine;

namespace TechtonicaVR.Patches;

public abstract class GameObjectPatch : IPatch
{
    private string gameObjectName;
    private bool applied = false;

    public GameObjectPatch(string gameObjectName)
    {
        this.gameObjectName = gameObjectName;
    }

    public bool Apply()
    {
        var obj = GameObject.Find(gameObjectName) ?? GameObjectFinder.FindObjectByName(gameObjectName);
        if (obj == null)
        {
            return false;
        }

        applied = Apply(obj);
        return applied;
    }

    public bool IsApplied()
    {
        return applied;
    }

    protected abstract bool Apply(GameObject gameObject);
}