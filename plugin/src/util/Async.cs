using System;
using System.Collections;
using UnityEngine;

namespace TechtonicaVR.Util;

public class Async
{
	public static IEnumerator Timeout(Action callback, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		callback();
	}

	public static IEnumerator TimeoutFrames(Action callback, int frames)
	{
		for (int i = 0; i < frames; i++)
		{
			yield return new WaitForEndOfFrame();
		}
		callback();
	}

	public static IEnumerator Interval(Action callback, float seconds, float startInSeconds)
	{
		yield return new WaitForSeconds(startInSeconds < 0 ? seconds : startInSeconds);
		while (true)
		{
			callback();
			yield return new WaitForSeconds(seconds);
		}
	}
}

public class AsyncGameObject : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<AsyncGameObject>();
	private static AsyncGameObject instance;

	public static AsyncGameObject Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject(nameof(AsyncGameObject)).AddComponent<AsyncGameObject>();
			}
			return instance;
		}
	}

	void Awake()
	{
		if (instance != null)
		{
			Logger.LogError("AsyncGameObject already exists, destroying this one");
			Destroy(this);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void timeout(Action callback, float seconds)
	{
		StartCoroutine(Async.Timeout(callback, seconds));
	}

	public void timeoutFrames(Action callback, int frames)
	{
		StartCoroutine(Async.TimeoutFrames(callback, frames));
	}

	public void interval(Action callback, float seconds, float startInSeconds)
	{
		StartCoroutine(Async.Interval(callback, seconds, startInSeconds));
	}
}
