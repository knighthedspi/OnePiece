using UnityEngine;
using System.Collections;

public class DisableUI : MonoBehaviour {

	public void Show()
	{
		iTweenEvent.GetEvent (gameObject, "Show").Play ();
	}

	public void Hide()
	{
		iTweenEvent.GetEvent (gameObject, "Hide").Play ();
	}

	private void OnCompleteHide()
	{
		Destroy (gameObject);
	}

	void OnDestroy()
	{
		UIManager.Instance.disable = false;
	}
}
