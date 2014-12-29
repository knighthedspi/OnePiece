using UnityEngine;
using System.Collections;

public class AdsManager : MonoBehaviour {

	public const string PUBLISHER_ID = "35058";
	public const string MEDIA_ID = "135913";
	public const string FOOTER_SPOT_ID = "344943";

	public static int idAdsBanner = -1;
	public static bool isShowing = false;
	// Use this for initialization
	void Awake () {
		#if UNITY_IPHONE || UNITY_ANDROID
		IMobileSdkAdsUnityPlugin.registerInline(PUBLISHER_ID,MEDIA_ID,FOOTER_SPOT_ID);
		IMobileSdkAdsUnityPlugin.start(FOOTER_SPOT_ID);
		// 表示位置を座標で指定する場合
//		IMobileSdkAdsUnityPlugin.show(FOOTER_SPOT_ID, IMobileSdkAdsUnityPlugin.AdType.BANNER, left, top);
		// 表示位置を定数パラメータで指定する場合 以下の場合は、画面中央下部に表示されます
		idAdsBanner = IMobileSdkAdsUnityPlugin.show(FOOTER_SPOT_ID, IMobileSdkAdsUnityPlugin.AdType.BANNER,IMobileSdkAdsUnityPlugin.AdAlignPosition.CENTER, 
		                              IMobileSdkAdsUnityPlugin.AdValignPosition.BOTTOM);
		isShowing = true;
		#endif
	}
	void Start()
	{
		Debug.Log("Ads manager");
	}
	// Update is called once per frame
	void Update () {
		if(ViewLoader.Instance.CurrentView == null)
			return;
		if(idAdsBanner != -1)
		{
			if(ViewLoader.Instance.CurrentView.name == Config.GAME_PLAY_VIEW)
			{
				if(isShowing)
				{
					IMobileSdkAdsUnityPlugin.setVisibility(idAdsBanner,false);
					isShowing = false;
				}
			}
			else
			{
				if(!isShowing)
				{
					IMobileSdkAdsUnityPlugin.setVisibility(idAdsBanner,true);
					isShowing = true;
				}
			}
		}
	}
}
