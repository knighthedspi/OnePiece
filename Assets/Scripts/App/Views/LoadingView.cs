using UnityEngine;
using System.Collections;

public class LoadingView : View {

	public UISprite loadingProgressBar;

	public float loadingProgress { get;set;} 


	// Update is called once per frame
	void FixedUpdate () {
		loadingProgressBar.fillAmount = loadingProgress;
	}
}
