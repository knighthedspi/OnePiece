using UnityEngine;
using System.Collections;

public class ViewManager : Singleton<ViewManager> {

	public LoadingView loadingView {get; private set;}

	void Awake(){
		// add Loading view then set it to invisible
		// TODO : add dialog manager
		GameObject	go = Instantiate (Resources.Load (Config.VIEWS_PREFABS_PATH + "/" + Config.LOADING_VIEW)) as GameObject;
		if (go != null){
			go.name = Config.LOADING_VIEW;
			go.transform.parent = this.gameObject.transform;
			loadingView = go.GetComponent<LoadingView>();
		}else
			OPDebug.LogError("Cannot instantiate " + Config.LOADING_VIEW);
	}

}
