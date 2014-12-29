using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rainbow : SpecialBlockDetail {

	public override void StartAnim ()
	{
		base.StartAnim ();
		Debug.Log ("------- rainbow  anim ------");
		GamePlayService gamePlay = GamePlayService.Instance; 
		gamePlay.updateRainbowBlocks(ownBlock);
	}
}
