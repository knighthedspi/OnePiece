using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;


public class OnePieceView : View{

	private LoadingView _loadingView;

	// fix loading time, should be modified later 
	protected float loadingTime = 0.0f;

	// load event handler, wait until action done then transit view, should be overrided
	protected virtual IEnumerator loadEventHandler() { 
		yield return null;
	}
	
	protected override void OnBeforeViewLoad (string viewName, params object[] parameters)
	{
		OPDebug.Log("Load " + viewName );
		if(_loadingView == null )
			_loadingView = ViewManager.Instance.loadingView;
		_loadingView.transitionIn();
	}

	protected override void OnViewLoaded (string viewName, params object[] parameters)
	{
		OPDebug.Log(viewName + " is loaded");
		if(loadingTime > 0.0f)
			CoroutineUtility.Instance.WaitAndExecute(loadingTime, transitionOut);
		CoroutineUtility.Instance.WaitUntilDoneThenExecute( loadEventHandler(), transitionOut);
	}

	private void transitionOut(){
		_loadingView.transitionOut();
	}

}

