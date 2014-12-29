using UnityEngine;
using System.Collections;
using System;

public class DialogResult : DialogBase {

	public NumberLabel numberLabel;
	public UISprite	   levelGause;	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void InitUI ()
	{
		base.InitUI ();
		int score = (!dialogData.textData.ContainsKey ("ScoreLbl")) ? 0 : Int32.Parse (dialogData.textData ["ScoreLbl"]);
		Debug.Log("Result-----------------" + score.ToString());
		numberLabel.setNumberTo (score);
		float fillLevel = (!dialogData.textData.ContainsKey ("Level_guage")) ? 0 : float.Parse (dialogData.textData ["Level_guage"]);
		levelGause.fillAmount = fillLevel;
	}

    public static void Create(int score, int bonusScore, int highScore, int userBelly, int belly, int level, float expFillAmount, EventDelegate.Callback callBack)
    {
        DialogData dialogData = new DialogData ();
        dialogData.dialogType = DialogType.DialogResult;
//        BeriLbl
        //#TODO exp
        dialogData.textData.Add("BellyLbl", userBelly.ToString());
        dialogData.textData.Add("ScoreLbl", score.ToString());
        dialogData.textData.Add("BestScoreLbl", "High Score: " + highScore.ToString());
        dialogData.textData.Add("BonusScore", bonusScore.ToString());
        dialogData.textData.Add("Belly", belly.ToString());
		dialogData.textData.Add("LevelLbl", level.ToString());
        //#TODO lam sao de add float?
        dialogData.textData.Add("Level_guage", expFillAmount.ToString());
        dialogData.eventData.Add ("CloseBtn", callBack);
        DialogManager.Instance.OpenDialog (dialogData);
    }
}
