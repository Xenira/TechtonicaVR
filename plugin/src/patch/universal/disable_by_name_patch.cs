using UnityEngine;

namespace TechtonicaVR.Patches.Universal;

public class DisableByNamePatch : GameObjectPatch
{
	public DisableByNamePatch(string gameObjectName) : base(gameObjectName)
	{
	}

	protected override bool Apply(GameObject component)
	{
		component.gameObject.SetActive(false);
		return true;
	}
}
