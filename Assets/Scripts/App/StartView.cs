using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartView : View {

	public GameObject StartBtn;

	[HideInInspector]
	public UI 		  UI {get; private set;}

	// init in awake funtion
	void Awake(){
		UI = gameObject.AddComponent<UI>();
		Debug.Log ("add UI class");
		UI.AttachButton(StartBtn, onStartBtnClicked);
	}

	private void onStartBtnClicked(){
		Debug.Log ("start loading game");
		var viewNames = new Dictionary<string, object[]>();
		viewNames.Add("GamePlay",null);
		ViewLoader.Instance.CleanLoad(viewNames);
	}
}
