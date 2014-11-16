using UnityEngine;
using System.Collections;

public class DialogBase : MonoBehaviour {

	public DialogData dialogData { set; get;}
	private Animator _animator;

	// Use this for initialization
	void Awake()
	{
		_animator = GetComponent<Animator> ();
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public  void Init(DialogData dialog)
	{
		this.dialogData = dialog;
		InitUI ();
		TransitionIn ();
	} 

	public void Close()
	{
		if(!_animator)
		{
			CloseWindow();
		}
		else 
		{
			TransitionOut();
		}

	}

	private void CloseWindow()
	{
		Debug.Log("Close window");
		Destroy(gameObject);
	}

	public virtual void InitUI()
	{
		// init Label
		UILabel[] labels = GetComponentsInChildren<UILabel> ();
		foreach(UILabel lb in labels)
		{
			string nText = lb.gameObject.name;
			if(dialogData.textData.ContainsKey(nText))
			{
				lb.text = dialogData.textData[nText];
			}
		}

		// init button
		UIButton[] buttons = GetComponentsInChildren <UIButton> ();
		foreach( UIButton uiButton in buttons)
		{
			string nButton = uiButton.gameObject.name;

			// add event for button
			if(dialogData.eventData.ContainsKey(nButton))
			{
				EventDelegate.Callback callback = dialogData.eventData[nButton];
				EventDelegate.Add(uiButton.onClick,callback);
			}

			// find button label with prefix "button_" and change label button
			string buttonLabel = "button_label_" + nButton;
			if(dialogData.textData.ContainsKey(buttonLabel))
			{
				UILabel label = uiButton.GetComponentInChildren<UILabel>();
				label.text = dialogData.textData[buttonLabel];
			}
		}

	}

	private void TransitionIn()
	{
		if (!_animator)
						return;
		string transIn = dialogData.textData.ContainsKey("TransitionIn") ? dialogData.textData["TransitionIn"] : "ZoomIn";
		_animator.Play (transIn);
	}

	private void TransitionOut()
	{
		if (!_animator)
						return;
		string transOut = (dialogData.textData.ContainsKey("TransitionOut") ? 
		                   dialogData.textData["TransitionOut"] : "ZoomOut"); 
		_animator.Play(transOut);
	}

	public void TransitionFinish()
	{
		Debug.Log("transition finish");
		if (!_animator)
						return;
		if(_animator.GetCurrentAnimatorStateInfo(0).IsTag("Out"))
		{
			OnTransOutFinish();
		}
		else
		{
			OnTransInFinish();
		}

	}

	protected virtual void OnTransInFinish()
	{

	}

	protected virtual void OnTransOutFinish()
	{
		CloseWindow();
	}


}
