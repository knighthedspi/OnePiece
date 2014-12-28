using UnityEngine;
using System.Collections;

public class DialogBonus : DialogBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void Create(string content, EventDelegate.Callback callBack,string title= "", string button_label = "")
	{
		DialogData dialogData = new DialogData ();
		dialogData.dialogType = DialogType.DialogBonus;
		dialogData.textData.Add("content", content);
		dialogData.textData.Add ("title",title);
		dialogData.eventData.Add ("CloseBtn", callBack);
		DialogManager.Instance.OpenDialog (dialogData);
	}
}
