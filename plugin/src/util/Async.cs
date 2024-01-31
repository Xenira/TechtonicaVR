using System;
using System.Collections;
using System.Threading;
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

	public static IEnumerator Interval(Action callback, float seconds, float startInSeconds, int? cnt = null)
	{
		yield return new WaitForSeconds(startInSeconds < 0 ? seconds : startInSeconds);
		while (true)
		{
			callback();

			if (cnt.HasValue)
			{
				if (--cnt <= 0)
				{
					break;
				}
			}

			yield return new WaitForSeconds(seconds);
		}
	}

	public static IEnumerator IntervalFrames(Action callback, int frames, int startInFrames, int? cnt = null)
	{
		for (int i = 0; i < startInFrames; i++)
		{
			yield return new WaitForEndOfFrame();
		}
		while (true)
		{
			callback();

			if (cnt.HasValue)
			{
				if (--cnt <= 0)
				{
					break;
				}
			}

			for (int i = 0; i < frames; i++)
			{
				yield return new WaitForEndOfFrame();
			}
		}
	}
}

public class AsyncGameObject : MonoBehaviour
{
	private static PluginLogger Logger = PluginLogger.GetLogger<AsyncGameObject>();
	private static AsyncGameObject instance;

	private static AsyncGameObject Instance
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

	public static void Timeout(Action callback, float seconds)
	{
		Instance.timeout(callback, seconds);
	}

	private void timeout(Action callback, float seconds)
	{
		StartCoroutine(Async.Timeout(callback, seconds));
	}

	public static void TimeoutFrames(Action callback, int frames)
	{
		Instance.timeoutFrames(callback, frames);
	}

	private void timeoutFrames(Action callback, int frames)
	{
		StartCoroutine(Async.TimeoutFrames(callback, frames));
	}

	public static void Interval(Action callback, float seconds, float startInSeconds, int? cnt = null)
	{
		Instance.interval(callback, seconds, startInSeconds, cnt);
	}

	private void interval(Action callback, float seconds, float startInSeconds, int? cnt = null)
	{
		StartCoroutine(Async.Interval(callback, seconds, startInSeconds, cnt));
	}
}
