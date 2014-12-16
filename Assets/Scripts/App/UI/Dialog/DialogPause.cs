using UnityEngine;
using System.Collections;

public class DialogPause : DialogBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void Create(EventDelegate.Callback continueAction, EventDelegate.Callback retryAction, EventDelegate.Callback returnMainAction)
	{
		DialogData dialogData = new DialogData ();
		dialogData.dialogType = DialogType.DialogPause;
		dialogData.eventData.Add ("continueBtn", continueAction);
		dialogData.eventData.Add ("returnMainBtn", retryAction);
		dialogData.eventData.Add ("retryBtn", returnMainAction);
		DialogManager.Instance.OpenDialog (dialogData);
	}
}
