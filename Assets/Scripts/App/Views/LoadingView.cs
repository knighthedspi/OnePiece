using UnityEngine;
using System.Collections;

public class LoadingView : MonoBehaviour{

	
	public void transitionIn(){
		this.gameObject.SetActive(true);
	}

	public void transitionOut(){
		this.gameObject.SetActive(false);
	}
}
