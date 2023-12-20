using UnityEngine;

namespace TechtonicaVR.Patches.Universal;

public class DisableComponentPatch<T> : GameComponentPatch<T> where T : MonoBehaviour
{
	protected override bool Apply(T component)
	{
		component.enabled = false;
		return true;
	}
}
