using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using System;

public class FBManager : Singleton<FBManager>
{

		public static bool isFLogined = false;
		private bool isInit = false;
		private static List<object>                 friends = null;
		private static Dictionary<string, string>   profile = null;
		private static List<object>                 scores = null;
		string meQueryString = "/me?fields=id,first_name,last_name,name";
	#region FB.Init()
		public void CallFBInit ()
		{
			FB.Init (SetInit, OnHideUnity);
		}
		private void SetInit()
		{
			Debug.Log("SetInit");
			enabled = true; // "enabled" is a property inherited from MonoBehaviour
#if UNITY_EDITOR
		Debug.Log("UNITY_EDITOR");
		Dictionary<string,string> testUser = new Dictionary<string,string>();
		testUser.Add("id","5395743982710432");
		testUser.Add("name","User Test");
		testUser.Add("first_name","User");
		testUser.Add("last_name","Test");
		isFLogined = true;
		AppManager.Instance.user = OPUserDAO.Instance.Save (testUser);
		return;
#endif
			if (FB.IsLoggedIn) 
			{
				Debug.Log("Already logged in");
				OnLoggedIn();
			}
			else
			{
				Debug.Log("Not loggin!");
				isFLogined  = false;
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
						OnLoggedIn();
						lastResponse = "Login was successful!";
				}
				Debug.Log (lastResponse);
		}
		
		void OnLoggedIn ()
		{
				Debug.Log ("Logged in. ID: " + FB.UserId);
			
				// Reqest player info and profile picture                                                                           
				FB.API (meQueryString, Facebook.HttpMethod.GET, APICallback);  
//			LoadPictureAPI(Util.GetPictureURL("me", 128, 128),MyPictureCallback);    
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
				isFLogined  = true;
				Debug.Log ("Text="+result.Text);
				profile = FBUtility.DeserializeJSONProfile (result.Text);
				Debug.Log (profile["id"]);
				//create or update user to database
				AppManager.Instance.user = OPUserDAO.Instance.Save (profile);
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

		public int GetHighScore ()
		{
				FB.API("/app/scores?fields=score,user.limit(20)", Facebook.HttpMethod.GET, ScoresCallback);
				return 0;
		}

		void ScoresCallback(FBResult result) 
		{
			Debug.Log("ScoresCallback");
			if (result.Error != null)
			{
				Debug.LogError(result.Error);
				return;
			}
			Debug.Log(result.Text);
		}
	#endregion
	#region invite
	public void onChallengeClicked()                                                                                              
	{ 
		FB.AppRequest(
			to: null,
			filters : "",
			excludeIds : null,
			message: "Game nay hay vai! Vao choi di cac ban oi :)))",
			title: "Co dam choi Onepice voi tao khong (devil)!",
			callback:appRequestCallback
			);                                                                                                                
		
	}                                                                                                                              
	private void appRequestCallback (FBResult result)                                                                              
	{                                                                                                                              
		Debug.Log("appRequestCallback");                                                                                         
		if (result != null)                                                                                                        
		{                                                                                                                          
			var responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;                                      
			object obj = 0;                                                                                                        
			if (responseObject.TryGetValue ("cancelled", out obj))                                                                 
			{                                                                                                                      
				Debug.Log("Request cancelled");                                                                                  
			}                                                                                                                      
			else if (responseObject.TryGetValue ("request", out obj))                                                              
			{                
//				AddPopupMessage("Request Sent", ChallengeDisplayTime);
				Debug.Log("Request sent");                                                                                       
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
