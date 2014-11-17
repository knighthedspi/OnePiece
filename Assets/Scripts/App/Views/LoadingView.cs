using UnityEngine;
using System.Collections;

public class LoadingView : MonoBehaviour{

	public UISprite loadingProgressBar;
	
	public float loadingProgress { get;set;} 

	private Animator _loadingAnimator;

	void Awake(){
		_loadingAnimator = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		loadingProgressBar.fillAmount = loadingProgress;
	}

	public void setLoadingProgress(float progress){
		loadingProgress = progress;
	}

	public void transitionIn(){
		this.gameObject.SetActive(true);
		_loadingAnimator.Play(Config.TRANSITION_IN);
	}

	public void transitionOut(){
		_loadingAnimator.Play(Config.TRANSITION_OUT);
		this.gameObject.SetActive(false);
	}
}
