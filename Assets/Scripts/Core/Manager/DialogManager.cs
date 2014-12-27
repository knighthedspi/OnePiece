using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogManager : Singleton<DialogManager> {

	private bool _isProcessing;
	private GameObject _dialogWindow;
	private UIRoot _uiRoot;

	private DialogBase _currentDialog; 
	private List<DialogData> _dialogQueue = new List<DialogData>();

	void Awake()
	{

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Complete this instance.
	/// </summary>
	public void Complete()
	{
		_isProcessing = false;
		Close ();
	}

	/// <summary>
	/// Opens the dialog.
	/// </summary>
	/// <param name="dialogData">Dialog data.</param>
	/// <param name="immediate">If set to <c>true</c> immediate.</param>
	public void OpenDialog(DialogData dialogData, bool immediate = false)
	{
		if(immediate)
		{
			CreateDialog(dialogData);
		}
		else
		{
			_dialogQueue.Add(dialogData);
			ProcessDialog();
		}
	}


	private void Close()
	{
		if(_currentDialog)
		{
			_currentDialog.Close();
		}
		ProcessDialog ();
	}

	private void ProcessDialog()
	{
		if (_isProcessing)
						return;
		if (_dialogQueue.Count <= 0)
		{
			UIManager.Instance.disable = false;
			return;
		}
		UIManager.Instance.disable = true;	
		CreateDialog (_dialogQueue [0]);
		_dialogQueue.RemoveAt (0);
	}

	private void CreateDialog(DialogData dialogData)
	{
		_isProcessing = true;
		string path = string.Format(Config.DIALOG_RESOURCE_PREFIX +  "{0}", dialogData.dialogType.ToString ());
		GameObject dialogPrb = Resources.Load (path, typeof(GameObject)) as GameObject;
		GameObject parent = GetDialogWindow ();
		GameObject go = NGUITools.AddChild (parent, dialogPrb);
		NGUITools.BringForward (go);

		_currentDialog = go.GetComponent<DialogBase> ();
		_currentDialog.Init (dialogData);

	}

	private GameObject GetDialogWindow()
	{
		if(!_dialogWindow)
		{
			OPDebug.Log("add dialog window");
			GameObject root = ViewManager.Instance.globalViewObject;
			NGUITools.BringForward(root);
			_dialogWindow = NGUITools.AddChild(root);
            _dialogWindow.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			_dialogWindow.name = "DialogWindow";
		}
		return _dialogWindow;
	}

}
