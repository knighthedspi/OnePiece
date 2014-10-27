using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary> 
//  Link-Matching Game Puzzle
/// </summary>
public class GameSystem_LinkMatch : MainGameSystem {
	List<Block> _stackBlock = new List<Block>();
	List<GameObject> _stackDot = new List<GameObject>();
	List<GameObject> _stackLine = new List<GameObject>();

	GameObject _currentLine = null;

	// Use this for initialization
	void Start () {
		// super-class(MainGameSystem) start function called
		base.Start();
		_gameState = GameState.GAME_WORK;
	}

	//make prefab
	GameObject MakeDotPrefab(){
#if(_NGUI_PRO_VERSION_)
		return (GameObject)Instantiate(Resources.Load("Prefab/UI/NGUI Pro/Dotting"));
#else
		return (GameObject)Instantiate(Resources.Load("Prefab/UI/NGUI Free/Dotting"));
#endif
	}
	GameObject MakeLinePrefab(){
#if(_NGUI_PRO_VERSION_)
		return (GameObject)Instantiate(Resources.Load("Prefab/UI/NGUI Pro/ConnectLine"));
#else
		return (GameObject)Instantiate(Resources.Load("Prefab/UI/NGUI Free/ConnectLine"));
#endif
	}

	// get tiles position... 
	// how to dispose tiles
	public override Vector2 tilePos(int x,int y){
		if(x >= (int)_tilesNum.x || x < 0)return new Vector2(0,0);
		if(y >= (int)_tilesNum.y || y < 0)return new Vector2(0,0);
		
		float curve = x%2*40;

		Transform trans = _board.transform;
		
#if(_NGUI_PRO_VERSION_)
		float width = _board.GetComponent<UISprite>().width;
		float height = _board.GetComponent<UISprite>().height;
#else
		float width = trans.localScale.x;
		float height = trans.localScale.y;
#endif
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
	bool existsBlockInStack(Block _b){
		foreach(Block b in _stackBlock){
			if(_b == b)return true;
		}
		return false;
	}

	//make new draw line and set currentLine 
	void newCurrentLine(Vector3 p){
		_currentLine = MakeLinePrefab();
		_currentLine.transform.parent = _panel.transform;
#if(_NGUI_PRO_VERSION_)
		_currentLine.transform.localScale = new Vector3(1,1,1);
		_currentLine.GetComponent<UISprite>().width  = 18;
		_currentLine.GetComponent<UISprite>().height = 0;
#else
		_currentLine.transform.localScale = new Vector3(18,0,1);
#endif
		_currentLine.transform.localPosition = p;
	}

	//add tile in linked stack 
	void Push(Block _b){
		_stackBlock.Add(_b);
		_b.touchDown();

		//dot generate
		GameObject dot = MakeDotPrefab();
		dot.transform.parent = _panel.transform;
#if(_NGUI_PRO_VERSION_)
		dot.transform.localScale = new Vector3(1,1,1);
		dot.GetComponent<UISprite>().width  = 24;
		dot.GetComponent<UISprite>().height = 24;
#else
		dot.transform.localScale = new Vector3(24,24,1);
#endif
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
	void dotLineDestroy(){
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

#if(_NGUI_PRO_VERSION_)
		_currentLine.GetComponent<UISprite>().width = 18;
		_currentLine.GetComponent<UISprite>().height= (int)dis;
#else
		_currentLine.transform.localScale = new Vector3(18,dis,0);
#endif
	}

	//==================================
	// update functions start
	//==================================
	
	// update touching or mouse process 
	void updateTouchBoard(){
		if(isBlocksMoveToAnim()) return ;

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
							if(dis < 85f){
								if(_stackBlock.Count > 1){
									if(_stackBlock[_stackBlock.Count - 2] == _b){
										Pop();
									}
								}
								if(!existsBlockInStack(_b)){
									Push(_b);
									newCurrentLine(_b.transform.localPosition);
								}
							}
						}
					}
				}
	        }
	        
	    }else{ // touch end or mouse release

	    	if(_stackBlock.Count > 0){
	    		if(_stackBlock.Count >= 3){
					destroyBlocks(_stackBlock);
					decreaseTurn();
				}
				foreach(Block b in _stackBlock)
					b.touchUp();
					
				_stackBlock.Clear();
				dotLineDestroy();
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
