using UnityEngine;

namespace TechtonicaVR.Patches.Universal;

public class DisableByNamePatch : GameObjectPatch
{
    private bool applied = false;

    public DisableByNamePatch(string gameObjectName) : base(gameObjectName)
    {
    }

    protected override bool Apply(GameObject component)
    {
        component.gameObject.SetActive(false);
        return true;
    }
}