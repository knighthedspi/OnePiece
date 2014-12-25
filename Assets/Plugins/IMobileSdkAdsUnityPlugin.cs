using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using ImobileSdkAdsUnityPluginUtility;

public class IMobileSdkAdsUnityPlugin : MonoBehaviour {
	
	/// <summary>
	/// 端末の向き
	/// </summary>
	public enum ImobileSdkAdsAdOrientation : int {
		IMOBILESDKADS_AD_ORIENTATION_AUTO,
		IMOBILESDKADS_AD_ORIENTATION_PORTRAIT,
		IMOBILESDKADS_AD_ORIENTATION_LANDSCAPE,
	}
	
	/// <summary>
	/// 水平方向の広告表示位置
	/// </summary>
    public enum AdAlignPosition{
        LEFT,
        CENTER,
        RIGHT
    }
	
	/// <summary>
	/// 垂直方向の広告表示位置
	/// </summary>
    public enum AdValignPosition{
        BOTTOM,
        MIDDLE,
        TOP
    }

	/// <summary>
	/// 広告の種類
	/// </summary>
    public enum AdType{
        ICON,
        BANNER,
        BIG_BANNER,
		TABLET_BANNER,
		TABLET_BIG_BANNER,
        MEDIUM_RECTANGLE,
		BIG_RECTANGLE,
		SKYSCRAPER,
		WIDE_SKYSCRAPER,
		SQUARE,
		SMALL_SQUARE,
		HALFPAGE
    }

	#region Unity Pugin init
	#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void imobileAddObserver_(string gameObjectName);
	[DllImport("__Internal")]
	private static extern void imobileRemoveObserver_(string gameObjectName);
	[DllImport("__Internal")]
	private static extern void imobileRegisterWithPublisherID_(string publisherid, string mediaid, string spotid); 
	[DllImport("__Internal")]
	private static extern void imobileStart_();
	[DllImport("__Internal")]
	private static extern void imobileStop_();
	[DllImport("__Internal")]
	private static extern bool imobileStartBySpotID_(string spotid);
	[DllImport("__Internal")]
	private static extern bool imobileStopBySpotID_(string spotid);
	[DllImport("__Internal")]
	private static extern bool imobileShowBySpotID_(string spotid);
	[DllImport("__Internal")]
	private static extern bool imobileShowBySpotIDWithPositionAndIconParams_(string spotid,
	                                                                         string publisherid,
	                                                                         string mediaid,
	                                                                         int left,
	                                                                         int top,
	                                                                         int width,
	                                                                         int height,
	                                                                         int iconNumber,
	                                                                         int iconViewLayoutWidth,
	                                                                         int iconSize,
	                                                                         bool iconTitleEnable,
	                                                                         int iconTitleFontSize,
	                                                                         string iconTitleFontColor,
	                                                                         int iconTitleOffset,
	                                                                         bool iconTitleShadowEnable,
	                                                                         string iconTitleShadowColor,
	                                                                         int iconTitleShadowDx,
	                                                                         int iconTitleShadowDy,
	                                                                         int adViewId);
	[DllImport("__Internal")]
	private static extern void imobileSetAdOrientation_(ImobileSdkAdsAdOrientation orientation);
	[DllImport("__Internal")]
	private static extern void imobileSetVisibility_(int adViewId, bool visible);
	[DllImport("__Internal")]
	private static extern void imobileSetLegacyIosSdkMode_(bool isLegacyMode);

	#elif UNITY_ANDROID
    private static AndroidJavaClass imobileSdkAdsAndroidPlugin = new AndroidJavaClass("jp.co.imobile.sdkads.android.unity.Plugin");
    #endif
	#endregion

	#region Unity Pugin Function

	/// <summary>
	/// 広告表示の状態通知イベントを受け取るオブジェクトを登録します
	/// </summary>
	/// <param name="gameObjectName">登録するゲームオブジェクト名</param>
	public static void addObserver(string gameObjectName){
		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileAddObserver_(gameObjectName);
		}
		#elif UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("addObserver",gameObjectName);
		}
		#endif
	}
	
	/// <summary>
	/// 広告表示の状態通知イベントを受け取るオブジェクトを解除します
	/// </summary>
	/// <param name="gameObjectName">解除するゲームオブジェクト名</param>
	public static void removeObserver(string gameObjectName){
		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileRemoveObserver_(gameObjectName);
		}
		#elif UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("removeObserver",gameObjectName);
		}
		#endif
	}
	
	/// <summary>
	/// 全画面広告のスポットを登録します
	/// </summary>
	/// <param name="partnerid">パートナーID</param>
	/// <param name="mediaid">メディアID</param>
	/// <param name="spotid">スポットID</param>
	public static void register(string partnerid, string mediaid, string spotid){

		IMobileSpotInfoManager.SetSpotInfo(spotid, partnerid, mediaid);

		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileRegisterWithPublisherID_(partnerid, mediaid, spotid);
			imobileStartBySpotID_(spotid);
		}
		#elif UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android) {
            imobileSdkAdsAndroidPlugin.CallStatic("registerFullScreen", partnerid, mediaid, spotid);
            imobileSdkAdsAndroidPlugin.CallStatic("start", spotid);
        }
		#endif
	}

    /// <summary>
	/// 全画面広告のスポットを登録します
    /// </summary>
    /// <param name="partnerid">Partnerid.</param>
    /// <param name="mediaid">Mediaid.</param>
    /// <param name="spotid">Spotid.</param>
    public static void registerFullScreen(string partnerid, string mediaid, string spotid){
        register (partnerid, mediaid, spotid);
    }

    /// <summary>
    /// インライン広告のスポットを登録します
    /// </summary>
    /// <param name="partnerid">Partnerid.</param>
    /// <param name="mediaid">Mediaid.</param>
    /// <param name="spotid">Spotid.</param>
    public static void registerInline(string partnerid, string mediaid, string spotid){
        IMobileSpotInfoManager.SetSpotInfo(spotid, partnerid, mediaid);
    }
	
	/// <summary>
	/// 登録済みの全ての広告のスポット情報の取得を開始します
	/// </summary>
	public static void start(){
	}
	
	/// <summary>
	/// 登録済みの全ての広告のスポット情報の取得を停止します
	/// </summary>
	public static void stop(){
		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileStop_();
		}
		#elif UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("stop");
		}
		#endif
	}
	
	/// <summary>
	/// 広告のスポット情報の取得を開始します
	/// </summary>
	/// <param name="spotid">スポットID</param>
    public static void start(string spotid){
	}
	
	/// <summary>
	/// 広告のスポット情報の取得を停止します
	/// </summary>
	/// <param name="spotid">スポットID</param>
    public static void stop(string spotid){
		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileStopBySpotID_(spotid); 
		}
		#elif UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("stop", spotid);
		}
		#endif
	}
	
	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
    public static void show(string spotid){
		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileShowBySpotID_(spotid);		
		}
		#elif UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("show", spotid);
		}
		#endif
	}

	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
	/// <param name="adType">AdType</param>
	/// <param name="alignPosition">AdAlignPosition</param>
	/// <param name="valignPosition">AdValignPosition</param>
	public static int show(string spotid, AdType adType, AdAlignPosition alignPosition, AdValignPosition valignPosition) {
		return show (spotid, adType, alignPosition, valignPosition, null);
	}
	
	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
	/// <param name="adType">AdType</param>
	/// <param name="alignPosition">AdAlignPosition</param>
	/// <param name="valignPosition">AdValignPosition</param>
	/// <param name="iconParams">IMobileIconParams</param>
	public static int show(string spotid, AdType adType, AdAlignPosition alignPosition, AdValignPosition valignPosition, IMobileIconParams iconParams) {
		Rect adRect = IMobileSdkAdsViewUtility.getAdRect (alignPosition, valignPosition, adType, iconParams);
		return show (spotid, adType, adRect, iconParams);
	}
	
	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
	/// <param name="adType">AdType</param>
	/// <param name="left">水平方向の広告表示座標</param>
	/// <param name="top">垂直方向の広告表示座標</param>
    public static int show(string spotid, AdType adType, int left, int top){
        return show(spotid, adType, left, top, null);
	}

	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
	/// <param name="adType">AdType</param>
	/// <param name="left">水平方向の広告表示座標</param>
	/// <param name="top">垂直方向の広告表示座標</param>
	/// <param name="iconParams">IMobileIconParams</param>
    public static int show(string spotid, AdType adType, int left, int top, IMobileIconParams iconParams){
		Rect adRect = IMobileSdkAdsViewUtility.getAdRect (left, top, adType, iconParams);
		return show (spotid, adType, adRect, iconParams);
	}
				
	private static int show(string spotid, AdType adType, Rect adRect, IMobileIconParams iconParams){

		iconParams = iconParams ?? new IMobileIconParams();
		
		string partnerId = IMobileSpotInfoManager.GetPartnerId(spotid);
		string mediaId = IMobileSpotInfoManager.GetMediaId(spotid);
		int adViewId = IMobileAdViewIdManager.createId();
		
		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileShowBySpotIDWithPositionAndIconParams_(spotid, 
			                                              partnerId,
			                                              mediaId,
			                                              (int)adRect.x,
			                                              (int)adRect.y,
			                                              (int)adRect.width,
			                                              (int)adRect.height,
			                                              iconParams.iconNumber,
			                                              iconParams.iconViewLayoutWidth,
			                                              iconParams.iconSize,
			                                              iconParams.iconTitleEnable,
			                                              iconParams.iconTitleFontSize,
			                                              iconParams.iconTitleFontColor,
			                                              iconParams.iconTitleOffset,
			                                              iconParams.iconTitleShadowEnable,
			                                              iconParams.iconTitleShadowColor,
			                                              iconParams.iconTitleShadowDx,
			                                              iconParams.iconTitleShadowDy,
			                                              adViewId);
		}
		#elif UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("show", partnerId, mediaId, spotid, adViewId, (int)adRect.x, (int)adRect.y,
			                                      iconParams.iconNumber, 
			                                      iconParams.iconViewLayoutWidth,
												  iconParams.iconSize,
			                                      iconParams.iconTitleEnable,
												  iconParams.iconTitleFontSize,
			                                      iconParams.iconTitleFontColor, 
												  iconParams.iconTitleOffset,
			                                      iconParams.iconTitleShadowEnable,
			                                      iconParams.iconTitleShadowColor,
			                                      iconParams.iconTitleShadowDx,
			                                      iconParams.iconTitleShadowDy
			                                      );
		}
		#endif
		
		return adViewId;
	}
	
	/// <summary>
	/// 広告の表示の向きを設定します
	/// (iPhoneのみ設定可能)
	/// </summary>
	/// <param name="orientation">ImobileSdkAdsAdOrientation</param>
	public static void setAdOrientation(ImobileSdkAdsAdOrientation orientation){
		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileSetAdOrientation_(orientation);
			return;
		}
		#elif UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("setAdOrientation", (int)orientation);
		}
		#endif
	}

	/// <summary>
	/// 広告の表示・非表示の切り替えを行います
	/// </summary>
	/// <param name="adViewId">showメソッドの戻り値として受け取るAdViewId</param>
	/// <param name="visible">表示するかどうか</param>
    public static void setVisibility(int adViewId, bool visible){
		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileSetVisibility_(adViewId, visible);
			return;
		}
        #elif UNITY_ANDROID
        if(Application.platform == RuntimePlatform.Android) {
            imobileSdkAdsAndroidPlugin.CallStatic("setVisibility", adViewId, visible);
        }
		#endif
    }

	/// <summary>
	/// Xcode5でのビルドに対応させる場合に設定します
	/// </summary>
	/// <param name="isLegacyMode">Xcode5でのビルドに対応させるかどうか</param>
	public static void setLegacyIosMode(bool isLegacyMode) {
		#if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileSetLegacyIosSdkMode_(isLegacyMode);
			return;
		}
		#endif
	}


	#endregion
}