using UnityEngine;

namespace TechtonicaVR.Patches;

public abstract class GameComponentPatch<T> : IPatch where T : Object
{
    private bool applied = false;

    public bool Apply()
    {
        var component = GameObject.FindObjectOfType<T>();
        if (component == null)
        {
            return false;
        }

        applied = Apply(component);
        return applied;
    }

    public bool IsApplied()
    {
        return applied;
    }

    protected abstract bool Apply(T component);
}