using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePlay : GameSystem_LinkMatch {

	public int deltaStartX  = 13;
	public int deltaStartY  = -20;
	private bool _startGame;
	private Monster _currentCharacter;
	

	// Use this for initialization
	public override void Start () {
		base.Start();
		GamePlayService.Instance.initBlock(_tilesNum, _tiles);
		_top_field.GetComponent<Field>().Finish = OnFinishedWorking;
		_startGame = true;
	}
	
	/// <summary>
	/// /Find hint
	/// </summary>
	public override void FindHint ()
	{
		_hints = GamePlayService.Instance.FindHint();
	}
	
	// tile pos
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
		_UI_MonsterHP.fillAmount = 1;
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
			loadCharacter(new Vector3(-100, 290, 0), Vector3.zero);
			_startGame = false;
		}
		loadMonster(new Vector3(100, 290, 0), Vector3.zero);
	}

	private GameObject loadCharacters(OPCharacter model, string tag, Vector3 postion, Vector3 direction)
	{
		GameObject gameObj = MonsterService.Instance.createMonster(model, _panel, postion, direction);
		gameObj.tag = tag;
		return gameObj;
	}

	private void loadCharacter(Vector3 pos, Vector3 direction){
		//#TODO get by user level
		OPCharacter characterObj = CharacterService.Instance.getCharacterByLevel(1);
		GameObject character = loadCharacters(characterObj, Config.TAG_CHARACTER, pos, direction);
		_currentCharacter = character.GetComponent<Monster>();
		_currentCharacter.setProperties(characterObj);
		_playerAttackPoint = characterObj.AttackPoint;
		_currentCharacter.Finish = OnFinishedCharacterAnim;
		_currentCharacter.entryPlay();
	}

	private void loadMonster(Vector3 pos, Vector3 direction){
		int cmID = 0;
		if(_currentMonster != null)
			cmID = _currentMonster.id;
		OPCharacter monsterObj = CharacterService.Instance.getCurrentUnit(cmID);
		GameObject monster = loadCharacters( monsterObj, Config.TAG_UNIT, pos, direction);
		_currentMonster = monster.GetComponent<Monster>();
		_currentMonster.setProperties(monsterObj);
		_currentMonster.id = monsterObj.Id;
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
		}
	}

	/// <summary>
	/// Updates the timer.
	/// </summary>
	protected override void updateTimer(){
		base.updateTimer();
		_UI_PlayerHP.fillAmount = remain_time/stage_time;
	}
}
