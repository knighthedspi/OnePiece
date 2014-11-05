﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePlay : GameSystem_LinkMatch {

	public int deltaStartX  = 13;
	public int deltaStartY  = -20;
	

	// Use this for initialization
	public override void Start () {
		base.Start();
		GamePlayService.Instance.initBlock(_tilesNum, _tiles);
	}

	// find hint
	public override void FindHint ()
	{
		_hints = GamePlayService.Instance.FindHint();
	}

	// tile pos
	public override Vector2 tilePos(int x,int y){
		if(x >= (int)_tilesNum.x || x < 0)return new Vector2(0,0);
		if(y >= (int)_tilesNum.y || y < 0)return new Vector2(0,0);
		
		float curve = x%2*40;
		
		Transform trans = _board.transform;
		
		float width = _board.GetComponent<UISprite>().width;
		float height = _board.GetComponent<UISprite>().height;
		float startX = trans.localPosition.x - width/2 + deltaStartX;
		float startY = trans.localPosition.y - height/2 + deltaStartY;
		float posX = (x*_tileSize.x);
		float posY = ((_tilesNum.y-y-1)*(_tileSize.y));
		return new Vector2(
			posX + startX + _tileSize.x/2 + _boardPadding.x + _tilesMargin.x * x,
			posY + startY + _tileSize.y/2 + _boardPadding.y + _tilesMargin.y * y + curve
			);
	}
	

}
