using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary> 
//  Color-Matching Game Puzzle
/// </summary>
public class GameSystem_ColorMatch : MainGameSystem {
	private Block _currentBlock = null;

	// Use this for initialization
	void Start () {
		// super-class(MainGameSystem) start function called
		base.Start();

		_gameState = GameState.GAME_WORK;
	}

	// get tiles position... 
	// how to dispose tiles
	public override Vector2 tilePos(int x,int y){
		if(x >= (int)_tilesNum.x || x < 0)return new Vector2(0,0);
		if(y >= (int)_tilesNum.y || y < 0)return new Vector2(0,0);
		
		Transform trans = _board.transform;
#if(_NGUI_PRO_VERSION_)
		float width = _board.GetComponent<UISprite>().width;
		float height = _board.GetComponent<UISprite>().height;
#else
		float width = trans.localScale.x;
		float height = trans.localScale.y;
#endif		
		return new Vector2(
				(x*_tileSize.x) + trans.localPosition.x - width/2 + _tileSize.x/2 + _boardPadding.x + _tilesMargin.x * x,
				((_tilesNum.y-y-1)*_tileSize.y) + trans.localPosition.y - height/2 + _tileSize.y/2 + _boardPadding.y+ _tilesMargin.y * y
			);
	}

	// find block in block list
	bool findBlock(List<Block> _list,Block _b){
		foreach(Block b in _list){
			if(b == _b)return true;
		}
		return false;
	}

	// testing around tiles are same type?
	// if same type tile, add destroy list. (will be destroy)
	bool infectionBlock(int x,int y,int type,List<Block> destroyList){
		if(findBlock(destroyList,_tiles[x,y])) return false;
		destroyList.Add(_tiles[x,y]);

		if(x - 1 >= 0 && _tiles[x-1,y]._type == type){
			infectionBlock(x-1,y,type,destroyList);
		}
		if(x + 1 < _tilesNum.x && _tiles[x+1,y]._type == type ){
			infectionBlock(x+1,y,type,destroyList);
		}
		if(y - 1 >= 0 && _tiles[x,y-1]._type == type){
			infectionBlock(x,y-1,type,destroyList);
		}
		if(y + 1 < _tilesNum.y && _tiles[x,y+1]._type == type){
			infectionBlock(x,y+1,type,destroyList);
		}
		return true;
	}

	//color matching process
	void colorMatchingProcess(){
		int cx = (int)_currentBlock._posInBoard.x;
		int cy = (int)_currentBlock._posInBoard.y;
		int type = _currentBlock._type;

		List<Block> destroyList = new List<Block>();

		infectionBlock(cx,cy,type,destroyList);
		if( destroyList.Count >= 2 ){
			destroyBlocks(destroyList);
			decreaseTurn();
			increaseCombo();
		}else{
			attackedEffect();
			resetCombo();
		}
	}

	//==================================
	// update functions start
	//==================================
	//update touching or mouse process
	void updateTouchBoard(){
		if(isBlocksMoveToAnim()) return ;

		if (Input.GetMouseButton(0)){ // touch start or mouse clicked

        	Vector3 p = screenTo2DPoint(Input.mousePosition);
			if( intersectNodeToPoint(_board,p) ){
	        	if( _gameState == GameState.GAME_PLAYING ){
	        		Vector2 idx = findIntersectBlock(p);
	        		if(_currentBlock == null){
		        		if(idx.x >= 0){
							Block _b = _tiles[(int)idx.x,(int)idx.y];
							_b.touchDown();

							_currentBlock = _b;
						}
					}
	        	}
	        }

	    }else{  // touch end or mouse release

	    	if(_currentBlock != null){
	    		_currentBlock.touchUp();

        		Vector3 p = screenTo2DPoint(Input.mousePosition);
	    		if(intersectNodeToPoint(_currentBlock.gameObject,p)){
	    			colorMatchingProcess();
	    		}

				_currentBlock = null;
	    	}else{
	    	}
	    	
	    }
	}

	//==================================
	// update functions end
	//==================================
	void Update () {
		if(_gameEnd)return ;
		// super-class(MainGameSystem) Update function called
		base.Update();

		updateTouchBoard();
	}
}
