using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

class NotificationCanvasPatche : GameComponentPatch<NotificationUI>

{
    protected override bool Apply(NotificationUI component)
    {
        var tlc = component.gameObject.transform.GetChild(0);
        if (tlc == null)
        {
            return false;
        }

        tlc.transform.position = new Vector3(tlc.transform.position.x, 14, tlc.transform.position.z);
        return true;
    }
}