using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rainbow : SpecialBlockDetail {

	public override void StartAnim ()
	{
		base.StartAnim ();
		Debug.Log ("------- rainbow  anim ------");
		GamePlayService gamePlay = GamePlayService.Instance; 
		BlockType randType = gamePlay.GetRandType ();
		List<Block> sameType = gamePlay.GetListBlockType(randType);
		foreach(Block block in sameType)
		{
			gamePlay.RemoveBlock(block.posInBoard);




		}
		gamePlay.RemoveBlock (ownBlock.posInBoard);
//		GamePlayService.Instance.fi
//		Debug.LogError (string.Format ("type  {0}    length  {1} ", randType.ToString (), sameType.Count));
	}
}
