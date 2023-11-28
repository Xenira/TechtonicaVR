using TechtonicaVR.Patches.Universal;
using UnityEngine;

namespace TechtonicaVR.Patches.UI;

class ToolbarUiPatch : GameComponentPatch<ToolbarUI>

{
    protected override bool Apply(ToolbarUI component)
    {
        component.gameObject.transform.position = new Vector3(component.gameObject.transform.position.x, 20, 30.4f);
        SetDefaultLayerPatch.SetChildLayer(component.gameObject, 0);
        return true;
    }

}