using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
	GAME_WORK,
	GAME_WORKING,
	GAME_PLAYING,
	GAME_CLEAR
}

public class GamePlayView : OnePieceView {
	#region 	UI
	public      GameObject 									pauseBtn;
	public      NumberLabel 								goldLabel;
	public      NumberLabel 								scoreLabel;
	public      GameObject 									hintPrefab;
	public      UILabel										comboLabel;
	public      UILabel 									feverLabel;
	public      GameObject 									panel;
	public      GameObject 									hintPanel;
	public      GameObject 									board;
	public      GameObject[] 								blocksPrefab;
	public      UISprite 									UI_TimerBar;
	public 		float 										deltaStartX 			= 4;
	public 		float 										deltaStartY				= -62;
	public 		UISprite 									UI_FeverBack;
	public		GameObject									UI_TimeUp;
	#endregion	UI

	#region 	ANIMATION
	public      Animator 									topFieldAnimator;
	public      Animator 									comboAnimator;
	public		Animator 									feverAnimator;
	public		Animator 									boardAnimator;
	#endregion 	ANIMATION

	#region 	BLOCK_N_BOARD
	public      Vector2 									blockNum				= new Vector2(7,  6)  ;
	public      Vector2 									blockSize 				= new Vector2(86, 78) ;
	public      Vector2 									boardPadding			= new Vector2(32, 48) ;
	public      Vector2 									blockMargin				= new Vector2(-12, -8);
	#endregion 	BLOCK_N_BOARD

	#region 	PLAYER_SETTING
	public      float 										remain_time 			= 60;
	public      float 										hintTime 				= 5;
	public 		int 										scoreRatio1 			= 20;
	public 		int 										scoreDelta 				= 3;
	public 		int 										scoreRatio2 			= 50;
	public 		int 										bellyRatio1 			= 2;
	public 		int 										bellyRatio2 			= 4;
	public 		float 										comboStepTime 			= 1.5f;
	public 		int 										feverLimit 				= 5;
	public 		float 										feverStepTime 			= 1.5f;
	public 		float 										deltaMonsterPos 		= 300;
	#endregion 	PLAYER_SETTING

	#region 	CAMERA_SETTING 
	public      Camera 										camera;
	#endregion 	CAMERA_SETTING 

	#region 	UI_EVENT_DELEGATE
	[HideInInspector]
	public      UI 		  									UI 						{get; private set;}
	#endregion 	UI_EVENT_DELEGATE

	#region 	GAME_STATEMENT
	protected   GameState 									gameState;
	private      float 										_playerAttackPoint;
	protected   bool 										_gameEnd				= false;
	protected   float 										stage_time;
	private     bool 										_isPaused;
	protected   int 										_currentCombo 			= 0;
	private 	int 										_expCount;
	private 	int 										_beriCount				= 0;		
	private 	int 										_scorePoint				= 0;
	private 	bool 										_startCombo				= false;
	private 	float 										_comboTime				= 0;
	private 	float 										_feverTime				= 0;				
	private 	bool 										_startFever 			= false;
	private 	float										_stepTime				= 0;
	#endregion 	GAME_STATEMENT

	#region 	MONSTER_OBJECTS
	protected 	CharacterController 						_currentMonster 		= null;
	private 	List<CharacterController> 					_monsterList 			= new List<CharacterController>();
	#endregion 	MONSTER_OBJECTS

	#region  	HINT_SUGGESTION
	protected 	List<Block> 								_hints 					= new List<Block>();
	protected 	List<GameObject> 							_hintObjs 				= new List<GameObject>();
	private 	bool 										_hintDirty 				= true;
	private 	float 										_hintTime 				= 0;
	#endregion  HINT_SUGGESTION

	#region  	SERVICE
	private 	GamePlayService 							_service;
	private 	UserService 								_userService;
	private 	OPUser 										_user;
	#endregion  SERVICE
	
	#region 	INITIALIZE_GAME
	protected override void Start() 
	{
		base.Start();
		Initialize();
	}

	protected override void OnOpen (params object[] parameters)
	{
		SoundManager.Instance.PlayBGM("bgm_03_fun");
	}

	protected virtual void Initialize()
	{
		InitializeGameStatement();
		InitializeGameService();
		InitializeGameSetup();
		loadCharacters();		
	}

	private void InitializeGameStatement()
	{
		UI = gameObject.AddComponent<UI>();
		UI.AttachButton(pauseBtn, onPauseBtnClicked);
		_isPaused = false;
		goldLabel.setNumber(0);
		scoreLabel.setNumber(0);
		stage_time = remain_time;
		gameState = GameState.GAME_WORK;
	 	topFieldAnimator.GetComponent<Field>().Finish = OnFinishedWorking;
		// TODO : get player attack point from DB
		_playerAttackPoint = 100;
	}

	private void InitializeGameService()
	{
		_service = GamePlayService.Instance;
		_service.initialize(blockNum, deltaStartX, deltaStartY, blockSize, boardPadding, blockMargin, board, panel, camera);
		_userService = UserService.Instance;
		_user = AppManager.Instance.User;
	}

	private void InitializeGameSetup()
	{
		OPGameSetup setup = new OPGameSetup();
		setup.blockMargin = blockMargin;
		setup.blockNum	  = blockNum;
		setup.blockSize   = blockSize;
		AppManager.Instance.gameSetup = setup;
	}

	private void loadCharacters()
	{
		_service.loadCharacters(Config.CHARACTER_POSITION, Vector3.zero, initMonsterPosition(), Vector3.zero, ref _monsterList);
	}

	private List<Vector3> initMonsterPosition()
	{
		List<Vector3> pos = new List<Vector3>(); 
		for(int i = 0;i < Config.COUNT_OF_MONSTERS;i++) {
			pos.Add(new Vector3(Config.MONSTER_POSITION.x, Config.MONSTER_POSITION.y + deltaMonsterPos * (i + 1) , 0));
		}
		return pos;
	}
	
	private void loadMonster(Vector3 pos)
	{
		if(_monsterList.Count == 0) {
			StartCoroutine(TimeUp());
			return;
		}
		_currentMonster = _monsterList[0];
		_monsterList.Remove(_currentMonster);
		// TODO : update entry animation
		TweenPosition.Begin(_currentMonster.entireMonsterObj, Config.TWEEN_DURATION, pos); 
		_currentMonster.Finish = OnFinishMonsterAnim;
		_currentMonster.entryPlay();
	}

	#endregion 	INITIALIZE_GAME

	#region 	UPDATE_HANDLER
	protected virtual void Update()
	{
		if(_isPaused)
			return;
		if(gameState == GameState.GAME_CLEAR)
		{
			clearBlocksLines();
			return;
		}
		updateWorking();
		updateEmptyFill();
		updateTouchBoard();
		updateHint();
	}
	
	protected virtual void FixedUpdate()
	{
		updateTimer();
	}

	protected virtual void updateWorking(){
		if(gameState == GameState.GAME_WORK) {
			gameState = GameState.GAME_WORKING;
			topFieldAnimator.Play(Config.FIELD_WORKING_ANIM);
		}
	}

	protected virtual void updateEmptyFill()
	{
		_service.fillBoard(blocksPrefab, ref _hintDirty, ref _hintObjs);
	}

	protected virtual void updateTouchBoard()
	{
		if(_service.isBlocksMoveToAnim()) return ;
		if(_currentMonster == null || _currentMonster!=null && _currentMonster.getCurrentAnimationState().Equals(Config.DIE_ANIM)) {
			clearBlocksLines();
			return;
		}
		// TODO : improve performance
		int destroyedBlocks = 0;
		_service.updateBoard(_startFever, OnFinishedPlayerAttack, ref destroyedBlocks);
		if(destroyedBlocks > 0)
			updateScore(destroyedBlocks);
	}

	// TODO : improve performance
	protected virtual void updateHint()
	{
		if(_service.isBlocksMoveToAnim())
			return;
		if(_hintDirty) {
			_hints.Clear();
			_hints =  _service.FindHint();

			OPDebug.Log("update hints: " + _hints.Count);

			// reset board if no hint found
			if(_hints.Count == 0) {
				resetBoard();
			} else {
				foreach(GameObject go in _hintObjs) {
					Destroy(go);
				}
				_hintObjs.Clear();
				
				foreach(Block _b in _hints) {
					GameObject obj = (GameObject)Instantiate(hintPrefab);
					obj.transform.parent = hintPanel.transform;
					obj.transform.localPosition = new Vector3(_b.transform.localPosition.x, _b.transform.localPosition.y, 10000);
					obj.transform.localScale = new Vector3(0, 0, 1);
					
					_hintObjs.Add(obj);
				}
			}
			_hintDirty = false;
			_hintTime = 0;
		}
		_hintTime += Time.deltaTime;
		if(_hintTime > hintTime) {
			foreach(GameObject go in _hintObjs) {
				go.transform.localScale = new Vector3(1, 1, 1);
				go.GetComponent<UISprite>().width = (int)blockSize.x ;
				go.GetComponent<UISprite>().height = (int)blockSize.y ;
			}
		}
		
	}

	protected virtual void updateScore(int destroyedBlocks)
	{
		OPDebug.Log("update score with count of block is " + destroyedBlocks + " and combo is " + _currentCombo + "; start combo is " + _startCombo);
		if(_startCombo)
			increaseCombo();	
		
		_scorePoint += _service.calculateScore(destroyedBlocks, _currentCombo, scoreRatio1, scoreRatio2);
		_beriCount += _service.calculateBelly(destroyedBlocks);
		_expCount += _service.calculateExp(destroyedBlocks);
		scoreLabel.setText(_scorePoint.ToString());
		goldLabel.setText(_beriCount.ToString());
	}
	
	protected virtual void updateTimer(){
		if(gameState == GameState.GAME_CLEAR)
			return;
		_stepTime += Time.deltaTime;
		if(_stepTime >= 1.0) {
			remain_time --;
			_stepTime = 0;
		}
		if(remain_time <= 0) {
			gameState = GameState.GAME_CLEAR;
			StartCoroutine(TimeUp());
		}
		UI_TimerBar.fillAmount = remain_time/stage_time;
		updateCombo();
		updateFever();
	}

	protected virtual void updateCombo(){
		_comboTime += Time.deltaTime;
		if(_comboTime >= comboStepTime)
			resetCombo();
		else
			_startCombo = true;
	}


	protected virtual void updateFever(){
		if(_startFever)
			_feverTime += Time.deltaTime;
		if(_feverTime >= feverStepTime)
			resetFever();
		updateFeverUI();
	}

	// TODO : update fever ui
	protected virtual void updateFeverUI(){

		
	}
	
	#endregion 

	#region 	CALLBACK_FUNCTION
	protected virtual void onPauseBtnClicked(){
		_isPaused = !_isPaused;
		Debug.Log("check pause: " + _isPaused);
		if(_isPaused)
			Time.timeScale = 0.0f;
		else
			Time.timeScale = 1.0f;
	}
	
	protected virtual void OnFinishedWorking()
	{
		gameState = GameState.GAME_PLAYING;
		loadMonster(Config.MONSTER_POSITION);
		updateTurnUI();
	}
	
	protected virtual void updateTurnUI()
	{

	}
	
	public void OnFinishMonsterAnim(string type)
	{
		if(_currentMonster == null)
			return;
		OPDebug.Log("finish anim: " + type);
		if(type.Equals(Config.DIE_ANIM))
	 	{
			Destroy(_currentMonster.entireMonsterObj);
			_currentMonster = null;
			gameState = GameState.GAME_WORK;	
		}
	}

	protected virtual void loadResultDialog()
	{
		//#TODO pass parameter
		//#TODO animation bonus score
		DialogOneButton.Create("Score: " + _scorePoint * 1.1, OnOkClick, "Result", "_OK");
	}

	private void OnOkClick()
	{
		//#TODO close dialog
		DialogManager.Instance.Complete();
		//#TODO back to homescreen
		ViewLoader.Instance.ReplaceLoad(Config.START_VIEW, null);
	}

	private void attackEffect(Vector2 lastPos)
	{
		GameObject o = _service.MakeDamageEffect();
		o.GetComponent<DamageLabel>().go(_playerAttackPoint, panel, lastPos);
		if(_currentMonster == null)
			return;
		_currentMonster.attackedPlay();
		_currentMonster.decreaseHPAmount(_playerAttackPoint);
		if(_currentMonster._currentHP < 0) {
			if(gameState == GameState.GAME_PLAYING) {
				_currentMonster.diePlay();
			}
		}
	}
	
	private void OnFinishedPlayerAttack(Vector2 lastPos)
	{
		attackEffect(lastPos);
		//		SoundManager.Instance.PlaySE("water-drop");
	}

	#endregion 	CALLBACK_FUNCTION	

	#region 	GAME_EFFECT
	protected virtual void resetCombo()
	{
		_currentCombo = 0;
		_comboTime = 0.0f;
		_startCombo = false;
	}

	protected virtual void resetFever()
	{
		_startFever = false;
		_feverTime = 0.0f;
		boardAnimator.SetBool("startFever", false);
		_service.clearFeverEffects();
	}

	protected virtual void increaseCombo()
	{
		_currentCombo++;
		if(_currentCombo > 1) {
			comboLabel.text = _currentCombo.ToString() + " Combos";
			OPDebug.Log("play " + _currentCombo + " combos animation");
			comboAnimator.Play(Config.COMBO_ANIM);
		}
		_comboTime = 0;
		if(_currentCombo > feverLimit && !_startFever)
			startFever();		
		if(_startFever)
			_feverTime = 0;
	}

	// TODO : improve fever animation
	protected virtual void startFever()
	{
		_startFever = true;
		_feverTime = 0.0f;

		feverLabel.text = "Fever time";
		feverAnimator.Play(Config.FEVER_ANIM);
		boardAnimator.SetBool("startFever", true);
	}

	private void resetBoard()
	{
		_service.clearBlocks();
		updateEmptyFill();
	}

	private void clearBlocksLines()
	{
		_service.clearStackBlock();
		_service.dotLineDestroy();
		_hints.Clear();
		foreach(GameObject go in _hintObjs)
			Destroy(go);
		_hintObjs.Clear();
	}
	#endregion	GAME_EFFECT

	#region 	CLEAR_STATE 
	protected virtual IEnumerator TimeUp()
	{
		TweenAlpha.Begin(UI_TimeUp, 1.0f, 1f);
		TweenPosition.Begin(UI_TimeUp, 1.0f, new Vector3(0, 0, 0));
		yield return new WaitForSeconds(3f);
		
		TweenAlpha.Begin(UI_TimeUp, 1.5f, 0f);
		yield return new WaitForSeconds(2f);
		
		_userService.increaseBelly(_user, _beriCount);
		//#TODO check high score if has
		if(_userService.isHighScore(_user, _scorePoint)) {
			//#TODO Show dialog high score
		}
		//#TODO check leveup if has
		if(_userService.isHighScore(_user, _scorePoint)) {
			//#TODO Show dialog high score
		}
		//#TODO show load result
		loadResultDialog();
		yield return 0;
	}
	#endregion
}
