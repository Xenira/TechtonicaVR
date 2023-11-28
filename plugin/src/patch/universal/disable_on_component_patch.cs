using System;
using UnityEngine;

namespace TechtonicaVR.Patches.Universal;

public class DisableOnComponentPatch<T> : GameComponentPatch<T> where T : MonoBehaviour
{
    private bool applied = false;

    protected override bool Apply(T component)
    {
        component.gameObject.SetActive(false);
        return true;
    }
}