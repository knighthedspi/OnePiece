﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GamePlay : GameSystem_LinkMatch {

	public int deltaStartX  = 13;
	public int deltaStartY  = -20;
	private bool _startGame;
	private Monster _currentCharacter;

	// belly count
	private int _beriCount;

	// score point
	private int _scorePoint;

	// score calculator
	public int scoreRatio1 = 40;
	public int scoreDelta = 3;
	public int scoreRatio2 = 50;

	// belly calculator
	public int bellyRatio1 = 2;
	public int bellyRatio2 = 4;

	// combo
	public float comboStepTime = 0.2f;
	private bool _startCombo ;
	private float _comboTime;

	// fever
	public int feverLimit = 10;
	public float feverStepTime = 0.3f;
	private bool _startFever ;
	private float _feverTime;
	private Animator _feverAnimator;
	private Animator _boardAnimator;
	private List<Block> _neighbors;

	//fever ui
	public UISprite _UI_FeverBack;

	// list of monster
	private List<Monster> _monsterList;
	public int deltaMonsterPos = 300;

	// Use this for initialization
	public override void Start () {
		base.Start();
		GamePlayService.Instance.initBlock(_tilesNum, _tiles);
		_top_field.GetComponent<Field>().Finish = OnFinishedWorking;
		_startGame = true;

		_beriCount = 0;
		_scorePoint = 0;
		_startCombo = false;
		_comboTime = 0.0f;
		_startFever = false;
		_feverTime = 0.0f;
		_feverAnimator = _stageLabel.GetComponent<Animator>();
		_boardAnimator = _board.GetComponent<Animator>();
		_neighbors = new List<Block>();

		_monsterList = new List<Monster>();
	}
	
	/// <summary>
	/// /Find hint
	/// </summary>
	public override void FindHint ()
	{
		_hints = GamePlayService.Instance.FindHint();
	}
	
	/// <summary>
	/// Calculate the position for each block
	/// </summary>
	/// <returns>The position of block</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public override Vector2 tilePos(int x,int y){
		if(x >= (int)_tilesNum.x || x < 0)return new Vector2(0,0);
		if(y >= (int)_tilesNum.y || y < 0)return new Vector2(0,0);
		
		float curve = x%2*42;
		
		Transform trans = _board.transform;
		
		float width = _board.GetComponent<UISprite>().width;
		float height = _board.GetComponent<UISprite>().height;
		float startX = trans.localPosition.x - width/2 + deltaStartX;
		float startY = trans.localPosition.y - height/2 + deltaStartY;
		float posX = (x*_tileSize.y);
		float posY = ((_tilesNum.x-y-1)*(_tileSize.x));

		posX = posX + startX + _tileSize.y/2 + _boardPadding.x + _tilesMargin.x * x;
		posY = posY + startY + _tileSize.x/2 + _boardPadding.y + _tilesMargin.y * y + curve;

		return new Vector2(posX , posY);
	}
	
	public void OnFinishedWorking(){
		_gameState = GameState.GAME_PLAYING;
		loadCharacters();
		updateTurnUI();
	}

	protected override void updateTurnUI ()
	{
		// don't show turn label because we use timer instead
		_turnLabel.text = "";
	}

	private void loadCharacters()
	{
		if(_startGame){
			loadCharacter(Config.CHARACTER_POSITION, Vector3.zero);
			loadMonsterList(initMonsterPosition(), Vector3.zero);
			loadMonster(Config.MONSTER_POSITION);
			_UI_MonsterHP.fillAmount = 1;
			_startGame = false;
		}
	}

	/// <summary>
	/// Loads the character.
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="direction">Direction.</param>
	private void loadCharacter(Vector3 pos, Vector3 direction){
		_currentCharacter = GamePlayService.Instance.loadCharacter(_panel, pos, direction);
		_playerAttackPoint = _currentCharacter._attackPoint;
		_currentCharacter.Finish = OnFinishedCharacterAnim;
		_currentCharacter.entryPlay();
	}

	/// <summary>
	/// Inits the list of monster position.
	/// </summary>
	/// <returns>The monster position.</returns>
	private List<Vector3> initMonsterPosition(){
		List<Vector3> pos = new List<Vector3>(); 
		// TODO : fix monster position
		for(int i = 0; i < Config.COUNT_OF_MONSTERS; i++){
			pos.Add(new Vector3(Config.MONSTER_POSITION.x, Config.MONSTER_POSITION.y + deltaMonsterPos * i, 0) );
		}
		return pos;
	}

	private void loadMonsterList(List<Vector3> pos, Vector3 direction){
		_monsterList = GamePlayService.Instance.loadMonsterList(_panel, pos, direction);
	}

	/// <summary>
	/// Loads from monster list to current monster
	/// </summary>
	/// <param name="pos">Position.</param>
	private void loadMonster(Vector3 pos){
		// check count of monster list , if equal to 0 it means all monster are defeated
		if(_monsterList.Count == 0){
			StartCoroutine("GameClear");
			return;
		}
		// get monster from list
		_currentMonster = _monsterList[0];
		// remove from list
		_monsterList.Remove(_currentMonster);
		TweenPosition.Begin(_currentMonster.gameObject, Config.TWEEN_DURATION, pos); 
		_currentMonster.Finish = OnFinishedMonsterAnim;
		_currentMonster.entryPlay();
	}

	private void attackEffect(Vector2 lastPos){
		GameObject o = (GameObject)Instantiate(Resources.Load("Prefab/UI/NGUI Pro/DamageLabel"));
		o.GetComponent<DamageLabel>().go(_playerAttackPoint,_panel,lastPos);
		if(_currentMonster == null)
			return;
		_currentMonster._hp -= _playerAttackPoint;
		_UI_MonsterHP.fillAmount = _currentMonster._hp / _currentMonster._maxhp;
		if(_currentMonster._hp < 0){
			if(_gameState == GameState.GAME_PLAYING){
				_currentMonster.diePlay();
				_gameState = GameState.GAME_READY;
			}
		}
	}

	/// <summary>
	/// Raises the finished player attack event.
	/// </summary>
	/// <param name="lastPos">Last position.</param>
	public override void OnFinishedPlayerAttack(Vector2 lastPos){
		_currentCharacter.attackPlay();
		attackEffect(lastPos);
//		SoundManager.Instance.PlaySE("water-drop");
	}

	/// <summary>
	/// Raises the finished character animation event.
	/// </summary>
	/// <param name="type">Type.</param>
	public void OnFinishedCharacterAnim( string type ){
		if(_currentCharacter == null) return ;

		if(type == "attack" && _currentMonster != null){
			_currentMonster.attackedPlay();
		}
		else if(type == "die"){
			Destroy(_currentCharacter.gameObject);
			_currentCharacter = null;
			StartCoroutine("GameOver");
		}
	}

	/// <summary>
	/// Raises the finished monster animation event.
	/// </summary>
	/// <param name="type">Type.</param>
	public void OnFinishedMonsterAnim( string type ){
		// monster is only be attacked then die	
		if(_currentMonster == null)
			return;
		if(type == "die"){
			Destroy(_currentMonster.gameObject);
			_currentMonster = null;
			_gameState = GameState.GAME_WORK;
			loadMonster(Config.MONSTER_POSITION);
			_UI_MonsterHP.fillAmount = 1;
		}
	}

	/// <summary>
	/// Updates the timer.
	/// </summary>
	protected override void updateTimer(){
		base.updateTimer();

		// update combo time
		_comboTime += Time.deltaTime;
		if(_comboTime >= comboStepTime)
			resetCombo();
		else
			_startCombo = true;

		// update fever time when in fever mode
		if(_startFever)
			_feverTime += Time.deltaTime;
		if(_feverTime >= feverStepTime)
			resetFever();
		_UI_PlayerHP.fillAmount = remain_time/stage_time;
		updateFeverUI();
	}

	/// <summary>
	/// Resets fever time
	/// </summary>
	private void resetFever(){
		_startFever = false;
		_feverTime = 0.0f;
		// stop board animation
		_boardAnimator.SetBool("startFever", false);
		_neighbors.Clear();
	}

	/// <summary>
	/// Starts fever time
	/// </summary>
	private void startFever(){
		_startFever = true;
		_feverTime = 0.0f;

		// fever animation
		_stageLabel.text = "Fever time";
		_feverAnimator.Play("StageLabel");
		// start board animation
		_boardAnimator.SetBool("startFever", true);
	}

	/// <summary>
	/// Updates the stack block.
	/// </summary>
	/// <param name="b">Main block</param>
	protected override void updateStackBlock(Block b){
		if(_startFever){
			GamePlayService.Instance.addNeighborBlock2Stack(b, _neighbors);
		}
	}
	
	/// <summary>
	/// Releases blocks when release touch or mouse
	/// </summary>
	protected override void releaseBlocks(){
		if(_stackBlock.Count > 0){
			if(_stackBlock.Count >= 3){
				_stackBlock.AddRange(_neighbors);
				destroyBlocks(_stackBlock);
				decreaseTurn();
				// update score 
				updateScore();
			}else
				_neighbors.Clear();
			foreach(Block b in _stackBlock)
				b.touchUp();
			
			_stackBlock.Clear();
			_neighbors.Clear();
			dotLineDestroy();
		}else{
			_neighbors.Clear();
		}
		
	}


	/// <summary>
	/// Resets the combo whether combotime exceeded limit
	/// </summary>
	protected override void resetCombo(){
		base.resetCombo();
		_comboTime = 0.0f;
		_startCombo = false;
	}

	/// <summary>
	/// Increases current combo value when in combo mode
	/// </summary>
	protected override void increaseCombo(){
		base.increaseCombo();
		_comboTime = 0;
		// start fever time when combo greater than fever limt
		if(_currentCombo > feverLimit && !_startFever)
			startFever();

		if(_startFever)
			_feverTime = 0;
	}

	/// <summary>
	/// Updates score, override base function
	/// </summary>
	protected override void updateScore(){
		OPDebug.Log("update score with count of block is " + _stackBlock.Count + " and combo is " + _currentCombo + "; start combo is " + _startCombo);
		if(_startCombo)
			increaseCombo();	

		_scorePoint += scoreRatio1 * ( _stackBlock.Count - scoreDelta) + _currentCombo + scoreRatio2;
		if(_stackBlock.Count >= 4)
			_beriCount += bellyRatio1 * ( _stackBlock.Count - bellyRatio2 );

		// update UI
		_scoreLabel.setText(_scorePoint.ToString());
		_goldLabel.setText(_beriCount.ToString());
	}

	protected override IEnumerator GameClear ()
	{
		dotLineDestroy();
		return base.GameClear ();
	}

	//Update fever UI
	protected void updateFeverUI()
	{
		float percent = (float)_currentCombo /(float)feverLimit; 
//		Debug.Log(_currentCombo);
		percent = 0.0f;
		Debug.LogError(percent);
		_UI_FeverBack.fillAmount = percent;
		Debug.LogError(_UI_FeverBack.fillAmount);
	}
}
