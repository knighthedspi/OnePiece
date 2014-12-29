using UnityEngine;
using System.Collections;

public class SpecialBlockDetail : MonoBehaviour {

	protected Block ownBlock;

	void Awake()
	{
		ownBlock = GetComponent<Block> ();
	}
	public void Init()
	{

	}

	public virtual void StartAnim()
	{
		Debug.Log ("Start anim");
	}
}
