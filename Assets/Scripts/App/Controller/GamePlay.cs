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

	// find hint
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
		//#TODO update new character when one be defeated
	}

	private void loadCharacters()
	{
		if(_startGame){
			//#TODO get by user level
			GameObject character = loadCharacters(CharacterService.Instance.getCharacterByLevel(1), Config.TAG_CHARACTER, new Vector3(-100, 290, 0), Vector3.zero);
			_startGame = false;
			_currentCharacter = character.GetComponent<Monster>();
			_currentCharacter.Finish = OnFinishedCharacterAnim;
			_currentCharacter.entryPlay();
		}
		GameObject monster = loadCharacters(CharacterService.Instance.getCurrentUnit(), Config.TAG_UNIT, new Vector3(100, 290, 0), Vector3.zero);
		_currentMonster = monster.GetComponent<Monster>();
		_currentMonster.Finish = OnFinishedMonsterAnim;
		_currentMonster.entryPlay();
	}

	private GameObject loadCharacters(OPCharacter model, string tag, Vector3 postion, Vector3 direction)
	{
		//#TODO load prefab by model.id
		//#TODO load character tu model OPCharacter
		GameObject gameObj = MonsterService.Instance.createMonster(model, _panel, postion, direction);
		gameObj.tag = tag;
		return gameObj;
	}

	private void attackEffect(Vector2 lastPos){
		GameObject o = (GameObject)Instantiate(Resources.Load("Prefab/UI/NGUI Pro/DamageLabel"));
		o.GetComponent<DamageLabel>().go(_playerAttackPoint,_panel,lastPos);
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
		if(_currentMonster != null){
			if( _currentMonster._turn == 0 ){
				_gameState = GameState.GAME_READY;
				
				_currentMonster.attackPlay();
				_currentMonster._turn = _currentMonster._maxturn;				
				updateTurnUI();
				SoundManager.Instance.PlaySE("shoot");
			}else{
				_currentCharacter.attackPlay();
				attackEffect(lastPos);
				SoundManager.Instance.PlaySE("hit");
			}
		}
	}

	/// <summary>
	/// Raises the finished character animation event.
	/// </summary>
	/// <param name="type">Type.</param>
	public void OnFinishedCharacterAnim( string type ){
		if(_currentCharacter == null) return ;

		OPDebug.Log("current state is " + type);

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
		if(_currentMonster == null) return ;
		
		if( type == "attack" && _gameState == GameState.GAME_READY ){
			_gameState = GameState.GAME_PLAYING;
			
			_playerHP -= _currentMonster._attackPoint;
			_UI_PlayerHP.fillAmount = _playerHP/_playerMaxHP;

			if(_currentCharacter != null)
				_currentCharacter.attackedPlay();
			attackedEffect();

			if(_playerHP <= 0){
				_currentCharacter.diePlay();
			}
			
		}else if(type == "die"){
			Destroy(_currentMonster.gameObject);
			_currentMonster = null;
			
			_gameState = GameState.GAME_WORK;
		}
	}

}
