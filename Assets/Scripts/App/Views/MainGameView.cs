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
    private string BGM = "bgm_06_full_dream_box";

	[HideInInspector]
	public UI 		  UI {get; private set;}

	// init in awake funtion
	protected override void Awake(){
		base.Awake();

		UI = gameObject.AddComponent<UI>();
		UI.AttachButton(StartBtn, OnStartBtnClicked);
        UI.AttachButton(InviteBtn, OnInviteBtnClick);
        UI.AttachButton(SoundSettingBtn, OnSSBtnClick);
		var viewNames = new Dictionary<string, object []>();
		viewNames.Add(Config.GLOBAL_VIEW, null);
		ViewLoader.Instance.CleanLoad(viewNames, true);

	}

	protected override void Start(){
		base.Start();
        user = AppManager.Instance.user;
        BeriLbl.text = user.Belly.ToString(); 
        // Reports that the user is viewing the Main Menu
		ChangeSpriteSoundBtn ();
        if (GoogleAnalytics.instance)
            GoogleAnalytics.instance.LogScreen("MainGameView");
	}

	void Update()
	{
		if(LevelGuage.fillAmount == 0)
		{
			LevelGuage.fillAmount = LevelService.Instance.fillAmount(user);
		}
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

		var viewNames = new Dictionary<string, object []>();
		viewNames.Add(Config.GLOBAL_VIEW, null);
		viewNames.Add(Config.GAME_PLAY_VIEW, null);
		ViewLoader.Instance.CleanLoad(viewNames, true);


	}

    private void OnSSBtnClick()
    {
		UIButton btn = SoundSettingBtn.GetComponentInChildren<UIButton>();
		SoundManager.isSound = !SoundManager.isSound;
		PlayerPrefs.SetInt("isSound",SoundManager.isSound ? 1 : 0 );
		if (SoundManager.isSound)
		{
			SoundManager.Instance.PlayBGM(BGM);
		}
		else
		{
			SoundManager.Instance.StopBGM();
		}
		ChangeSpriteSoundBtn();
    }

	public void ChangeSpriteSoundBtn ()
	{
		UIButton btn = SoundSettingBtn.GetComponentInChildren<UIButton>();
		if (SoundManager.isSound)
		{

			btn.normalSprite = "sound";
			btn.hoverSprite = "soundpressing";
			btn.pressedSprite = "soundpressed";
		}
		else
		{
			btn.normalSprite = "mute";
			btn.hoverSprite = "mutepressing";
            btn.pressedSprite = "mutepressed";
        }
	}

	protected override void OnOpen (params object[] parameters)
	{
        SoundManager.Instance.PlayBGM(BGM);
	}
}
