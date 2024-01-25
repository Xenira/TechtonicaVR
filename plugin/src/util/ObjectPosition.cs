using UnityEngine;

namespace TechtonicaVR.Util;

public class ObjectPosition
{
	private static PluginLogger Logger = PluginLogger.GetLogger<ObjectPosition>();

	public static Vector3 addLocalPositions(Transform transform, Transform stopAt = null)
	{
		var position = transform.localPosition;
		while (transform.parent != null && transform.parent != stopAt)
		{
			transform = transform.parent;
			Logger.LogDebug($"Adding {transform.localPosition} from {transform.gameObject.name} to {position}");
			position += transform.localPosition;
		}
		return position;
	}

	public static Vector3 addLocalPositions(Transform transform, int stopAt)
	{
		var position = transform.localPosition;
		while (transform.transform.parent != null && stopAt > 0)
		{
			transform = transform.parent;
			Logger.LogDebug($"Adding {transform.localPosition} from {transform.gameObject.name} to {position}");
			position += transform.localPosition;
			stopAt--;
		}
		if (stopAt > 0)
		{
			Logger.LogWarning($"Ran out of parents to add to {transform.gameObject.name} ({stopAt} remaining)");
		}
		return position;
	}
}
