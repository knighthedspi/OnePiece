using UnityEngine;
using System.Collections;

public class DialogResult : DialogBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void Create(int score, int bonusScore, int highScore, int userBelly, int belly, int level, float expFillAmount, EventDelegate.Callback callBack)
    {
        Debug.LogError(bonusScore);
        DialogData dialogData = new DialogData ();
        dialogData.dialogType = DialogType.DialogResult;
//        BeriLbl
        //#TODO exp
        dialogData.textData.Add("BellyLbl", userBelly.ToString());
        dialogData.textData.Add("ScoreLbl", score.ToString());
        dialogData.textData.Add("BestScoreLbl", "High Score: " + highScore.ToString());
        dialogData.textData.Add("BonusScore", bonusScore.ToString());
        dialogData.textData.Add("Belly", belly.ToString());
        //#TODO lam sao de add float?
        dialogData.textData.Add("Level_guage", expFillAmount.ToString());
        dialogData.eventData.Add ("CloseBtn", callBack);
        DialogManager.Instance.OpenDialog (dialogData);
    }
}
