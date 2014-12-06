using UnityEngine;
using System.Collections;

public class TimeUpController : MonoBehaviour {
	public delegate void OnFinished();
	public OnFinished Finish;

	public void OnFinish()
	{
		Finish();
	}
}
