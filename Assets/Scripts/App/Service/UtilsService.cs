using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UtilsService{

	public static string appUrlAndroid = "market://details?id=onepiece.vn.onepiecepuzzle";
	public static string appUrliOS = "";
	
	public void rateApp()
	{
		string appUrl = appUrliOS;
#if UNITY_ANDROID
		appUrl = appUrlAndroid;
#endif
		Application.OpenURL(appUrl);
	}

}