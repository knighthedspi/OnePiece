using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class UI : MonoBehaviour {

	private Dictionary<string, string> eventDelegates;

	public void AttachButton(GameObject gobj, EventDelegate.Callback callback){
		var button = gobj.GetComponent<UIButton>();
		if(button == null) button = gobj.AddComponent<UIButton>();
		EventDelegate.Add(button.onClick, callback);
	}

	public void AttachEventTrigger(MonoBehaviour target, Dictionary<string, string[]> eventDelegateDic, GameObject go){
		var trigger = go.GetComponent<UIEventTrigger>();
		if(trigger == null) trigger = go.AddComponent<UIEventTrigger>();
		foreach (KeyValuePair<string, string[]> pair in eventDelegateDic) {
			List<EventDelegate> eventDelegates = new List<EventDelegate>();
			foreach(string methodName in pair.Value){
				EventDelegate eventDelegate = new EventDelegate();
				eventDelegate.target = target;
				eventDelegate.methodName = methodName;
				eventDelegate.parameters[0].obj = go;
				eventDelegates.Add(eventDelegate);
			}
			typeof(UIEventTrigger).InvokeMember(pair.Key,
			                              BindingFlags.SetField,
			                              null,
			                              trigger, 
			                              new object[] {eventDelegates});
		}
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

