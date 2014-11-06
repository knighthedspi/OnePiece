using UnityEngine;

public enum DebugOptions
{
	LOCAL =1,
	SANDBOX = 2,
	STAGING = 3,
	HONBAN = 4,
}

public class OPDebug{
	public static DebugOptions MODE = DebugOptions.LOCAL;

	/// <summary>
	/// /Log
	/// </summary>
	/// <param name="obj">Object.</param>
	public static void Log(object obj)
	{
		if(MODE == DebugOptions.LOCAL)
		{
			Debug.Log(obj);
		}
	}

	/// <summary>
	/// Log error
	/// </summary>
	/// <param name="obj">Object.</param>
	public static void LogError(object obj)
	{
		if(MODE == DebugOptions.LOCAL)
		{
			Debug.LogError(obj);
		}
	}

	/// <summary>
	/// Logs the warn.
	/// </summary>
	/// <param name="obj">Object.</param>
	public static void LogWarn(object obj)
	{
		if(MODE == DebugOptions.LOCAL)
		{
			Debug.LogWarning(obj);
		}
	}
}
