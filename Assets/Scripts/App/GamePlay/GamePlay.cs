using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePlay : GameSystem_LinkMatch {

	public int deltaStartX  = 13;
	public int deltaStartY  = -20;

	private bool [,] visited ;

	private List<List<Block>> list_matches;
	

	// Use this for initialization
	public override void Start () {
		base.Start();
		visited = new bool[(int) _tilesNum.x, (int)_tilesNum.y];
		list_matches = new List<List<Block>>();
	}

	// find hint
	public override void FindHint ()
	{
		FindMatches();
		if(list_matches.Count == 0)
			return;
		_hints = list_matches[0];
		foreach(List<Block> matchingSet in list_matches){
			if(matchingSet.Count > _hints.Count)
				_hints = matchingSet;
		}

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

	private bool checkIsSameTypeOfBlock(Block b1, Block b2){
		return (b1._type == b2._type);
	}

	private void Search(int x, int y, Block type, List<Block> matchingSet ){

		if(checkIsSameTypeOfBlock(_tiles[x,y], type)){
			matchingSet.Add(_tiles[x,y]);
			visited[x,y] = true;
			if( x < _tilesNum.x - 1 && !visited[x + 1, y]){
				Search(x + 1, y, type, matchingSet);
			}

			if( y < _tilesNum.y - 1 && !visited[x, y + 1]){
				Search(x , y + 1, type, matchingSet);
			}

			if( x > 0 && !visited[x - 1, y]){
				Search(x - 1, y, type, matchingSet);
			}
			
			if( y > 0 && !visited[x, y - 1]){
				Search(x , y - 1, type, matchingSet);
			}

			if( x % 2 != 0) {
				if( x < _tilesNum.x - 1 && y > 0  && !visited[x + 1, y - 1]){
					Search(x + 1, y - 1, type, matchingSet);
				}

				if( x > 0 && y > 0  && !visited[x - 1, y - 1]){
					Search(x - 1, y - 1, type, matchingSet);
				}
			}else{
				if( x < _tilesNum.x - 1 && y < _tilesNum.y - 1  && !visited[x + 1, y + 1]){
					Search(x + 1, y + 1, type, matchingSet);
				}
				
				if( x > 0 && y < _tilesNum.y - 1  && !visited[x - 1, y + 1]){
					Search(x - 1, y + 1, type, matchingSet);
				}
			}
		}
	}

	private void ClearVisitedMatch(){
		for(int x = 0; x < (int) _tilesNum.x; x ++){
			for(int y = 0; y < (int) _tilesNum.y ; y++){
				visited[x, y] = false;
			}
		}
	}
	

	private void FindMatches(){
		ClearVisitedMatch();
		list_matches.Clear();

		for(int x = 0; x < (int) _tilesNum.x; x++){
			for(int y = 0; y <  (int) _tilesNum.y; y++){
				if(!visited[x,y]){
					visited[x, y] = true;
					List<Block> matchingSet = new List<Block>();
					Search(x, y, _tiles[x,y], matchingSet);
					if(matchingSet.Count > 2){
						list_matches.Add(matchingSet);
					}
				}
			}
		}

	}
	
}
