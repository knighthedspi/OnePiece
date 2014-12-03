using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainGameView : OnePieceView {

	public GameObject StartBtn;
    public GameObject InviteBtn;
    public GameObject SoundSettingBtn;
    public UILabel BeriLbl;
    public UISprite LevelGuage;

   
    private OPUser user;


	[HideInInspector]
	public UI 		  UI {get; private set;}

	// init in awake funtion
	protected override void Awake(){
		base.Awake();
		UI = gameObject.AddComponent<UI>();
		UI.AttachButton(StartBtn, OnStartBtnClicked);
        UI.AttachButton(InviteBtn, OnInviteBtnClick);
        UI.AttachButton(SoundSettingBtn, OnSSBtnClick);
	}

	protected override void Start(){
		base.Start();
        user = AppManager.Instance.user;
        BeriLbl.text = user.Belly.ToString();
        LevelGuage.fillAmount = (float)(user.Exp / (user.Exp* 1.2));
	}

	private void OnInviteBtnClick()
	{
//		DialogOneButton.Create ("Test Dialog",OnOkClick,"Title","_OK");
		FBManager.Instance.onChallengeClicked();
	}

	private void OnOkClick()
	{
		Debug.Log("OK--------- con de");
		DialogManager.Instance.Complete ();
	}


	private void OnStartBtnClicked(){
		Debug.Log ("start loading game");
		SoundManager.Instance.PlaySE("sakura_voice_r02");
		ViewLoader.Instance.ReplaceLoad(Config.GAME_PLAY_VIEW, null);
	}

    private void OnSSBtnClick()
    {
        //#TODO setting sound
    }


	protected override void OnOpen (params object[] parameters)
	{
		SoundManager.Instance.PlayBGM("bgm_01_main");
	}
}
