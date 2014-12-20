using UnityEngine;
using System.Collections;

public class DialogLevelUp : DialogBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void Create(int level, EventDelegate.Callback callBack)
    {
        DialogData dialogData = new DialogData ();
        dialogData.dialogType = DialogType.DialogLevelUp;
        dialogData.textData.Add("ScoreLbl", "Level " + level.ToString());
        //        numberLabel.setNumberTo(score);
        dialogData.eventData.Add ("CloseBtn", callBack);
        DialogManager.Instance.OpenDialog (dialogData);
    }
}
