using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TechtonicaVR.Util;

public class GameObjectFinder
{

	private static Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

	public static GameObject FindObjectByName(string name)
	{
		if (cache.ContainsKey(name))
		{
			return cache[name];
		}

		Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
		GameObject obj = objs.FirstOrDefault(o => o.gameObject.name == name)?.gameObject;

		if (obj != null)
		{
			cache[name] = obj;
		}

		return obj;
	}

	public static IEnumerable<GameObject> FindParentObjectsByName(string name, GameObject child)
	{
		Transform[] objs = child.GetComponentsInParent<Transform>();
		return objs.Where(o => o.gameObject.name == name).Select(o => o.gameObject);
	}

	public static GameObject FindParentObjectByName(string name, GameObject child)
	{
		Transform[] objs = child.GetComponentsInParent<Transform>();
		GameObject obj = objs.FirstOrDefault(o => o.gameObject.name == name)?.gameObject;

		return obj;
	}

	public static IEnumerable<GameObject> FindChildObjectsByName(string name, GameObject parent)
	{
		Transform[] objs = parent.GetComponentsInChildren<Transform>();
		return objs.Where(o => o.gameObject.name == name).Select(o => o.gameObject);
	}

	public static GameObject FindChildObjectByName(string name, GameObject parent)
	{
		Transform[] objs = parent.GetComponentsInChildren<Transform>();
		GameObject obj = objs.FirstOrDefault(o => o.gameObject.name == name)?.gameObject;

		return obj;
	}

	public static GameObject FindChildObjectByName(string name, Transform parent)
	{
		return FindChildObjectByName(name, parent.gameObject);
	}

	public static IEnumerable<GameObject> FindSiblingChildObjectsByName(string name, GameObject sibling)
	{
		if (sibling.transform.parent == null)
		{
			yield break;
		}

		for (int i = 0; i < sibling.transform.parent.childCount; i++)
		{
			var child = sibling.transform.parent.GetChild(i);
			if (child.gameObject.name == name)
			{
				yield return child.gameObject;
			}
		}
	}
}
