using UnityEngine;

namespace TechtonicaVR.Patches.MainGame.UI;

class QuestTaskListPatch : GameComponentPatch<QuestUI>

{
    protected override bool Apply(QuestUI component)
    {
        var tlc = component.gameObject.transform.GetChild(0);
        if (tlc == null)
        {
            return false;
        }

        tlc.transform.position = new Vector3(2.1f, 13.4f, 30.69f);
        return true;
    }
}