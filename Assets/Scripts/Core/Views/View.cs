using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public abstract class View : MonoBehaviour {

	public AudioListener audioListener;
	public bool          Opened {get; private set;}	
	public bool          Closed {get; private set;}

	private static Stack<string> history = new Stack<string>();

	protected virtual void Awake() {
		ViewLoader.Instance.BeforeLoadView += HandleBeforeLoadView;
		ViewLoader.Instance.OnViewLoaded += HandleOnViewLoaded;
	}

	void HandleOnViewLoaded (string viewName, params object[] parameters)
	{
		OnViewLoaded(viewName, parameters);
		ViewLoader.Instance.BeforeLoadView -= HandleBeforeLoadView;
		ViewLoader.Instance.OnViewLoaded -= HandleOnViewLoaded;
	}

	void HandleBeforeLoadView (string viewName, params object[] parameters)
	{
		OnBeforeViewLoad(viewName, parameters);
	}

	protected virtual void Start() {
        if(audioListener != null) return;
        audioListener = GetComponentInChildren<AudioListener>();
    }

	public static void PushHistory(string page) {
		history.Push(page);
	}

	public static string PopHistory() {
		if(history.Count == 0) 
			return null;
		return history.Pop();
	}

	public static string PeekHistory() {
		if(history.Count == 0) 
			return null;
		return history.Peek();
	}

	public static void ClearHistory() {
		history.Clear();
	}
	
    public void Open(params object[] parameters) {
        Opened = true;
        Closed = false;
        OnOpen(parameters);
        if(audioListener == null) return;
        SoundManager.Instance.OnEnableAudioListener(audioListener);
    }
	
    public void Close() {
        OnClose();
        Closed = true;
        Opened = false;
        if(audioListener == null) return;
        SoundManager.Instance.OnDisableAudioListener(audioListener);
    }
	
    public void Suspend() {
        OnSuspend();
        if(audioListener == null) return;
        SoundManager.Instance.OnDisableAudioListener(audioListener);
    }
	
    public void Resume(params object[] parameters) {OnResume(parameters);}
	
    protected virtual void OnOpen(params object[] parameters){}
    protected virtual void OnClose(){}
    protected virtual void OnSuspend(){}
	protected virtual void OnResume(params object[] parameters){}
	protected virtual void OnBeforeViewLoad(string viewName, params object[] parameters){}
	protected virtual void OnViewLoaded(string viewName, params object[] parameters){}
}

