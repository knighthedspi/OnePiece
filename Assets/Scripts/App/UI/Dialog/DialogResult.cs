using UnityEngine;
using System.Collections;

public class DialogResult : DialogBase {

    public NumberLabel numberLabel;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Create(int score, EventDelegate.Callback callBack,string title= "", string button_label = "")
    {
        DialogData dialogData = new DialogData ();
        dialogData.dialogType = DialogType.DialogResult;
        dialogData.textData.Add("ScoreLbl", score.ToString());
        //        numberLabel.setNumberTo(score);
        dialogData.eventData.Add ("CloseBtn", callBack);
        DialogManager.Instance.OpenDialog (dialogData);
    }
}
