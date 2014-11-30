using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartView : OnePieceView {

	public GameObject StartBtn;

	[HideInInspector]
	public UI 		  UI {get; private set;}

	// init in awake funtion
	protected override void Awake(){
		base.Awake();
		UI = gameObject.AddComponent<UI>();
		Debug.Log ("add UI class");
		UI.AttachButton(StartBtn, onStartBtnClicked);
	}

	protected override void Start(){
		base.Start();
		Transform tDialog = transform.Find ("TestDialog");
		if(tDialog)
		{
			UI.AttachButton(tDialog.gameObject,TestDialogClick);
		}
	}

	private void TestDialogClick()
	{
//		DialogOneButton.Create ("Test Dialog",OnOkClick,"Title","_OK");
		FBManager.Instance.onChallengeClicked();
	}

	private void OnOkClick()
	{
		Debug.Log("OK--------- con de");
		DialogManager.Instance.Complete ();
	}


	private void onStartBtnClicked(){
		Debug.Log ("start loading game");
		SoundManager.Instance.PlaySE("sakura_voice_r02");
		ViewLoader.Instance.ReplaceLoad(Config.GAME_PLAY_VIEW, null);
	}

	protected override void OnOpen (params object[] parameters)
	{
		SoundManager.Instance.PlayBGM("bgm_01_main");
	}
}
