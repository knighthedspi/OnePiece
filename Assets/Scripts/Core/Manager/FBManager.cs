using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using System;

public enum FBNextFunc
{
	NONE_FUNC,
	SHARE_FUNC,
	INVITE_FUNC
}

public class FBManager : Singleton<FBManager>
{
	
	public static bool isFLogined = false;
	private bool isInit = false;
	private static List<object>                 friends = null;
	private static Dictionary<string, string>   profile = null;
	private static List<object>                 scores = null;
	string meQueryString = "/me?fields=id,first_name,last_name,name";
	private FBNextFunc fbNextFunc = FBNextFunc.NONE_FUNC;
	#region FB.Init()
	public void CallFBInit ()
	{
		AppManager.Instance.user = OPUserDAO.Instance.CreateUserDefault ();
		//			if(CheckInternet())
		//				FB.Init (SetInit, OnHideUnity);
	}
	
	private void SetInit ()
	{
		Debug.Log ("SetInit");
		enabled = true; // "enabled" is a property inherited from MonoBehaviour
		if (FB.IsLoggedIn) {
			Debug.Log ("Already logged in");
			OnLoggedIn ();
		} else {
			Debug.Log ("Not loggin!");
			isFLogined = false;
			AppManager.Instance.user = OPUserDAO.Instance.CreateUserDefault ();
		}
	}
	
	private void OnInitComplete ()
	{
		Debug.Log ("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
		isInit = true;
		if (!FB.IsLoggedIn) {
			Debug.Log ("call FB Login");
			CallFBLogin ();
		}
	}
	
	private void OnHideUnity (bool isGameShown)
	{
		Debug.Log ("Is game showing? " + isGameShown);
	}
	
	#endregion
	
	#region FB.Login()
	
	public void CallFBLogin ()
	{
		FB.Login ("email,publish_actions,user_friends", LoginCallback);
	}
	
	void LoginCallback (FBResult result)
	{
		Debug.Log (result.Text);
		string lastResponse;
		if (result.Error != null)
			lastResponse = "Error Response:\n" + result.Error;
		else if (!FB.IsLoggedIn) {
			lastResponse = "Login cancelled by Player";
		} else {
			OnLoggedIn ();
			lastResponse = "Login was successful!";
		}
		Debug.Log (lastResponse);
	}
	
	void OnLoggedIn ()
	{
		Debug.Log ("Logged in. ID: " + FB.UserId);
		
		// Reqest player info and profile picture                                                                           
		FB.API (meQueryString, Facebook.HttpMethod.GET, APICallback);  
	}
	
	void APICallback (FBResult result)
	{
		FBUtility.Log ("APICallback");
		if (result.Error != null) {
			FBUtility.LogError (result.Error);
			// Let's just try again
			FB.API (meQueryString, Facebook.HttpMethod.GET, APICallback);
			return;
		}
		isFLogined = true;
		Debug.Log ("Text=" + result.Text);
		profile = FBUtility.DeserializeJSONProfile (result.Text);
		Debug.Log (profile ["id"]);
		//create or update user to database
		AppManager.Instance.user = OPUserDAO.Instance.Save (profile);
		switch (fbNextFunc) {
		case FBNextFunc.INVITE_FUNC:
			onChallengeClicked ();
			break;
		case FBNextFunc.SHARE_FUNC:
			Share ();
			break;
		case FBNextFunc.NONE_FUNC:
		default:
			break;
		}
		fbNextFunc = FBNextFunc.NONE_FUNC;
	}
	
	private void CallFBLogout ()
	{
		FB.Logout ();
	}
	
	#endregion
	
	#region score of game
	public void SaveHighScore (int highScore)
	{
		if (!FB.IsLoggedIn)
			return;
		var query = new Dictionary<string, string> ();
		query ["score"] = highScore.ToString ();
		FB.API ("/me/scores", Facebook.HttpMethod.POST, delegate(FBResult r) {
			Debug.Log ("Result: " + r.Text);
		}, query);
	}
	/// <summary>
	/// Gets the high score of 20 friends
	/// </summary>
	/// <returns>The high score.</returns>
	public int GetHighScore ()
	{
		FB.API ("/app/scores?fields=score,user.limit(20)", Facebook.HttpMethod.GET, ScoresCallback);
		return 0;
	}
	
	void ScoresCallback (FBResult result)
	{
		Debug.Log ("ScoresCallback");
		if (result.Error != null) {
			Debug.LogError (result.Error);
			return;
		}
		Debug.Log (result.Text);
	}
	#endregion
	
	#region share
	public void Share ()
	{ 
//		string folderPath = Application.persistentDataPath;
//		DateTime saveNow = DateTime.Now;
//		string filename = "picoonepice.png";
//		string filePath = folderPath+filename;
//
//		if(!System.IO.Directory.Exists(folderPath))
//			System.IO.Directory.CreateDirectory(folderPath);
//		Application.CaptureScreenshot(filePath);
		if (FB.IsLoggedIn) {
			Debug.Log ("share facebook");                                                                                            
			FB.Feed (
				link: "http://gametech.vn/picoonepice.html",
				linkCaption: "Picopiece",               
				linkName: "Ho ho ho! My best score is " + AppManager.Instance.user.HighScore + ". Play with me on Picopiece!",  
				linkDescription : "Are you ready! Lets take an adventure with luffy team!",
//				picture: "File://"+filePath,
//				picture:"http://gametech.vn/wp-content/uploads/2014/12/3DProfiler.png",
				callback: ShareCallBack
			); 
		} else {
			fbNextFunc = FBNextFunc.SHARE_FUNC;
			if(isInit)
				CallFBLogin ();
			else
			{
				FB.Init(CallFBLogin,OnHideUnity);
			}
		}
	}
	
	private void ShareCallBack (FBResult result)
	{
		Debug.Log ("share callback");                                                                                         
		if (result != null) {                                                                                                                          
			var responseObject = Json.Deserialize (result.Text) as Dictionary<string, object>;                                      
			object obj = 0;                                                                                                        
			if (responseObject.TryGetValue ("cancelled", out obj)) {                                                                                                                      
				Debug.Log ("Request cancelled");                                                                                  
			} else if (responseObject.TryGetValue ("id", out obj)) {                
				Debug.Log ("Shared");
				//bonus belly
				AppManager.Instance.user.Belly += Config.BELLY_FB_SHARE;
				OPUserDAO.Instance.Update (AppManager.Instance.user);
			}                                                                                                                      
		}
	}
	#endregion
	#region invite
	//Logined
	public void onChallengeClicked ()
	{ 
		if (FB.IsLoggedIn) {
			FB.AppRequest (
				to: null,
				filters : "",
				excludeIds : null,
				message: "Are you ready! Lets take an adventure with luffy team!",
				title: "Play with me on Picopiece",
				callback: appRequestCallback
				);                                                                                                                
		} else {
			fbNextFunc = FBNextFunc.INVITE_FUNC;
			if(isInit)
				CallFBLogin ();
			else
			{
				FB.Init(CallFBLogin,OnHideUnity);
			}
		}
	}
	private void appRequestCallback (FBResult result)
	{                                                                                                                              
		Debug.Log ("appRequestCallback");                                                                                         
		if (result != null) {                                                                                                                          
			var responseObject = Json.Deserialize (result.Text) as Dictionary<string, object>;                                      
			object obj = 0;                                                                                                        
			if (responseObject.TryGetValue ("cancelled", out obj)) {                                                                                                                      
				Debug.Log ("Request cancelled");                                                                                  
			} else if (responseObject.TryGetValue ("request", out obj)) {                
				Debug.Log ("Request sent");
				//bonus belly
				AppManager.Instance.user.Belly += Config.BELLY_FB_INVITE;
				OPUserDAO.Instance.Update (AppManager.Instance.user);
			}                                                                                                                      
		}                                                                                                                          
	} 
	
	#endregion
	
	#region load picture
	delegate void LoadPictureCallback (Texture texture);
	
	IEnumerator LoadPictureEnumerator (string url, LoadPictureCallback callback)
	{
		WWW www = new WWW (url);
		yield return www;
		callback (www.texture);
	}
	
	void LoadPictureAPI (string url, LoadPictureCallback callback)
	{
		FB.API (url, Facebook.HttpMethod.GET, result =>
		        {
			if (result.Error != null) {
				FBUtility.LogError (result.Error);
				return;
			}
			
			var imageUrl = FBUtility.DeserializePictureURLString (result.Text);
			
			StartCoroutine (LoadPictureEnumerator (imageUrl, callback));
		});
	}
	
	void LoadPictureURL (string url, LoadPictureCallback callback)
	{
		StartCoroutine (LoadPictureEnumerator (url, callback));
		
	}
	
	void MyPictureCallback (Texture texture)
	{
		FBUtility.Log ("MyPictureCallback");
		
		if (texture == null) {
			// Let's just try again
			LoadPictureAPI (FBUtility.GetPictureURL ("me", 128, 128), MyPictureCallback);
			
			return;
		}
		
		//process
	}
	#endregion
}