using UnityEngine;

namespace TechtonicaVR.Debug;

public class DebugLine : MonoBehaviour
{
    public Transform start;
    public Transform end;

    private LineRenderer line;

    private void Start()
    {
        if (ModConfig.DebugLineEnabled() == false)
        {
            Destroy(this);
            return;
        }

        line = gameObject.AddComponent<LineRenderer>();
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        line.positionCount = 2;
    }

    private void Update()
    {
        line.SetPosition(0, start.position);
        line.SetPosition(1, end.position);
    }
}