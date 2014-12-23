using UnityEngine;
using System.Collections;

public class UIManager : Singleton<UIManager> {

	public GameObject disableObj;
	private bool _isDisable;
	private UIRoot _uiRoot;
	private DisableUI _disableUI;

	public bool disable
	{
		get
		{
			return _isDisable;
		}
		set
		{
			if( _isDisable == value)
				return;
			_isDisable = value;
			if(_isDisable)
			{
				CheckUIRoot();
				GameObject go 	= NGUITools.AddChild(_uiRoot.gameObject,disableObj);
				_disableUI 		= go.GetComponent<DisableUI>();
				NGUITools.BringForward(go);
				_disableUI.Show();
			}
			else
			{
				if(_disableUI != null && !_disableUI.gameObject.Equals(null))
				{
					_disableUI.Hide();
				}
				_disableUI = null;
			}
		}

	}

	void Awake()
	{

	}

	private void CheckUIRoot()
	{
		if (_uiRoot != null)
						return;
		GameObject obj = GameObject.Find ("Global");
		if (obj != null)
			_uiRoot = obj.GetComponent<UIRoot> ();
		else
			Debug.LogError("Has no uiroot global");

	}
}
