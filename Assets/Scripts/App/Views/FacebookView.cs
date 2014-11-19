using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FacebookView : OnePieceView {	
    public GameObject loginFacebook;
    public GameObject startGame;

    [HideInInspector]
    public UI 		  UI { get; private set; }
	
    private bool isPaused;
	
    // Use this for initialization
    void Start()
    {
        UI = gameObject.AddComponent<UI>();
        UI.AttachButton(loginFacebook, onFBBtnClicked);
        UI.AttachButton(startGame, onStartGameClick);

        Transform tDialog = transform.Find("TestDialog");
        if(tDialog) {
            UI.AttachButton(tDialog.gameObject, TestDialogClick);
        }

        //#TODO Init User right place
        OPUser user = OPUserDAO.Instance.getCurrentUser();
        AppManager.Instance.User = user;
    }
	
    // handle click event on Pause Button
    private void onFBBtnClicked()
    {
        Debug.Log("isInit: " + isInit);
        if(!isInit) {
            Debug.Log("Call FB Init");
            CallFBInit();
        }
    }

	private void onStartGameClick()
	{
		Debug.Log ("start game");
		var viewNames = new Dictionary<string, object []>();
		viewNames.Add(Config.GLOBAL_VIEW, null);
		viewNames.Add(Config.START_VIEW, null);
		ViewLoader.Instance.CleanLoad(viewNames);

	}
	

    public void TestDialogClick()
    {
        DialogOneButton.Create("Test Dialog", OnOkClick, "Title", "_OK");
    }

    public void OnOkClick()
    {
        Debug.Log("OK--------- con de");
        DialogManager.Instance.Complete();
    }

	#region FB.Init() example
	
    private bool isInit = false;
	
    private void CallFBInit()
    {
        FB.Init(OnInitComplete, OnHideUnity);
    }
	
    private void OnInitComplete()
    {
        Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
        isInit = true;
        if(!FB.IsLoggedIn) {
            Debug.Log("call FB Login");
            CallFBLogin();
        }
    }
	
    private void OnHideUnity(bool isGameShown)
    {
        Debug.Log("Is game showing? " + isGameShown);
    }
	
	#endregion
	
	#region FB.Login() example
	
    private void CallFBLogin()
    {
        FB.Login("email,publish_actions", LoginCallback);
    }
	
    void LoginCallback(FBResult result)
    {
        Debug.Log(result.Text);
        string lastResponse;
        if(result.Error != null)
            lastResponse = "Error Response:\n" + result.Error;
        else if(!FB.IsLoggedIn) {
            lastResponse = "Login cancelled by Player";
        } else {
            lastResponse = "Login was successful!";
        }
        Debug.Log(lastResponse);
    }
	
    private void CallFBLogout()
    {
        FB.Logout();
    }

	#endregion

}