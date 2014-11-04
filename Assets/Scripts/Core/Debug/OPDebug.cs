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
	public static void Log(object obj)
	{
		if(MODE == DebugOptions.LOCAL)
		{
			Debug.Log(obj);
		}
	}
}
