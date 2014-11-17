using UnityEngine;
using System.Collections;

public class LoadingView : MonoBehaviour{

	public UISprite loadingProgressBar;
	
	public float loadingProgress { get;set;} 
	
	// Update is called once per frame
	void FixedUpdate () {
		loadingProgressBar.fillAmount = loadingProgress;
	}

	public void setLoadingProgress(float progress){
		loadingProgress = progress;
	}
}
