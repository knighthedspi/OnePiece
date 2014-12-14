using UnityEngine;
using System.Collections;
using System;

public class DialogHighScore : DialogBase {

    public NumberLabel numberLabel;
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
		Debug.Log("Here-----------------" + score.ToString());
		numberLabel.setNumberTo (score);
	}

    public static void Create(int score, EventDelegate.Callback callBack,string title= "", string button_label = "")
    {
        DialogData dialogData = new DialogData ();
        dialogData.dialogType = DialogType.DialogHighScore;
		Debug.Log ("sore  " + score.ToString ());
        dialogData.textData.Add("ScoreLbl", score.ToString());
        //        numberLabel.setNumberTo(score);
        dialogData.eventData.Add ("CloseBtn", callBack);
        DialogManager.Instance.OpenDialog (dialogData);
    }
}
