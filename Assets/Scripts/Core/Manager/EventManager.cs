using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class EventManager : Singleton<EventManager> {

	public void AttachButton(GameObject gobj, EventDelegate.Callback callback){
		var button = gobj.GetComponent<UIButton>();
		if(button == null) button = gobj.AddComponent<UIButton>();
		EventDelegate.Add(button.onClick, callback);
	}

	public void RemoveButton(GameObject gobj, EventDelegate.Callback callback){
		var button = gobj.GetComponent<UIButton>();
		if(button != null) EventDelegate.Remove(button.onClick, callback);
    }

	public void AttachEvents(List<EventDelegate> list, EventDelegate.Callback callback){
		EventDelegate.Add(list, callback);
	}
	
	public void removeEvents(List<EventDelegate> list, EventDelegate.Callback callback){
		EventDelegate.Remove(list, callback);
	}

}



