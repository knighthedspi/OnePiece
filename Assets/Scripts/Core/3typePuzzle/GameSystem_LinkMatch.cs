using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary> 
//  Link-Matching Game Puzzle
/// </summary>
public class GameSystem_LinkMatch : MainGameSystem {
	protected List<Block> _stackBlock = new List<Block>();
	protected List<GameObject> _stackDot = new List<GameObject>();
	protected List<GameObject> _stackLine = new List<GameObject>();

	protected GameObject _currentLine = null;

	private Vector2 _startBlockPosInBoard;
	private Vector2 _endBlockPosInBoard;

	private float _limit;

	// Use this for initialization
	public virtual void Start () {
		// super-class(MainGameSystem) start function called
		base.Start();
		_gameState = GameState.GAME_WORK;
		_limit = (float) Math.Sqrt(_tileSize.x * _tileSize.x / 4 + _tileSize.y * _tileSize.y); 
	}

	//make prefab
	GameObject MakeDotPrefab(){
		return (GameObject)Instantiate(Resources.Load("Prefab/UI/NGUI Pro/Dotting"));
	}
	GameObject MakeLinePrefab(){
		return (GameObject)Instantiate(Resources.Load("Prefab/UI/NGUI Pro/ConnectLine"));
	}

	// get tiles position... 
	// how to dispose tiles
	public override Vector2 tilePos(int x,int y){
		if(x >= (int)_tilesNum.x || x < 0)return new Vector2(0,0);
		if(y >= (int)_tilesNum.y || y < 0)return new Vector2(0,0);
		
		float curve = x%2*40;

		Transform trans = _board.transform;
		
		float width = _board.GetComponent<UISprite>().width;
		float height = _board.GetComponent<UISprite>().height;
		float startX = trans.localPosition.x - width/2 + 13;
		float startY = trans.localPosition.y - height/2 + 45;
		float posX = (x*_tileSize.x);
		float posY = ((_tilesNum.y-y-1)*(_tileSize.y));
		return new Vector2(
				posX + startX + _tileSize.x/2 + _boardPadding.x + _tilesMargin.x * x,
				posY + startY + _tileSize.y/2 + _boardPadding.y + _tilesMargin.y * y + curve
			);
	}

	//find tile in linked stack
	protected bool existsBlockInStack(Block _b){
		return _stackBlock.Contains(_b);
	}

	//make new draw line and set currentLine 
	void newCurrentLine(Vector3 p){
		_currentLine = MakeLinePrefab();
		_currentLine.transform.parent = _panel.transform;
		_currentLine.transform.localScale = new Vector3(1,1,1);
		_currentLine.GetComponent<UISprite>().width  = 18;
		_currentLine.GetComponent<UISprite>().height = 0;
		_currentLine.transform.localPosition = p;
		SoundManager.Instance.PlaySE("water-drop");
	}

	//add tile in linked stack 
	void Push(Block _b){
		_stackBlock.Add(_b);
		_b.touchDown();

		//dot generate
		GameObject dot = MakeDotPrefab();
		dot.transform.parent = _panel.transform;
		dot.transform.localScale = new Vector3(1,1,1);
		dot.GetComponent<UISprite>().width  = 24;
		dot.GetComponent<UISprite>().height = 24;
		dot.transform.localPosition = _b.transform.localPosition;

		_stackDot.Add(dot);

		if(_currentLine != null){
			_stackLine.Add(_currentLine);
			drawConnectLine(dot.transform.localPosition);
		}
	}

	//pop tile in linked stack
	void Pop(){
		Block last = _stackBlock[_stackBlock.Count - 1];

		_stackBlock.Remove(last);
		last.touchUp();

		GameObject _golast = _stackDot[_stackDot.Count - 1];
		_stackDot.Remove(_golast);
		Destroy(_golast);

		if(_stackLine.Count > 0){
			Destroy(_currentLine);
			_currentLine = _stackLine[_stackLine.Count - 1];
			_stackLine.Remove(_currentLine);
		}
	}

	// remove all dot and drawLine
	protected void dotLineDestroy(){
		foreach(GameObject go in _stackDot)
			Destroy(go);
		_stackDot.Clear();

		foreach(GameObject go in _stackLine)
			Destroy(go);
		_stackLine.Clear();

		Destroy(_currentLine);
		_currentLine = null;
	}

	//update current draw line rotate and scale
	void drawConnectLine(Vector3 p){
		Vector2 target = (p-_currentLine.transform.localPosition);
		target.Normalize();

		float dis = Vector2.Distance(_currentLine.transform.localPosition,p);
		float angle = Vector2.Angle(target,new Vector2(0,1));
		if(target.x < 0){
			angle = 180-angle+180;	
		}

		Quaternion _target = Quaternion.Euler(0, 0, -angle);
		_currentLine.transform.rotation = Quaternion.Slerp(_currentLine.transform.rotation, _target, 1);

		_currentLine.GetComponent<UISprite>().width = 18;
		_currentLine.GetComponent<UISprite>().height= (int)dis;
		_startBlockPosInBoard = findIntersectBlock(_currentLine.transform.localPosition);
		_endBlockPosInBoard	  = findIntersectBlock(p);
	}

	//==================================
	// update functions start
	//==================================
	
	// update touching or mouse process 
	void updateTouchBoard(){
		if(isBlocksMoveToAnim()) return ;
		if(remain_time <= 0 || _currentMonster == null || _currentMonster!=null && _currentMonster.getCurrentAnimationState().Equals("die")) {
			if(_stackBlock.Count > 0){
				foreach(Block b in _stackBlock)
					b.touchUp();
				_stackBlock.Clear();
			}
			dotLineDestroy();
			return;
		}
		if (Input.GetMouseButton(0)){ // touch start or mouse clicked

        	Vector3 p = screenTo2DPoint(Input.mousePosition);
			if( intersectNodeToPoint(_board,p) ){
        		Vector2 idx = findIntersectBlock(p);
        		if(_currentLine != null){
        			drawConnectLine(p);
        		}
				if(idx.x >= 0){
					Block _b = _tiles[(int)idx.x,(int)idx.y];
					if(_stackBlock.Count == 0){
						Push(_b);
						newCurrentLine(_b.transform.localPosition);
					}else{
						Block last = _stackBlock[_stackBlock.Count - 1];
						if(last._type == _b._type){
							float dis = Vector2.Distance(last.transform.localPosition,_b.transform.localPosition);
							if(dis <= _limit){
								if(_stackBlock.Count > 1){
									if(_stackBlock[_stackBlock.Count - 2] == _b){
										Pop();
									}
								}
								if(!existsBlockInStack(_b)){
									Push(_b);
									newCurrentLine(_b.transform.localPosition);
								}
							}else{
								// find intersect block that has the same type
								OPDebug.Log("draw line from " + _startBlockPosInBoard + " to " + _endBlockPosInBoard);
								findIntersectBlockWithLine(last._type);
							}
						}
					}
					// update stack block if necessary
					updateStackBlock(_b);
				}		
	        }  		
	    }else{ // touch end or mouse release
			releaseBlocks();
	    }
	}

	private void findIntersectBlockWithLine(int blockType){
		int startPosX = _startBlockPosInBoard.x < _endBlockPosInBoard.x ? (int)_startBlockPosInBoard.x : (int)_endBlockPosInBoard.x;
		int stopPosX = _startBlockPosInBoard.x < _endBlockPosInBoard.x ? (int)_endBlockPosInBoard.x : (int)_startBlockPosInBoard.x;
		int startPosY = _startBlockPosInBoard.y < _endBlockPosInBoard.y ? (int)_startBlockPosInBoard.y : (int)_endBlockPosInBoard.y;
		int stopPosY = _startBlockPosInBoard.y < _endBlockPosInBoard.y ? (int)_endBlockPosInBoard.y : (int)_startBlockPosInBoard.y;
		OPDebug.Log("start:(" + startPosX + ";" + startPosY + ");stop:(" + stopPosX + ";" + stopPosY + "); line direction: " + _currentLine.gameObject.transform.localRotation.eulerAngles );
		for(int i = startPosX ; i <= stopPosX; i++){
			for(int j = startPosY ; j <= stopPosY; j++){
				Block b = _tiles[i,j];
				Block last = _stackBlock[_stackBlock.Count - 1];
				float dis = Vector2.Distance(last.transform.localPosition, b.transform.localPosition);
				//only add to line iff block has same type and intersected with current line
				if(b._type == blockType && !existsBlockInStack(b) && dis <= _limit ){
					OPDebug.Log("add block(" + i + ";" + j + ") to stack");
					Push(b);
					newCurrentLine(b.transform.localPosition);
				}
			}
		}
	}

	/// <summary>
	/// Updates the stack block.
	/// </summary>
	/// <param name="b">Main block</param>
	protected virtual void updateStackBlock(Block b){}

	/// <summary>
	/// Releases blocks when release touch or mouse
	/// </summary>
	protected virtual void releaseBlocks(){
		if(_stackBlock.Count > 0){
			if(_stackBlock.Count >= 3){
				destroyBlocks(_stackBlock);
				decreaseTurn();
				// update score 
				updateScore();
			}
			foreach(Block b in _stackBlock)
				b.touchUp();
			
			_stackBlock.Clear();
			dotLineDestroy();
		}

	}

	/// <summary>
	/// Updates the score.
	/// </summary>
	protected virtual void updateScore(){}

	//==================================
	// update functions end
	//==================================
	protected virtual void Update () {
		if(_gameEnd)return ;
		// super-class(MainGameSystem) Update function called
		base.Update();

		updateTouchBoard();
	}
}
