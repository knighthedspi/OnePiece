using UnityEngine;
using System.Collections;
/// <summary>
/// Class for playgame view setup
/// </summary>
public class OPGameSetup  {
	public 		Vector2 		blockNum 			{ set; get;}
	public 		Vector2 		blockSize 			{ set; get;}
	public 		Vector2			blockMargin 		{ set; get;}
	public      Vector2 		boardPadding		{ set; get;}

	public      float 			stage_time 			{ set; get;}
	public      float 			hintTime 			{ set; get;}
	public 		int 			scoreRatio1 		{ set; get;}
	public 		int 			scoreRatio2 		{ set; get;}
	public 		float 			comboStepTime 		{ set; get;}
	public 		int 			feverLimit 			{ set; get;}
	public 		float 			feverStepTime 		{ set; get;}
	public 		float 			deltaMonsterPos 	{ set; get;}
	public 		float 			deltaStartX 		{ set; get;}
	public 		float 			deltaStartY			{ set; get;}
}
