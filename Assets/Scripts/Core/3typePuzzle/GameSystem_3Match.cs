using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary> 
//  3-Matching Game Puzzle
/// </summary>
public class GameSystem_3Match : MainGameSystem {
	public enum SpecialBlock{
		MATCHED_4=1,
		MATCHED_5,
		MATCHED_CROSS,
	}

	public GameObject _4Match_BlockPrefab = null;
	public GameObject _5Match_BlockPrefab = null;
	public GameObject _CrossMatch_BlockPrefab = null;

	/////////private variable///////////
	private List<List<Block>> destroyVertical = new List<List<Block>>();
	private List<List<Block>> destroyHorizontal = new List<List<Block>>();

	private List<Block> store = new List<Block>();
	
	private Vector2 _selectedBlock;

	private Vector2 _swapingBlock1;
	private Vector2 _swapingBlock2;
	
	// Use this for initialization
	void Start () {
		// super-class(MainGameSystem) start function called
		base.Start();
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

	// find 3 or more matching tiles
	int checkDestroyBlock(int type,List<Block> store,List<List<Block>> ori,int i,int j){
		if(_tiles[i,j]._type != type){
			if(store.Count >= 3){
				ori.Add(new List<Block>(store));
			}
			type = _tiles[i,j]._type;
			store.Clear();
		}
		store.Add(_tiles[i,j]);
		return type;
	}

	//find block list block belong to list 
	List<Block> findDestroyBlockInList(List<List<Block>> ori,Block b){
		foreach (List<Block> _list in ori){
			foreach (Block _b in _list){
				if(_b == b)return _list;
			}
		}
		return null;
	}

	//checking 2 list is crossed
	Block findCrossBlock(List<Block> v,List<Block> h){
		foreach (Block _vb in v){
			foreach (Block _hb in h){
				if(_vb == _hb){
					return _vb;
				}
			}
		}
		return null;
	}

	//get tile line
	// first argument is line number
	// second argument is axis. ori == true => x-axis, false => y-axis 
	List<Block> findBlockLine(int line,bool ori){
		List<Block> blocks = new List<Block>();
		if(ori){
			for(int x=0;x < _tilesNum.x; x++){
				blocks.Add(_tiles[x,line]);
			}
		}else{
			for(int y=0;y < _tilesNum.y; y++){
				blocks.Add(_tiles[line,y]);
			}
		}
		return blocks;
	}

	// swap position 2 block
	void swapBlock(int i,int j,int ti,int tj){
		Block tb = _tiles[ti,tj];
		_tiles[ti,tj] = _tiles[i,j];
		_tiles[i,j] = tb;

		_tiles[ti,tj]._posInBoard = new Vector2(ti,tj);
		_tiles[i,j]._posInBoard = new Vector2(i,j);

		_tiles[ti,tj].moveTo(tilePos(ti,tj));
		_tiles[i,j].moveTo(tilePos(i,j));

		_swapingBlock1 = new Vector2(ti,tj);
		_swapingBlock2 = new Vector2(i,j);
	}

	//create special tile 
	// 3 more matching or cross matching will be called this function
	void createSpecialBlock(SpecialBlock t, Block b ){
		GameObject _block = null;
		if(t == SpecialBlock.MATCHED_4){
			_block = (GameObject)Instantiate(_4Match_BlockPrefab);
		}else if(t == SpecialBlock.MATCHED_5){
			_block = (GameObject)Instantiate(_5Match_BlockPrefab);
		}else if(t == SpecialBlock.MATCHED_CROSS){
			_block = (GameObject)Instantiate(_CrossMatch_BlockPrefab);
		}else{
			return ;
		}

		_block.transform.parent = _panel.transform;
		_block.transform.localPosition = b.transform.localPosition;
		_block.transform.localScale = b.transform.localScale;

		Block _b = _block.GetComponent<Block>();
		_b._posInBoard = b._posInBoard;
		_b._type = -(int)t;
		_tiles[(int)_b._posInBoard.x,(int)_b._posInBoard.y] = _b;

		Destroy(b.gameObject);
	}


	// destroy blocks. and return how many destroyed blocks count.
	// if now monster turn up, player will attacking to monster!
	// if 3 more matching or cross matching, make special tile
	private int destroyBlocks(List<Block> v, Block except=null,Block focusTo=null,bool useMoreMatch = true){
		int count = 0;
		if(useMoreMatch)
		{
			if(v.Count > 3 && focusTo == null){
				focusTo = v[v.Count-1];
			}
		}

		foreach (Block _b in v){
			if(except == _b) continue;

			_tiles[(int)_b._posInBoard.x,(int)_b._posInBoard.y] = null;
			if(focusTo == null){

				GameObject _p = MakeBlockDestroyParticle();
				_p.transform.parent = _panel.transform;
				_p.SendMessage("generate",_b);

				if( _currentMonster != null){
					PlayerAttackParticle __=(MakePlayerAttackParticle()).GetComponent<PlayerAttackParticle>();
					__.generate(_panel,_b,_b.transform.localPosition,new Vector2(0,300));
					__.Finish = OnFinishedPlayerAttack;
				}

				Destroy(_b.gameObject);	
			}else{
				_b.moveTo(focusTo.transform.localPosition);
				_b._dieAfterAnim = true;
			}
			count++;
		}
		
		if(focusTo && except != focusTo){
			if(v.Count == 4){
				createSpecialBlock(SpecialBlock.MATCHED_4,focusTo);
			}else if(v.Count >= 5){
				createSpecialBlock(SpecialBlock.MATCHED_5,focusTo);
			}
		}

		return count;
	}

	//==================================
	// update functions start
	//==================================

	// 3 matching process!
	void updateThreeMatching(){
		if(isBlocksMoveToAnim()) return ;

		destroyVertical.Clear();
		destroyHorizontal.Clear();
		store.Clear();

		for(int i=0;i < _tilesNum.x ; i++){
			int type=0;
			store.Clear();
			for(int j=0;j < _tilesNum.y ; j++){
				type = checkDestroyBlock(type,store,destroyVertical,i,j);
			}
			if(store.Count >= 3)destroyVertical.Add(new List<Block>(store));
		}
		
		for(int j=0;j < _tilesNum.y ; j++){
			int type=0;
			store.Clear();
			for(int i=0;i < _tilesNum.x ; i++){
				type = checkDestroyBlock(type,store,destroyHorizontal,i,j);
			}
			if(store.Count >= 3)destroyHorizontal.Add(new List<Block>(store));
		}

		List<Block> crossVertical = null;
		List<Block> crossHorizontal = null;
		foreach (List<Block> _list in destroyVertical){
			foreach (Block _b in _list){
				List<Block> ret = findDestroyBlockInList(destroyHorizontal,_b);
				if(ret != null){
					crossVertical = _list;
					crossHorizontal = ret;
					break;
				}
			}
			if(crossVertical != null)break;
		}

		int destroyNum = 0;
		bool anyBlockDestroyed = true;
		if(crossVertical != null){
			Block cross = findCrossBlock(crossVertical,crossHorizontal);
			destroyNum += destroyBlocks(crossVertical  ,cross,cross);
			destroyNum += destroyBlocks(crossHorizontal,cross,cross);

			createSpecialBlock(SpecialBlock.MATCHED_CROSS,cross);
			
		}else if(destroyVertical.Count > 0){
			destroyNum += destroyBlocks(destroyVertical[0],null);
		}else if(destroyHorizontal.Count > 0){
			destroyNum += destroyBlocks(destroyHorizontal[0],null);
		}else{
			anyBlockDestroyed = false;
			if(_gameState == GameState.GAME_ENTRY){
				_gameState = GameState.GAME_WORK;
				_descriptLabel.text = "Go";
#if(_NGUI_PRO_VERSION_)
				_descriptLabel.gameObject.GetComponent<Animator>().Play("ReadyGo_NGUI_Pro");
#else
				_descriptLabel.gameObject.GetComponent<Animator>().Play("ReadyGo");
#endif
			}
		}

		_scoreLabel.addNumberTo(destroyNum*(3*(_currentCombo+1)));

		if(_currentMonster != null){
			if(anyBlockDestroyed){
				increaseCombo();
			}else{
				resetCombo();
			}
		}

		if( _gameState == GameState.GAME_SWAP_BLOCK ){
			if(anyBlockDestroyed == false){
				if(_swapingBlock1.x != -1){
					swapBlock((int)_swapingBlock1.x,(int)_swapingBlock1.y,
							  (int)_swapingBlock2.x,(int)_swapingBlock2.y);
				}
			}else{
				decreaseTurn();
			}
			_gameState = GameState.GAME_PLAYING;
		}
	}

	//update touching or mouse process
	void updateTouchBoard(){
		if(isBlocksMoveToAnim()) return ;

		if (Input.GetMouseButton(0)){ // touch start or mouse clicked

        	Vector3 p = screenTo2DPoint(Input.mousePosition);
			if( intersectNodeToPoint(_board,p) ){
	        	if( _gameState == GameState.GAME_PLAYING ){
	        		Vector2 idx = findIntersectBlock(p);
	        		if(idx.x >= 0){
						_gameState = GameState.GAME_SELECT_BLOCK;
						_selectedBlock = idx;

						Block _b = _tiles[(int)idx.x,(int)idx.y];
						_b.touchDown();
					}
	        	}else if( _gameState == GameState.GAME_SELECT_BLOCK ){
	        		int i = (int)_selectedBlock.x;
	        		int j = (int)_selectedBlock.y;
	        		Block b = _tiles[i,j];
	        		if(!intersectNodeToPoint(b.gameObject,p)){
	        			float dx = p.x - b.transform.localPosition.x;
	        			float dy = p.y - b.transform.localPosition.y;

						b.touchUp();
						int ti=i;
	        			int tj=j;
	        			if(Math.Abs(dx) > Math.Abs(dy)){
	        				if(dx > 0)
	        					ti += 1;
        					else
	        					ti -= 1;
	        			}else{
	        				if(dy < 0)
	        					tj += 1;
        					else
	        					tj -= 1;
	        			}

	        			bool isNotBlock = (ti >= 0 && ti < _tilesNum.x && tj >= 0 && tj < _tilesNum.y);

	        			if(b._type >= 0){
		        			_swapingBlock1 = new Vector2(-1,-1);
			        		_swapingBlock2 = new Vector2(-1,-1);
		        			if( isNotBlock ){
		        				swapBlock(i,j,ti,tj);
							}
		        			_gameState = GameState.GAME_SWAP_BLOCK;
        				}else{
							SpecialBlock type = (SpecialBlock)Math.Abs(b._type);
							if(isNotBlock && type == SpecialBlock.MATCHED_5){
								b.moveTo(_tiles[ti,tj].transform.localPosition);
								b._dieAfterAnim = true;

								_tiles[i,j] = null;
								
								List<Block> f = findBlocksWithType(_tiles[ti,tj]._type);
								destroyBlocks(f,null,null,false);
							}
        				}
	        		}
	        	}
			}

		}else{ // touch end or mouse release

			if(_gameState == GameState.GAME_SELECT_BLOCK){
				Block b = _tiles[(int)_selectedBlock.x,(int)_selectedBlock.y];
				b.touchUp();

				if(b._type < 0){
					SpecialBlock type = (SpecialBlock)Math.Abs(b._type);
					if( type == SpecialBlock.MATCHED_4 ){
						List<Block> blocks = findBlockLine((int)_selectedBlock.x,false);
						destroyBlocks(blocks,null,null,false);
					}else if( type == SpecialBlock.MATCHED_CROSS ){
						List<Block> blocks1 = findBlockLine((int)_selectedBlock.x,false);
						List<Block> blocks2 = findBlockLine((int)_selectedBlock.y,true);

						destroyBlocks(blocks1,b,null,false);
						destroyBlocks(blocks2,null,null,false);
					}
				}
	        	_gameState = GameState.GAME_PLAYING;
			}
			
		}
	}

	bool checkIsSameBlock(Block b1,Block b2, Block b3){
		return b1._type == b2._type && b1._type == b3._type;
	}

	//find hint
	public override void FindHint(){
		for(int x=0;x < _tilesNum.x; x++){
			bool l = x>0;
			bool r = x<_tilesNum.x-1;
			for(int y=0;y < _tilesNum.y; y++){
				bool t = y>0;
				bool b = y<_tilesNum.y-1;
				if( y >=2 ){
					//find hint up
					if(l&&checkIsSameBlock(_tiles[x,y],_tiles[x,y-1],_tiles[x-1,y-2])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x,y-1]);
						_hints.Add(_tiles[x-1,y-2]);
						return ;
					}else if(r&&checkIsSameBlock(_tiles[x,y],_tiles[x,y-1],_tiles[x+1,y-2])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x,y-1]);
						_hints.Add(_tiles[x+1,y-2]);
						return ;
					}else if(r&&checkIsSameBlock(_tiles[x,y],_tiles[x+1,y-1],_tiles[x,y-2])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x+1,y-1]);
						_hints.Add(_tiles[x,y-2]);
						return ;
					}else if(l&&checkIsSameBlock(_tiles[x,y],_tiles[x-1,y-1],_tiles[x,y-2])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x-1,y-1]);
						_hints.Add(_tiles[x,y-2]);
						return ;
					}
				}
				if( y <= _tilesNum.y-3){
					//find hint down
					if(l&&checkIsSameBlock(_tiles[x,y],_tiles[x,y+1],_tiles[x-1,y+2])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x,y+1]);
						_hints.Add(_tiles[x-1,y+2]);
						return ;
					}else if(r&&checkIsSameBlock(_tiles[x,y],_tiles[x,y+1],_tiles[x+1,y+2])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x,y+1]);
						_hints.Add(_tiles[x+1,y+2]);
						return ;
					}else if(r&&checkIsSameBlock(_tiles[x,y],_tiles[x+1,y+1],_tiles[x,y+2])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x+1,y+1]);
						_hints.Add(_tiles[x,y+2]);
						return ;
					}else if(l&&checkIsSameBlock(_tiles[x,y],_tiles[x-1,y+1],_tiles[x,y+2])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x-1,y+1]);
						_hints.Add(_tiles[x,y+2]);
						return ;
					}
				}
				if( x >= 2 ){
					//find hint left
					if(b&&checkIsSameBlock(_tiles[x,y],_tiles[x-1,y],_tiles[x-2,y+1])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x-1,y]);
						_hints.Add(_tiles[x-2,y+1]);
						return ;
					}else if(t&&checkIsSameBlock(_tiles[x,y],_tiles[x-1,y],_tiles[x-2,y-1])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x-1,y]);
						_hints.Add(_tiles[x-2,y-1]);
						return ;
					}else if(t&&checkIsSameBlock(_tiles[x,y],_tiles[x-1,y-1],_tiles[x-2,y])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x-1,y-1]);
						_hints.Add(_tiles[x-2,y]);
						return ;
					}else if(b&&checkIsSameBlock(_tiles[x,y],_tiles[x-1,y+1],_tiles[x-2,y])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x-1,y+1]);
						_hints.Add(_tiles[x-2,y]);
						return ;
					}
				}
				if( x <= _tilesNum.x-3){
					//find hint right
					if(b&&checkIsSameBlock(_tiles[x,y],_tiles[x+1,y],_tiles[x+2,y+1])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x+1,y]);
						_hints.Add(_tiles[x+2,y+1]);
						return ;
					}else if(t&&checkIsSameBlock(_tiles[x,y],_tiles[x+1,y],_tiles[x+2,y-1])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x+1,y]);
						_hints.Add(_tiles[x+2,y-1]);
						return ;
					}else if(t&&checkIsSameBlock(_tiles[x,y],_tiles[x+1,y-1],_tiles[x+2,y])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x+1,y-1]);
						_hints.Add(_tiles[x+2,y]);
						return ;
					}else if(b&&checkIsSameBlock(_tiles[x,y],_tiles[x+1,y+1],_tiles[x+2,y])){
						_hints.Add(_tiles[x,y]);
						_hints.Add(_tiles[x+1,y+1]);
						_hints.Add(_tiles[x+2,y]);
						return ;
					}
				}
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

		updateThreeMatching();
		updateTouchBoard();
	}
}
