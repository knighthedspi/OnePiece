using UnityEngine;
using System.Collections;
using System;

public class SpecialBlock : Block {
	private bool _isActive;
	private SpecialBlockDetail _detail;

	public override void Init (BlockType type)
	{
		base.Init (type);
		string typeStr = blockType.ToString ();
		// upper the fisrt letter
		typeStr = char.ToUpper (typeStr [0]) + typeStr.Substring (1);
		Debug.Log ("type      " + typeStr.ToString ());
		_detail = gameObject.AddComponent (typeStr) as SpecialBlockDetail;
	}
	public override void TouchDown ()
	{
		Debug.Log (_detail);
		_detail.StartAnim ();
	}

	public override void TouchUp ()
	{
		base.TouchUp ();
	}



}
