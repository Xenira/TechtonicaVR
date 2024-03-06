using PiUtils.Patches;
using TechtonicaVR.VRCamera.Patch;
using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

class InspectorCanvasPatch : GameComponentPatch<InspectorUI>

{
	protected override bool Apply(InspectorUI gameObject)
	{
		var tlc = gameObject.transform.GetChild(0);
		if (tlc == null)
		{
			return false;
		}

		var sharedInspector = tlc.transform.GetChild(0);
		if (sharedInspector == null)
		{
			return false;
		}
		sharedInspector.transform.localPosition = new Vector3(23.8832f, -21.9059f, 0);

		var scannableInspector = tlc.gameObject.transform.GetChild(1).transform.GetChild(0);
		if (scannableInspector == null)
		{
			return false;
		}
		scannableInspector.transform.localPosition = new Vector3(211.4383f, -22.6601f, 0);


		TargetRaycastPatch.inspectorTlc = tlc;

		return true;
	}
}
