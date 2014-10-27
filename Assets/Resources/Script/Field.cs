using UnityEngine;
using System.Collections;

/// <summary> 
// Game Field. 
/// </summary>
public class Field : MonoBehaviour {
	public delegate void OnFinished();
	public OnFinished Finish;

	//walking Animation Finished.
	public void OnFinish(){
		Finish();
	}
}
