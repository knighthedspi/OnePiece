using UnityEngine;
using System.Collections;

public class DialogOneButton : DialogBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void InitUI ()
	{
		base.InitUI ();
	}

	public static void Create(string content, EventDelegate.Callback callBack,string title= "", string button_label = "")
	{
		DialogData dialogData = new DialogData ();
		dialogData.dialogType = DialogType.DialogOneButton;
		dialogData.textData.Add("content", content);
		dialogData.textData.Add ("title",title);
		dialogData.eventData.Add ("btnCenter", callBack);
		if(!string.IsNullOrEmpty(button_label))
			dialogData.textData.Add ("button_label_btnCenter",button_label);
		DialogManager.Instance.OpenDialog (dialogData, true);
	}
}
