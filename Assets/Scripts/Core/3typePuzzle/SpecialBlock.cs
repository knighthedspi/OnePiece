using UnityEngine;
using System.Collections;
using System;

public class SpecialBlock : Block {
	private bool _isActive;

	public override void Init (BlockType type)
	{
		base.Init (type);
		_isActive = false;
		string typeStr = blockType.ToString ();
		// upper the fisrt letter
//		typeStr = char.ToUpper (typeStr [0] + typeStr.Substring (1));
		gameObject.AddComponent (typeStr);
	}
	public override void TouchDown ()
	{
		SendMessage ("StartAnimation");
	}

	public override void TouchUp ()
	{
		base.TouchUp ();
		_isActive = true;
	}

	public override void Destroy ()
	{
		if(_isActive)
			base.Destroy ();
	}

}
