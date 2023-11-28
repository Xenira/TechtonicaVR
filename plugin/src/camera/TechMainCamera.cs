using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace TechtonicaVR.VRCamera;

public class TechMainCamera : MonoBehaviour
{
    private PostProcessLayer postProcessLayer;

    private void Start()
    {
        // Get the PostProcessLayer component attached to the camera
        postProcessLayer = GetComponent<PostProcessLayer>();

        // Disable the PostProcessLayer
        if (postProcessLayer != null)
        {
            postProcessLayer.enabled = false;
        }
    }
}
