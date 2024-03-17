using PiUtils.Patches;
using PiUtils.Util;

namespace TechtonicaVR.Patches.MainGame.UI;

public class SystemLogUiPatch : GameComponentPatch<SystemLogUI>
{
	protected override bool Apply(SystemLogUI component)
	{
		var log = GameObjectFinder.FindChildObjectByName("System Log", component.gameObject);
		if (log == null)
		{
			return false;
		}

		log.transform.localPosition = new UnityEngine.Vector3(0, 500, 0);

		return true;
	}
}
