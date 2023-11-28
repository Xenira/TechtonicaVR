using System.Collections.Generic;
using System.Linq;
using GameAnalyticsSDK.Setup;
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
}