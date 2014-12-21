using UnityEngine;
using System.Collections;
using System;

public class SpecialBlock : Block {
	private bool _isActive;
	private SpecialBolckDetail _detail;

	public override void Init (BlockType type)
	{
		base.Init (type);
		_isActive = false;
		string typeStr = blockType.ToString ();
		// upper the fisrt letter
		typeStr = char.ToUpper (typeStr [0]) + typeStr.Substring (1);

		_detail = gameObject.AddComponent (typeStr) as SpecialBolckDetail;
	}
	public override void TouchDown ()
	{
		_detail.StartAnim ();
	}

	public override void TouchUp ()
	{
		base.TouchUp ();
		_isActive = true;
//		_detail.
	}

	public override void Destroy ()
	{
		Debug.Log ("detroys ------- " + _isActive.ToString ());
		if(_isActive)
			base.Destroy ();
	}

}
