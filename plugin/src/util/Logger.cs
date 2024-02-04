using System;
using UnityEngine;

namespace TechtonicaVR.Util;

public class PluginLogger
{
	private string prefix;

	public PluginLogger(string prefix)
	{
		this.prefix = prefix;
	}
	public PluginLogger(Type type)
	{
		prefix = type.FullName;
	}

	public static PluginLogger GetLogger<T>()
	{
		return new PluginLogger(typeof(T));
	}

	public void LogInfo(string message)
	{
		TechtonicaVR.Logger.LogInfo($"[{prefix}] ({Time.frameCount}) {message}");
	}

	public void LogDebug(string message)
	{
		TechtonicaVR.Logger.LogDebug($"[{prefix}] ({Time.frameCount}) {message}");
	}

	public void LogWarning(string message)
	{
		TechtonicaVR.Logger.LogWarning($"[{prefix}] ({Time.frameCount}) {message}");
	}

	public void LogError(string message)
	{
		TechtonicaVR.Logger.LogError($"[{prefix}] ({Time.frameCount}) {message}");
	}

	internal void LogTrace(string v)
	{
		if (!ModConfig.DebugEnabled())
		{
			return;
		}
		TechtonicaVR.Logger.LogDebug($"[{prefix}] ({Time.frameCount}) {v}");
	}
}
