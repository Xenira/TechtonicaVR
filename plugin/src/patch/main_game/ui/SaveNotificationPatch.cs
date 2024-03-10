using PiUtils.Patches;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

public class SaveNotificationPatch : GameComponentPatch<SaveNotification>
{
	protected override bool Apply(SaveNotification component)
	{
		var tlc = component.transform.GetChild(0).gameObject;
		if (tlc == null)
		{
			return false;
		}

		var image = tlc.transform.GetChild(0).gameObject;
		if (image == null)
		{
			return false;
		}

		image.transform.localPosition = new Vector3(200, -200, -500);

		return true;
	}
}
