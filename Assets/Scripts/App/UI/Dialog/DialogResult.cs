using UnityEngine;
using System.Collections;

public class DialogResult : DialogBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void Create(int score, int highScore, int bonusExp, int belly, int bonusBelly, float expAmount,EventDelegate.Callback callBack,string title= "", string button_label = "")
    {
        DialogData dialogData = new DialogData ();
        dialogData.dialogType = DialogType.DialogResult;
//        BeriLbl
        //#TODO exp
        dialogData.textData.Add("BeriLbl", belly.ToString());
        dialogData.textData.Add("ScoreLbl", score.ToString());
        dialogData.textData.Add("BestScoreLbl", "High Score: " + highScore.ToString());
        dialogData.textData.Add("BonusBelly", bonusBelly.ToString());
        dialogData.textData.Add("BonusExp", bonusExp.ToString());
        dialogData.textData.Add("Level_guage", expAmount.ToString());
        dialogData.eventData.Add ("CloseBtn", callBack);
        DialogManager.Instance.OpenDialog (dialogData);
    }
}
