using System;

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
		TechtonicaVR.Logger.LogInfo($"[{prefix}] {message}");
	}

	public void LogDebug(string message)
	{
		TechtonicaVR.Logger.LogDebug($"[{prefix}] {message}");
	}

	public void LogWarning(string message)
	{
		TechtonicaVR.Logger.LogWarning($"[{prefix}] {message}");
	}

	public void LogError(string message)
	{
		TechtonicaVR.Logger.LogError($"[{prefix}] {message}");
	}
}
