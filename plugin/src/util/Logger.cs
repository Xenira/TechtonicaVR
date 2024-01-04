namespace TechtonicaVR.Util;

public class Logger<T>
{
	public void LogInfo(string message)
	{
		Plugin.Logger.LogInfo($"[{typeof(T).Name}] {message}");
	}

	public void LogDebug(string message)
	{
		Plugin.Logger.LogDebug($"[{typeof(T).Name}] {message}");
	}

	public void LogWarning(string message)
	{
		Plugin.Logger.LogWarning($"[{typeof(T).Name}] {message}");
	}

	public void LogError(string message)
	{
		Plugin.Logger.LogError($"[{typeof(T).Name}] {message}");
	}
}
