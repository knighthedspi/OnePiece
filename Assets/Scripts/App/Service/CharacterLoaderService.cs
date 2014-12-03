using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GamePlayService
{

    public readonly static GamePlayService Instance = new GamePlayService();

	/// <summary>
    /// Loads the character
    /// </summary>
    /// <returns>The Monster Component of this character.</returns>
    /// <param name="parent">Parent game object</param>
    /// <param name="pos">Position of character</param>
    /// <param name="direction">Direction of character</param>
    public CharacterController loadCharacter(Vector3 pos, Vector3 direction)
    {
       	OPCharacter characterObj = CharacterService.Instance.getCharacterByLevel(AppManager.Instance.user.LevelId);
        return MonsterService.Instance.createMonster(characterObj, Config.TAG_CHARACTER, _panel, pos, direction);
    }

    /// <summary>
    /// Loads list of monster per game level
    /// </summary>
    /// <returns>The list of Monster Components</returns>
    /// <param name="parent">Parent.</param>
    /// <param name="pos">Position.</param>
    /// <param name="direction">Direction.</param>
    public List<CharacterController> loadMonsterList(List<Vector3> pos, Vector3 direction)
    {
        List<OPCharacter> listMonsterModel = OPCharacterDAO.Instance.getListMonster(AppManager.Instance.user.CurrentMonsterID);
        List<CharacterController> listMonster = MonsterService.Instance.createListMonster(listMonsterModel, _panel, pos, direction); 
        return listMonster;	
    }
	
	/// <summary>
	/// Loads the monsters 
	/// </summary>
	/// <param name="parent">Parent game object</param>
	/// <param name="monsterPosition">Monster position.</param>
	/// <param name="monsterDirection">Monster direction.</param>
	/// <param name="currentCharacter">Current character will be loaded</param>
	/// <param name="monsterList">Monster list will be loded</param>
	public void loadMonters(List<Vector3> monsterPosition, Vector3 monsterDirection, ref List<CharacterController> monsterList)
	{
		monsterList = loadMonsterList(monsterPosition, monsterDirection);
	}

	/// <summary>
	/// Creates the troops (pooled objects 4 use in future)
	/// </summary>
	public void createTroops()
	{
		List<OPCharacter> listTroopModel = OPCharacterDAO.Instance.getListTroop();
		MonsterService.Instance.createTroops(listTroopModel);
	}

	/// <summary>
	/// Loads the troops into desired postition
	/// </summary>
	/// <param name="troopPosition">List of Troop position.</param>
	/// <param name="troopDirection">Troop direction.</param>
	/// <param name="troopList">Troop list will be loaded</param>
	public void loadTroops(List<Vector3> troopPosition, Vector3 troopDirection, ref List<CharacterController> troopList)
	{
		troopList = MonsterService.Instance.loadTroops(_panel, troopPosition, troopDirection);
	}

}