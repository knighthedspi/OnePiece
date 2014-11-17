using UnityEngine;
using System.Collections.Generic;
using System.Reflection;


public class OnePieceView : View{

	private LoadingView _loadingView;
	
	protected override void OnBeforeViewLoad (string viewName, params object[] parameters)
	{
		OPDebug.Log("Load " + viewName );
		if(_loadingView == null )
			_loadingView = ViewManager.Instance.loadingView;
		_loadingView.gameObject.SetActive(true);
		_loadingView.setLoadingProgress(0.0f);
	}

	protected override void OnViewLoaded (string viewName, params object[] parameters)
	{
		OPDebug.Log(viewName + " is loaded");
		_loadingView.setLoadingProgress(100.0f);

		// TODO : test loading progress , calculate exactly time to transition in
		CoroutineUtility.Instance.WaitAndExecute(2.0f, transitionIn);

	}

	// TODO : test show loading progress
	private void transitionIn(){
		_loadingView.gameObject.SetActive(false);
	}

}

