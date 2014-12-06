using UnityEngine;
using System.Collections;

public class Rainbow : SpecialBolckDetail {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void StartAnim ()
	{
		base.StartAnim ();
		Debug.Log ("------- rainbow  anim ------");
	}
}
