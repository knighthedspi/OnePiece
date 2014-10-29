using UnityEngine;
using System.Collections;

public class GamePlayView : View {

	public GameObject pauseBtn;

	[HideInInspector]
	public UI 		  UI {get; private set;}

	private bool isPaused;

	// Use this for initialization
	void Start () {
		UI = gameObject.AddComponent<UI>();
		UI.AttachButton(pauseBtn, onPauseBtnClicked);
		isPaused = false;
	}

	// handle click event on Pause Button
	private void onPauseBtnClicked(){
		isPaused = !isPaused;
		Debug.Log("check pause: " + isPaused);
		if(isPaused)
			Time.timeScale = 0.0f;
		else
			Time.timeScale = 1.0f;
	}
}
