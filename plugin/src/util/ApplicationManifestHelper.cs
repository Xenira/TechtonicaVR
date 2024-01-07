using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Valve.Newtonsoft.Json;
using Valve.VR;

namespace TechtonicaVR.Util;

public static class ApplicationManifestHelper
{
	private static PluginLogger Logger = new PluginLogger(typeof(ApplicationManifestHelper));

	public static void UpdateManifest(string manifestPath, string appKey, string imagePath, string name, string description, int steamAppId = 0, bool steamBuild = false)
	{
		try
		{
			var launchType = steamBuild ? GetSteamLaunchString(steamAppId) : GetBinaryLaunchString();
			Logger.LogDebug("Launch Type: " + launchType);
			var appManifestContent = $@"{{
                                            ""source"": ""builtin"",
                                            ""applications"": [{{
                                                ""app_key"": {JsonConvert.ToString(appKey)},
                                                ""image_path"": {JsonConvert.ToString(imagePath)},
                                                {launchType}
                                                ""last_played_time"":""{CurrentUnixTimestamp()}"",
                                                ""strings"": {{
                                                    ""en_us"": {{
                                                        ""name"": {JsonConvert.ToString(name)}
                                                    }}
                                                }}
                                            }}]
                                        }}";

			Logger.LogDebug("Writing manifest");
			File.WriteAllText(manifestPath, appManifestContent);

			Logger.LogDebug("Adding AppManifest");
			var error = OpenVR.Applications.AddApplicationManifest(manifestPath, false);
			if (error != EVRApplicationError.None)
			{
				Logger.LogError("Failed to set AppManifest " + error);
			}

			var processId = System.Diagnostics.Process.GetCurrentProcess().Id;
			Logger.LogDebug("Identifying application");
			var applicationIdentifyErr = OpenVR.Applications.IdentifyApplication((uint)processId, appKey);
			if (applicationIdentifyErr != EVRApplicationError.None)
			{
				Logger.LogError("Error identifying application: " + applicationIdentifyErr);
			}
		}
		catch (Exception exception)
		{
			Logger.LogError("Error updating AppManifest: " + exception);
		}
	}

	private static string GetSteamLaunchString(int steamAppId)
	{
		return $@"""launch_type"": ""url"",
                      ""url"": ""steam://launch/{steamAppId}/VR"",";
	}

	private static string GetBinaryLaunchString()
	{
		var workingDir = Directory.GetCurrentDirectory();
		var executablePath = Assembly.GetExecutingAssembly().Location;
		return $@"""launch_type"": ""binary"",
                      ""binary_path_windows"": {JsonConvert.ToString(executablePath)},
                      ""working_directory"": {JsonConvert.ToString(workingDir)},";
	}

	private static long CurrentUnixTimestamp()
	{
		var foo = DateTime.Now;
		return ((DateTimeOffset)foo).ToUnixTimeSeconds();
	}
}
