using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary> 
// game main system.
/// </summary>
public class MainGameSystem : MonoBehaviour {
	public enum GameState{
		GAME_ENTRY,
		GAME_WORK,
		GAME_WORKING,
		GAME_READY,
		GAME_PLAYING,
		GAME_ATTACKING,
		GAME_SELECT_BLOCK,
		GAME_SWAP_BLOCK,
	}

	////////////////////////////////////////
	//you can customizing game variable in inspector window
	////////////////////////////////////////
	public NumberLabel _goldLabel;
	public NumberLabel _cashLabel;
	public NumberLabel _scoreLabel;

	public GameObject _hintPrefab;

	public UILabel	_descriptLabel;
	public UILabel 	_stageLabel;
	public UILabel  _turnLabel;

	public GameObject _panel;
	public GameObject _hintPanel;
	public GameObject _board;
	public GameObject[] _blocksPrefab;
	public GameObject[] _monsterPrefab;

	public UISprite _UI_PlayerHP;
	public UISprite _UI_MonsterHP;

	public Animator _top_field;

	public Vector2 _tilesNum;
	public Vector2 _tileSize;

	public Camera _camera;

	public Vector2 _boardPadding;
	public Vector2 _tilesMargin;

	public int _maxStage;

	public float _hintTime=5f;

	//============================
	//player variables!
	//============================
	public float _playerHP;
	public float _playerMaxHP;
	public float _playerAttackPoint;


	//private member variable
	protected Block[,] _tiles;

	protected GameState _gameState;

	protected Monster _currentMonster = null;
	protected int _currentCombo = 0;
	protected int _currentStage = 0;

	protected float _fShakingScreen = 0;

	protected bool _gameEnd = false;
	protected List<Block> _hints = new List<Block>();
	protected List<GameObject> _hintSpr = new List<GameObject>();
	private bool _hintDirty = true;
	private float _hintDt = 0;

	// Use this for initialization
	public void Start () {
		if(_tilesNum.x <= 3) _tilesNum.x = 4;
		if(_tilesNum.y <= 3) _tilesNum.y = 4;
		
		_tiles = new Block[(int)_tilesNum.x,(int)_tilesNum.y];
		_gameState = GameState.GAME_ENTRY;

		_top_field.GetComponent<Field>().Finish = OnFinishedWorking;

		_goldLabel.setNumber(0);
		_cashLabel.setNumber(0);
		_scoreLabel.setNumber(0);
	}

	// get tiles position... 
	// how to dispose tiles
	// this function must be use after override
	public virtual Vector2 tilePos(int x,int y){
		return new Vector2();
	}

	//make particle
	public GameObject MakeBlockDestroyParticle(){
#if(_NGUI_PRO_VERSION_)
			return (GameObject)Instantiate(Resources.Load("Prefab/particle/NGUI Pro/BlockDestroyParticle"));
#else
			return (GameObject)Instantiate(Resources.Load("Prefab/particle/NGUI Free/BlockDestroyParticle"));
#endif
	}

	public GameObject MakePlayerAttackParticle(){
#if(_NGUI_PRO_VERSION_)
			return (GameObject)Instantiate(Resources.Load("Prefab/particle/NGUI Pro/PlayerAttackParticle"));
#else
			return (GameObject)Instantiate(Resources.Load("Prefab/particle/NGUI Free/PlayerAttackParticle"));
#endif
	}

	// make new block on top y-axis
	protected Block pushNewItem(int r , int x){
		if(r < 0 || r >= _blocksPrefab.Length ) r = 0;
		if(_tiles[x,0] != null) return null;

		GameObject newBlock = (GameObject)Instantiate(_blocksPrefab[r]);
		newBlock.transform.parent = _panel.transform;
		newBlock.transform.localPosition = new Vector3( tilePos(x,0).x , 200 );

#if(_NGUI_PRO_VERSION_)
			newBlock.transform.localScale = new Vector2(1,1);
			newBlock.GetComponent<UISprite>().width = (int)_tileSize.x;
			newBlock.GetComponent<UISprite>().height = (int)_tileSize.y;
#else
			newBlock.transform.localScale = new Vector2(_tileSize.x,_tileSize.y);
#endif

		Block b = newBlock.GetComponent<Block>();
		b._type = r;

		_tiles[x,0] = b;

		_hintDirty = true;
		foreach (GameObject go in _hintSpr){
			Destroy(go);
		}
		_hintSpr.Clear();
		return b;
	}

	//check someone tiles tweening?
	protected bool isBlocksMoveToAnim(){
		for(int i=0;i < _tilesNum.x ; i++){
			for(int j=0;j < _tilesNum.y ; j++){
				if( _tiles[i,j] == null) return true;
				if( _tiles[i,j].isAnimation() ){
					return true;
				}
			}
		}
		return false;
	}
 
	//find tile's array-x,y with mouse position
	protected Vector2 findIntersectBlock(Vector3 pos){
		for(int i=0;i < _tilesNum.x ; i++){
			for(int j=0;j < _tilesNum.y ; j++){
				if(_tiles[i,j] != null && 
					intersectNodeToPoint(_tiles[i,j].gameObject,pos)){
					return new Vector2(i,j);
				}
			}
		}
		return new Vector2(-1,-1);
	}

	//find all tiles with block type
	protected List<Block> findBlocksWithType(int type){
		List<Block> blocks = new List<Block>();

		for(int i=0;i < _tilesNum.x ; i++){
			for(int j=0;j < _tilesNum.y ; j++){
				if(_tiles[i,j] != null && _tiles[i,j]._type == type){
					blocks.Add(_tiles[i,j]);
				}
			}
		}
		return blocks;
	}

	// destroy blocks. and return how many destroyed blocks count.
	// if now monster turn up, player will attacking to monster!
	protected int destroyBlocks(List<Block> v, Block except=null){
		int count = 0;
		foreach (Block _b in v){
			if(except == _b) continue;

			_tiles[(int)_b._posInBoard.x,(int)_b._posInBoard.y] = null;

			GameObject _p = MakeBlockDestroyParticle();
			_p.transform.parent = _panel.transform;
			_p.SendMessage("generate",_b);

			if( _currentMonster != null){
				PlayerAttackParticle __= (MakePlayerAttackParticle()).GetComponent<PlayerAttackParticle>();
				__.generate(_panel,_b,_b.transform.localPosition,new Vector2(0,300));
				__.Finish = OnFinishedPlayerAttack;
			}

			Destroy(_b.gameObject);	
			
			count++;
		}
		return count;
	}

	// 3d position to 2d position
	protected Vector3 screenTo2DPoint(Vector3 pos){
		if(_camera == null){
			Debug.Log("You need set _camera value. recommand NGUI Camera");
			return new Vector3();
		}
		if(_panel == null){
			Debug.Log("You need set _panel value. recommand NGUI Panel");
			return new Vector3();
		}
		Vector3 worldPos = _camera.ScreenToWorldPoint(pos);
		return _panel.transform.worldToLocalMatrix.MultiplyPoint3x4(worldPos);
	}

	// test instersect point to NGUI Sprite 
	protected bool intersectNodeToPoint(GameObject node,Vector3 pos){
		float ax = 0;
		float ay = 0;
		UISprite spr = node.GetComponent<UISprite>();
		switch(spr.pivot){
			case UIWidget.Pivot.TopLeft: ax = 0;ay = 1.0f; break;
			case UIWidget.Pivot.Top: ax = 0.5f;ay = 1.0f; break;
			case UIWidget.Pivot.TopRight: ax = 1.0f;ay = 1.0f; break;
			case UIWidget.Pivot.Left: ax = 0;ay = 0.5f; break;
			case UIWidget.Pivot.Center: ax = 0.5f;ay = 0.5f; break;
			case UIWidget.Pivot.Right: ax = 1.0f;ay = 0.5f; break;
			case UIWidget.Pivot.BottomLeft: ax = 0;ay = 0; break;
			case UIWidget.Pivot.Bottom: ax = 0.5f;ay = 0; break;
			case UIWidget.Pivot.BottomRight: ax = 1.0f;ay = 0; break;
		}
#if(_NGUI_PRO_VERSION_)
		float width = (float)spr.width;
		float height = (float)spr.height;
#else
		float width = node.transform.localScale.x;
		float height = node.transform.localScale.y;
#endif

		Vector3 nodePos = new Vector3(
			node.transform.localPosition.x - width * ax,
			node.transform.localPosition.y - height * ay,
			0
		);
		if( pos.x > nodePos.x && pos.y > nodePos.y &&
			pos.x < nodePos.x + width &&
			pos.y < nodePos.y + height ){
			return true;
		}
		return false;
	}

	//create monster function
	protected GameObject createMonster(int type){
		GameObject monster = (GameObject)Instantiate(_monsterPrefab[type]);
		monster.transform.parent = _panel.transform;
		monster.transform.localPosition = new Vector3(0,290);
		monster.transform.localScale = new Vector3(0.8f,0.8f,1);

		_UI_MonsterHP.fillAmount = 1;

		return monster;
	}

	//update monster turn ui
	protected void updateTurnUI(){
		_turnLabel.text = _currentMonster._turn.ToString();
	}

	//monster turn decreasec 1
	protected void decreaseTurn(){
		if(_currentMonster != null){
			_currentMonster._turn -= 1;
			if(_currentMonster._turn <= 0){
				_currentMonster._turn = 0;
			}
			updateTurnUI();
		}
	}

	// combo increase
	protected void increaseCombo(){
		_currentCombo++;

		if(_currentCombo > 1){
			_descriptLabel.text = _currentCombo.ToString()+" Combo";
#if(_NGUI_PRO_VERSION_)
			_descriptLabel.gameObject.GetComponent<Animator>().Play("Combo_NGUI_Pro");
#else
			_descriptLabel.gameObject.GetComponent<Animator>().Play("Combo");
#endif
		}
	}

	//reset combo
	protected void resetCombo(){
		_currentCombo = 0;
	}

	//Effect for player attacked from monster. 
	protected void attackedEffect(){
		_fShakingScreen = 3;

		GameObject obj = GameObject.Find("MonsterAttackEffect").gameObject;
		obj.GetComponent<UISprite>().color = new Color(1f,0f,0f,1f);
		TweenAlpha.Begin( obj, 0.5f,  0f);
	}

	//===================================
	// Finish Callback Functions
	//===================================

	//finish field walking animation.
	public void OnFinishedWorking(){
		_gameState = GameState.GAME_PLAYING;
		GameObject obj = createMonster(UnityEngine.Random.Range(0,_monsterPrefab.Length));

		_currentMonster = obj.GetComponent<Monster>();
		_currentMonster.Finish = OnFinishedMonsterAnim;
		_currentMonster.entryPlay();

		updateTurnUI();
	}

	// finish any monster animation. 
	public void OnFinishedMonsterAnim( string type ){
		if(_currentMonster == null) return ;

		if( type == "attack" && _gameState == GameState.GAME_READY ){
			_gameState = GameState.GAME_PLAYING;

			_playerHP -= _currentMonster._attackPoint;
			_UI_PlayerHP.fillAmount = _playerHP/_playerMaxHP;

			attackedEffect();

			if(_playerHP <= 0){
				StartCoroutine("GameOver");
			}

		}else if(type == "die"){
			Destroy(_currentMonster.gameObject);
			_currentMonster = null;

			_gameState = GameState.GAME_WORK;
		}
	}

	// finish particle effect that player attack to monster 
	public void OnFinishedPlayerAttack(Vector2 lastPos){
		if(_currentMonster != null){
			
			_currentMonster._hp -= _playerAttackPoint;
			_UI_MonsterHP.fillAmount = _currentMonster._hp / _currentMonster._maxhp;

#if(_NGUI_PRO_VERSION_)
			GameObject o = (GameObject)Instantiate(Resources.Load("Prefab/UI/NGUI Pro/DamageLabel"));
#else
			GameObject o = (GameObject)Instantiate(Resources.Load("Prefab/UI/NGUI Free/DamageLabel"));
#endif
			o.GetComponent<DamageLabel>().go(_playerAttackPoint,_panel,lastPos);
			
			if(_currentMonster._hp < 0){
				if(_gameState == GameState.GAME_PLAYING){
					_currentMonster.diePlay();
					_gameState = GameState.GAME_READY;
				}
				return ;
			}

			if( _currentMonster._turn == 0 ){
				_gameState = GameState.GAME_READY;

				_currentMonster.attackPlay();
				_currentMonster._turn = _currentMonster._maxturn;

				updateTurnUI();
			}else{
				_currentMonster.attackedPlay();
			}
		}
	}

	//you need override!
	public virtual void FindHint(){}

	//===========================================
	// corutine game finish
	//===========================================

	//will be call game cleared
	IEnumerator GameClear(){
		_gameEnd = true;

		GameObject obj = GameObject.Find("GameClear").gameObject;

		TweenAlpha.Begin( obj, 1.0f,  1f);
		TweenPosition.Begin( obj, 1.0f,  new Vector3(0,0,0));
		yield return new WaitForSeconds(3f);

		TweenAlpha.Begin( obj, 1.5f,  0f);
		yield return new WaitForSeconds(2f);

		Application.LoadLevel(0);
	}

	//will be call game over
	IEnumerator GameOver(){
		_gameEnd = true;

		GameObject obj = GameObject.Find("GameOver").gameObject;

		TweenAlpha.Begin( obj, 1.0f,  1f);
		TweenPosition.Begin( obj, 1.0f,  new Vector3(0,0,0));
		yield return new WaitForSeconds(3f);

		TweenAlpha.Begin( obj, 1.5f,  0f);
		yield return new WaitForSeconds(2f);

		Application.LoadLevel(0);
	}

	//==================================
	// update functions start
	//==================================

	//if blank space in tiles, fill tile
	protected void updateEmptyFill(){
		for(int i=0;i < _tilesNum.x ; i++){
			for(int j=0;j < _tilesNum.y ; j++){
				if( _tiles[i,j] == null ){
					Block block = null;
					if( j == 0 ){
						//block = pushNewItem(UnityEngine.Random.Range(0,4),i);
						block = pushNewItem(UnityEngine.Random.Range(0,_blocksPrefab.Length),i);
					}else{
						block = _tiles[i,j-1];
						_tiles[i,j-1] = null;
					}
					if(block){
						block.moveToY(tilePos(i,j).y);
						_tiles[i,j] = block;
						_tiles[i,j]._posInBoard = new Vector2(i,j);
					}
				}
			}
		}
	}

	//update walking
	protected void updateWorking(){
		if(_gameState == GameState.GAME_WORK){
			_currentStage++;

			if( _maxStage < _currentStage){
				StartCoroutine("GameClear");
				return ;
			}

			_gameState = GameState.GAME_WORKING;
#if(_NGUI_PRO_VERSION_)
				_top_field.Play("InFieldWorking_NGUI_Pro");
#else
				_top_field.Play("InFieldWorking");
#endif
			_stageLabel.text = "stage "+_currentStage.ToString()+"/"+_maxStage.ToString();
			_stageLabel.gameObject.GetComponent<Animator>().Play("StageLabel");
		}
	}

	//for shaking screen effect.
	// when monster attacked.
	protected void updateShakingScreen(){
		if(_fShakingScreen > 0){
			_fShakingScreen -= 0.1f;
			_panel.transform.localPosition = new Vector3(
					UnityEngine.Random.Range(0,8)-4,
					UnityEngine.Random.Range(0,8)-4,
					0
				);
			if(_fShakingScreen < 0){
				_panel.transform.localPosition = new Vector3(0,0,0);
			}
		}
	}

	// update hint blocks
	// if not exeists, shaking blocks;
	void updateHint(){
		if(isBlocksMoveToAnim())return ;
		if(_hintDirty){
			_hints.Clear();
			FindHint();

			if(_hints.Count == 0){
				// TODO :: RE-POSITIONING BLOCKS
			}else{
				foreach (GameObject go in _hintSpr){
					Destroy(go);
				}
				_hintSpr.Clear();

				foreach (Block _b in _hints){
					GameObject obj = (GameObject)Instantiate(_hintPrefab);
					obj.transform.parent = _hintPanel.transform;
					obj.transform.localPosition = new Vector3(_b.transform.localPosition.x,_b.transform.localPosition.y,10000);
					obj.transform.localScale = new Vector3(0,0,1);

					_hintSpr.Add(obj);
				}
			}
			_hintDirty = false;
			_hintDt=0;
		}
		_hintDt+= Time.deltaTime;
		if(_hintDt > _hintTime){
			foreach (GameObject go in _hintSpr){
#if (_NGUI_PRO_VERSION_)
				go.transform.localScale = new Vector3(1,1,1);
				go.GetComponent<UISprite>().width  = (int)_tileSize.x;
				go.GetComponent<UISprite>().height = (int)_tileSize.y;
#else
				go.transform.localScale = new Vector3(_tileSize.x,_tileSize.y,1);
#endif
			}
		}
	}
	//==================================
	// update functions end
	//==================================
	public void Update () {
		updateShakingScreen();
		updateWorking();
	
		updateEmptyFill();
		updateHint();
	}
}