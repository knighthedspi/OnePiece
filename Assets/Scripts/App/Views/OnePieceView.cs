using UnityEngine;
using System.Collections.Generic;
using System.Reflection;


public class OnePieceView : View{

	protected override void OnBeforeViewLoad (string viewName, params object[] parameters)
	{
		OPDebug.Log("Load " + viewName);
		if(!viewName.Equals(Config.LOADING_VIEW) ){
			if(!ViewLoader.Instance.IsViewExist(Config.LOADING_VIEW))
				ViewLoader.Instance.AddLoad(Config.LOADING_VIEW, null);
			else
				ViewLoader.Instance.SetViewActive(Config.LOADING_VIEW, true);
		}
	}

	protected override void OnViewLoaded (string viewName, params object[] parameters)
	{
		OPDebug.Log(viewName + " is loaded");
		if(viewName.Equals(Config.LOADING_VIEW)){
			ViewLoader.Instance.SetViewActive(Config.LOADING_VIEW, false);
		}
	}

}

