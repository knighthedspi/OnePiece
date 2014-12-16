using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
	GAME_START,
	GAME_WORK,
	GAME_PLAYING,
	GAME_CLEAR
}

public class GamePlayView : OnePieceView {
	#region 	UI
	public      GameObject 									pauseBtn;
	public      NumberLabel 								beriLabel;
	public      NumberLabel 								scoreLabel;
	public      GameObject 									hintPrefab;
	public		GameObject									effectParticle;
	public		GameObject									Dotting;
	public		GameObject									ConnectLine;
	public      UILabel										comboLabel;
	public      UILabel 									feverLabel;
	public		NumberLabel									timeLabel;
	public      GameObject 									panel;
	public      GameObject 									hintPanel;
	public      GameObject 									board;
	public      Block 										blocksPrefab;
	public      UISprite 									UI_TimerBar;
	public		NumberLabel									timeUpLabel;
	#endregion	UI

	#region 	ANIMATION
	public      Animator 									comboAnimator;
	public		Animator 									feverAnimator;
	public		Animator 									boardAnimator;
	public		Animator									timeUpAnimator;
	public		Animator									quickUpAnimator;
	#endregion 	ANIMATION
	
	#region 	CAMERA_SETTING 
	public      Camera 										camera;
	#endregion 	CAMERA_SETTING 

	#region 	UI_EVENT_DELEGATE
	[HideInInspector]
	public      UI 		  									UI 						{get; private set;}
	#endregion 	UI_EVENT_DELEGATE

	#region 	GAME_STATEMENT
	protected   GameState 									gameState;
	private     float 										_playerAttackPoint;
	protected   bool 										_gameEnd				= false;
	protected  	float 										remain_time				;
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
	protected	float										count_down_time			;			
	private		int											_currentMonsterId		;
	#endregion 	GAME_STATEMENT

	#region 	MONSTER_OBJECTS
	private 	MonsterController 							_currentMonster 		= null;
	private 	List<MonsterController> 					_monsterList 			= new List<MonsterController>();
	private 	CharacterController							_currentTroop			= null;
	private		List<CharacterController>					_troopList				= new List<CharacterController>();
	private		bool										_killAllTroops			;
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

	#region     SETUP
	private		OPGameSetup									_gameSetup;
	#endregion
	
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
		loadCharacters();	
		playCountDownAnim();
	}

	private void InitializeGameStatement()
	{
		UI = gameObject.AddComponent<UI>();
		UI.AttachButton(pauseBtn, onPauseBtnClicked);
		_isPaused = false;
		beriLabel.setNumber(0);
		scoreLabel.setNumber(0);
		_gameSetup = AppManager.Instance.gameSetup;
		remain_time = _gameSetup.stage_time;
		stage_time = remain_time;
		gameState = GameState.GAME_START;
		count_down_time = _gameSetup.count_down_time;
	 	// TODO : get player attack point from DB
		_playerAttackPoint = 100;
		timeUpLabel.gameObject.GetComponent<TimeUpController>().Finish = OnFinishCountDown;
	}

	private void InitializeGameService()
	{
		_service = GamePlayService.Instance;
		_service.initialize( board, panel, camera, effectParticle, Dotting, ConnectLine);
		_userService = UserService.Instance;
		_user = AppManager.Instance.User;
	}

	//TODO : load from previous stage
	private void loadCharacters()
	{
		_service.loadMonters(initMonsterPosition(), Vector3.zero, ref _monsterList);
		_service.createTroops();
	}

	private List<Vector3> initMonsterPosition()
	{
		List<Vector3> pos = new List<Vector3>(); 
		for(int i = 0;i < Config.COUNT_OF_MONSTERS;i++) {
			pos.Add(new Vector3(Config.MONSTER_POSITION.x, Config.MONSTER_POSITION.y + _gameSetup.deltaMonsterPos * (i + 1) , 0));
		}
		return pos;
	}

	private List<Vector3> initTroopPosition()
	{
		List<Vector3> pos = new List<Vector3>(); 
		for(int i = 0;i < Config.COUNT_OF_TROOPS;i++) {
			pos.Add(new Vector3(Config.TROOP_POSITION[i].x, Config.TROOP_POSITION[i].y + _gameSetup.deltaMonsterPos * (i + 1) , 0));
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
		_currentMonsterId = _currentMonster.monsterModel.Id;
		_monsterList.Remove(_currentMonster);
		// TODO : update entry animation
		TweenPosition.Begin(_currentMonster.entireMonsterObj, Config.TWEEN_DURATION, pos); 
		_currentMonster.Finish = OnFinishMonsterAnim;
		_currentMonster.entryPlay();
	}

	private void loadTroops(Vector3[] posList)
	{
		_service.loadTroops(initTroopPosition(), Vector3.zero, ref _troopList);
		for( int i = 0; i < _troopList.Count; i++)
		{
			TweenPosition.Begin(_troopList[i].entireMonsterObj, Config.TWEEN_DURATION, posList[i]);
			_troopList[i].Finish = OnFinishTroopAnim;
			_troopList[i].entryPlay();
		}
		_killAllTroops = false;
	}

	private void loadCurrentTroop()
	{
		_currentTroop = _troopList[0];
		_troopList.Remove(_currentTroop);
	}

	protected void playCountDownAnim()
	{
		timeUpAnimator.Play(Config.COUNT_DOWN_ANIM);
	}

	#endregion 	INITIALIZE_GAME

	#region 	UPDATE_HANDLER
	protected virtual void Update()
	{
		if(gameState == GameState.GAME_START || _isPaused )
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
		if(gameState ==  GameState.GAME_START || gameState == GameState.GAME_CLEAR || _isPaused)
			return;
		updateTimer();
	}
	
	protected virtual void updateWorking(){
		if(gameState == GameState.GAME_WORK) {
			OnFinishedWorking();
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
					go.Recycle();
				}
				_hintObjs.Clear();
				
				foreach(Block b in _hints) {
					GameObject obj = hintPrefab.Spawn();
					obj.transform.parent = hintPanel.transform;
					obj.transform.localPosition = new Vector3(b.transform.localPosition.x, b.transform.localPosition.y, 10000);
					obj.transform.localScale = new Vector3(0, 0, 1);
					
					_hintObjs.Add(obj);
				}
			}
			_hintDirty = false;
			_hintTime = 0;
		}
		_hintTime += Time.deltaTime;
		if(_hintTime > _gameSetup.hintTime) {
			foreach(GameObject go in _hintObjs) {
				go.transform.localScale = new Vector3(1, 1, 1);
				go.GetComponent<UISprite>().width = (int) _gameSetup.blockSize.x ;
				go.GetComponent<UISprite>().height = (int)_gameSetup.blockSize.y ;
			}
		}
		
	}

	protected virtual void updateScore(int destroyedBlocks)
	{
		OPDebug.Log("update score with count of block is " + destroyedBlocks + " and combo is " + _currentCombo + "; start combo is " + _startCombo);
		if(_startCombo)
			increaseCombo();	
		
		_scorePoint += _service.calculateScore(destroyedBlocks, _currentCombo, _gameSetup.scoreRatio1, _gameSetup.scoreRatio2);
		_beriCount += _service.calculateBelly(destroyedBlocks);
		_expCount += _service.calculateExp(destroyedBlocks);
		scoreLabel.setText(_scorePoint.ToString());
		beriLabel.setText(_beriCount.ToString());
	}
	
	protected virtual void updateTimer(){
		_stepTime += Time.deltaTime;

		if(_stepTime >= 1.0)
		{
			timeLabel.setText(remain_time.ToString());
			remain_time --;
			_stepTime = 0;
		}

		if(remain_time <= 5)
		{
			quickUpAnimator.SetBool("showQuickUp", true);
		}

		if(remain_time <= 0) 
		{
			quickUpAnimator.SetBool("showQuickUp", false);
			timeLabel.setText(remain_time.ToString());
			gameState = GameState.GAME_CLEAR;
			StartCoroutine(TimeUp());
		}

		UI_TimerBar.fillAmount = remain_time/stage_time;
		updateCombo();
		updateFever();
	}

	protected virtual void updateCombo(){
		_comboTime += Time.deltaTime;
		if(_comboTime >= _gameSetup.comboStepTime)
			resetCombo();
		else
			_startCombo = true;
	}


	protected virtual void updateFever(){
		if(_startFever)
			_feverTime += Time.deltaTime;
		if(_feverTime >= _gameSetup.feverStepTime)
			resetFever();
		updateFeverUI();
	}

	// TODO : update fever ui
	protected virtual void updateFeverUI(){

		
	}
	
	#endregion 

	#region 	CALLBACK_FUNCTION

	protected virtual void onPauseBtnClicked()
	{
		_isPaused = !_isPaused;
		Debug.Log("check pause: " + _isPaused);
		if(_isPaused)
			Time.timeScale = 0.0f;
		else
			Time.timeScale = 1.0f;
	}

	protected virtual void OnFinishCountDown()
	{
		gameState = GameState.GAME_WORK;
		timeUpLabel.gameObject.SetActive(false);
	}

	protected virtual void OnFinishedWorking()
	{
		gameState = GameState.GAME_PLAYING;
		loadMonster(Config.MONSTER_POSITION);
		loadTroops(Config.TROOP_POSITION);
		loadCurrentTroop();
		updateTurnUI();
	}
	
	protected virtual void updateTurnUI()
	{

	}
	
	protected virtual void OnFinishMonsterAnim(string type)
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

	protected virtual void OnFinishTroopAnim(string type)
	{
		if(_currentTroop == null)
			return;
		if(type.Equals(Config.DIE_ANIM))
		{
			_currentTroop.entireMonsterObj.Recycle();
			if(_troopList.Count > 0)
				loadCurrentTroop();
			else
				_killAllTroops = true;
		}
	}

	protected virtual void loadResultDialog()
	{
		//#TODO pass parameter
		//#TODO animation bonus score
        int bonusScore = _scorePoint * ((5+_user.LevelId)/100);
        _scorePoint += bonusScore;
        int bonusBelly = _beriCount * ((10+ _user.LevelId)/100);
        _beriCount += bonusBelly;

        DialogResult.Create(_scorePoint, _user.HighScore, _expCount, _beriCount, bonusBelly, OnOkClick);
	}

	private void OnOkClick()
	{
        //#TODO close dialog
		DialogManager.Instance.Complete();
		//#TODO back to homescreen
		ViewLoader.Instance.ReplaceLoad(Config.MAIN_VIEW, null);
	}

	private void attackEffect(Vector2 lastPos)
	{
		if(!_killAllTroops)
			applyAttack(_currentTroop, lastPos);
		else
			applyAttack(_currentMonster, lastPos);
	}

	private void applyAttack(CharacterController character, Vector3 lastPos)
	{
		if(character == null)
			return;
		Vector3 pos = new Vector3 (lastPos.x, lastPos.y, 1);
		Vector3 target = new Vector3(character.entireMonsterObj.transform.localPosition.x, character.entireMonsterObj.transform.localPosition.y , 1);
		Debug.Log(pos + " ->>>>>" + target);
		DamageEffect.Create (_playerAttackPoint.ToString (), pos, target);
		character.attackedPlay();
		character.decreaseHPAmount(_playerAttackPoint);
		if(character.currentHP < 0)
		{
			character.diePlay();
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
		if(_currentCombo > _gameSetup.feverLimit && !_startFever)
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
//		timeUpLabel.gameObject.SetActive(true);
//		TweenAlpha.Begin(timeUpLabel.gameObject, 1.0f, 1f);
//		TweenPosition.Begin(timeUpLabel.gameObject, 1.0f, new Vector3(0, 0, 0));
//		yield return new WaitForSeconds(3f);
//		
//		TweenAlpha.Begin(timeUpLabel.gameObject, 1.5f, 0f);
//		yield return new WaitForSeconds(2f);

		//#TODO check high score if has
		if(_userService.isHighScore(_user, _scorePoint)) {
			//#TODO Show dialog high score
		}
		//#TODO check leveup if has
		if(_userService.isLevelUp(_user)) {
            _user.LevelId += 1;
			//#TODO Show dialog high score
		}

		saveGameState();

		//#TODO show load result
		loadResultDialog();
		yield return 0;
	}

	private void saveGameState()
	{
		_userService.increaseBelly(_user, _beriCount);
		int currentMonsterId = _currentMonster != null ? _currentMonsterId : (_currentMonsterId + 1);
		_user.CurrentMonsterID = _currentMonsterId;
		_userService.updateState(_user, _currentMonster);
	}
	#endregion
}
