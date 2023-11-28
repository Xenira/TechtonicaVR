using System.Linq;

namespace TechtonicaVR.Patches;

public class ExecuteAfterPatch : IPatch
{
    private bool applied = false;
    private IPatch patch;
    private IPatch[] dependsOn;

    public ExecuteAfterPatch(IPatch patch, IPatch[] dependsOn)
    {
        this.patch = patch;
        this.dependsOn = dependsOn;
    }

    public bool Apply()
    {
        if (dependsOn.Any(p => !p.IsApplied()))
        {
            return false;
        }

        applied = patch.Apply();
        return applied;
    }

    public bool IsApplied()
    {
        return applied;
    }
}