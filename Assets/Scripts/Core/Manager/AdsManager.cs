using UnityEngine;
using System.Collections;

public class AdsManager : MonoBehaviour {

	// Use this for initialization
	public static string publishId = "35058";
	public static string mediaId = "135913";
	public static string banner_spotId = "344943";
	void Awake()
	{

	}
	void Start () {
		#if UNITY_IPHONE || UNITY_ANDROID
		IMobileSdkAdsUnityPlugin.registerInline(publishId,mediaId,banner_spotId);
		IMobileSdkAdsUnityPlugin.start(banner_spotId);
		// 表示位置を座標で指定する場合
		//		IMobileSdkAdsUnityPlugin.show("344943", IMobileSdkAdsUnityPlugin.AdType.BANNER, 0, 0);
		// 表示位置を定数パラメータで指定する場合 以下の場合は、画面中央下部に表示されます
		IMobileSdkAdsUnityPlugin.show(banner_spotId, IMobileSdkAdsUnityPlugin.AdType.BANNER,IMobileSdkAdsUnityPlugin.AdAlignPosition.CENTER, IMobileSdkAdsUnityPlugin.AdValignPosition.BOTTOM);
		#endif
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
