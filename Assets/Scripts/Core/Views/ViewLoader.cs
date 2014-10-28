using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using ViewLoaded = System.Action<string, object[]>;
using System.Linq;

public class ViewLoader : MonoBehaviour{

    public string FirstViewName;
    public string ResourcePath = "Views";
    public GameObject[] ViewPrefabs; 
    private static ViewLoader currentInstance;
    private static bool isFirstScene = true;
    private Dictionary<string, GameObject> viewPrefabDict;
    private Dictionary<string, View> viewCaches;
	public View CurrentView{get; private set;}
    public event ViewLoaded OnViewLoaded;
    
    public static string GetViewName (GameObject prefab){
        return prefab.name;
    }
    public enum ViewLoadType{
        ByName,
        ByPrefab,
    }

    public static ViewLoader Instance {
        get {
            if (currentInstance == null || !currentInstance.gameObject.activeInHierarchy)
                currentInstance = FindObjectOfType (typeof(ViewLoader)) as ViewLoader;

            return currentInstance;
        }
    }

    private void Awake(){
        if (currentInstance == null || !currentInstance.gameObject.activeInHierarchy) {
            currentInstance = this;
        } else {
            Debug.LogError ("[ViewLoader] multi instances must not be active at the same time.", this.gameObject);
        }

        viewPrefabDict = new Dictionary<string, GameObject> ();
        viewCaches = new Dictionary<string, View> ();
        foreach (GameObject prefab in this.ViewPrefabs) {
            viewPrefabDict.Add (GetViewName (prefab), prefab);
        }
    }

    private void Start(){
        if(isFirstScene){
            Load (this.FirstViewName, true);
            isFirstScene = false;
        }
    }

    private void OnDestroy ()
    {
        if (currentInstance == this) {
            currentInstance = null;
        }
        this.OnViewLoaded = null;
    }

    private void Open (object[] parameters){
        ViewLoadType type = (ViewLoadType)parameters [0];
        string firstViewName;
        GameObject firstViewPrefab;
        object[] paramters;
        switch (type) {
        case ViewLoadType.ByName:
            firstViewName = parameters [1] as string;
            paramters = parameters [2] as object[];
            Load (firstViewName, paramters);
            break;
        case ViewLoadType.ByPrefab:
            firstViewPrefab = parameters [1] as GameObject;
            paramters = parameters [2] as object[];
            Load (firstViewPrefab, paramters);
            break;
        }
    }
	
    private void Close(){
        DestoryViewAll();
    }
	
    public void Load (string nextViewName, params object[] parameters){
        StartCoroutine (loadView (nextViewName, null, SwitchMode.Switch, parameters));
    }

    public void Load (GameObject nextViewPrefab, params object[] parameters){
        StartCoroutine (loadView (nextViewPrefab.name, nextViewPrefab, SwitchMode.Switch, parameters));
    }

	public void ReplaceLoad (string nextViewName, params object[] parameters){
		DestoryView(ViewLoader.Instance.CurrentView);
		AddLoad(nextViewName, parameters);
	}

	public void CleanLoad(Dictionary<string, object[]> nextViewNames){
		StartCoroutine(CleanLoadWithCoroutine(nextViewNames, false));
	}

	public void CleanLoad(Dictionary<string, object[]> nextViewNames, bool order){
		StartCoroutine(CleanLoadWithCoroutine(nextViewNames, order));
	}

	private IEnumerator CleanLoadWithCoroutine(Dictionary<string, object[]> nextViewNames, bool order){
		foreach (var x in nextViewNames.Select((Entry, Index) => new { Entry, Index })){
			if(x.Index == 0){
				yield return StartCoroutine (loadView (x.Entry.Key, null, SwitchMode.Clean, x.Entry.Value));
			}else{
				if(order)
					yield return StartCoroutine (loadView (x.Entry.Key, null, SwitchMode.Addition, x.Entry.Value));
				else
					StartCoroutine (loadView (x.Entry.Key, null, SwitchMode.Addition, x.Entry.Value));
			}
		}
	}
	
    public void CleanLoad (string nextViewName, params object[] parameters){
        StartCoroutine (loadView (nextViewName, null, SwitchMode.Clean, parameters));
    }

    public void CleanLoad (GameObject nextViewPrefab, params object[] parameters){
        StartCoroutine (loadView (nextViewPrefab.name, nextViewPrefab, SwitchMode.Clean, parameters));
    }
	
    public void AddLoad (string nextViewName, params object[] parameters){
        StartCoroutine (loadView (nextViewName, null, SwitchMode.Addition, parameters));
    }

    public void AddLoad (GameObject nextViewPrefab, params object[] parameters){
        StartCoroutine (loadView (nextViewPrefab.name, nextViewPrefab, SwitchMode.Addition, parameters));
    }
	
    public bool SetViewActive (string viewName, bool active){
        View instance = null;
        viewCaches.TryGetValue (viewName, out instance);
        if (instance != null) {
            instance.gameObject.SetActive (active);
            if (active) {
                instance.Suspend ();
            } else {
                instance.Resume ();
            }
            return true;
        }
        return false;
    }

    public void SetViewActiveAll (bool active){
        foreach (string key in viewCaches.Keys) {
            View instance = viewCaches [key] as View;
            instance.gameObject.SetActive (active);
            if(active){
                instance.Suspend ();
            }else{
				instance.Resume ();
            }
        }
    }


	public bool DestoryView(View view){
		return DestoryView(view.name);
	}
	
    public bool DestoryView (string viewName){
        View instance = null;
        viewCaches.TryGetValue (viewName, out instance);

		if(instance == null)
			return false;

		instance.Close ();
		Destroy (instance.gameObject);
		viewCaches.Remove (viewName);

		if(ViewLoader.Instance.CurrentView.name.Equals(viewName)){
			viewCaches.TryGetValue (View.PeekHistory(), out instance);
			CurrentView = instance;
		}

        return true;
    }

    public void DestoryViewAll (string donotDestoryView = ""){
        Dictionary<string, View> buffer = new Dictionary<string, View> (viewCaches);
        foreach (string key in buffer.Keys) {
            if (key != donotDestoryView) {
                View instance = viewCaches [key] as View;
                instance.Close ();
                Destroy (instance.gameObject);
                viewCaches.Remove (key);
            }
        }
    }

    public void CloseViewAll (string donotDestoryView = ""){
        Dictionary<string, View> buffer = new Dictionary<string, View> (viewCaches);
        foreach (string key in buffer.Keys) {
            if (key != donotDestoryView) {
                View instance = viewCaches [key] as View;                    
                instance.Suspend ();
                instance.gameObject.SetActive (false);
            }
        }
    }
	
    public bool IsViewActive (string viewName){
        View instance = null;
        viewCaches.TryGetValue (viewName, out instance);
        if (instance == null)
            return false;
        return instance.gameObject.activeSelf;
    }

    enum SwitchMode{
        Switch,
        Addition,
        Clean,
    }
	
    private IEnumerator loadView (string nextViewName, GameObject nextViewPrefab, SwitchMode switchMode, params object[] parameters){
        View nextView = null;
        viewCaches.TryGetValue (nextViewName, out nextView);
        if (CurrentView != null)
            View.PushHistory (CurrentView.name);
		 
        yield return null;

        if (CurrentView != null && !CurrentView.Closed) {
            if (switchMode == SwitchMode.Switch) {
                CloseViewAll (nextViewName);
            } else if (switchMode == SwitchMode.Clean) {
                DestoryViewAll (nextViewName);
                yield return null;
                yield return Resources.UnloadUnusedAssets ();
            }
        }

        yield return null;
        if (nextView != null && nextView != CurrentView) {
			nextView.gameObject.SetActive (true);
            yield return null;
            nextView.Resume ();
        } else if (nextView == null) {
            GameObject go = null;
            if (viewPrefabDict.ContainsKey (nextViewName)) {
                go = Instantiate (viewPrefabDict [nextViewName]) as GameObject;
            } else if (nextViewPrefab != null) {
                go = Instantiate (nextViewPrefab) as GameObject;
			} else if(!string.IsNullOrEmpty(nextViewName)){
                go = Instantiate (Resources.Load (ResourcePath + "/" + nextViewName)) as GameObject;
            }
            if (go != null){
				go.name = nextViewName;
				go.transform.parent = this.gameObject.transform;
				nextView = go.GetComponent<View> ();
				viewCaches.Add (nextViewName, nextView);
			}else{
				Debug.Log ("The new view could not be created!");
			}
        }

        yield return null;

        if (this.OnViewLoaded != null)
            this.OnViewLoaded (nextViewName, parameters);

        yield return null;

		if (nextView != null && !nextView.Opened)
            nextView.Open (parameters);
        CurrentView = nextView;
    }
}
