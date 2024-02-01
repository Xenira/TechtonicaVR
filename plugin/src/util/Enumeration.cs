using System;
using System.Collections.Generic;

namespace TechtonicaVR.Util;

public static class Enumeration
{
	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (var item in source)
		{
			action(item);
		}
	}

	public static IEnumerable<T> Inspect<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (var item in source)
		{
			action(item);
			yield return item;
		}
	}
}
