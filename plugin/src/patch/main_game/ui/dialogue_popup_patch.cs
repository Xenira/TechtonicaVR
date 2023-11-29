using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

class DialoguePopupPatch : GameComponentPatch<DialogueEntryPopupUI>

{
    protected override bool Apply(DialogueEntryPopupUI component)
    {
        var tlc = component.gameObject.transform;
        if (tlc == null)
        {
            return false;
        }

        tlc.transform.position = new Vector3(tlc.position.x, 16f, tlc.position.z);
        return true;
    }
}