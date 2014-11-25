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
    public Monster loadCharacter(Vector3 pos, Vector3 direction)
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
    public List<Monster> loadMonsterList(List<Vector3> pos, Vector3 direction)
    {
        List<OPCharacter> listMonsterModel = OPCharacterDAO.Instance.getListMonster(AppManager.Instance.user.CurrentMonsterID);
        List<Monster> listMonster = MonsterService.Instance.createListMonster(listMonsterModel, _panel, pos, direction); 
        return listMonster;	
    }

	/// <summary>
	/// Loads the characters ,include main character n monsters
	/// </summary>
	/// <param name="parent">Parent game object</param>
	/// <param name="characterPosition">Character position.</param>
	/// <param name="characterDirection">Character direction.</param>
	/// <param name="monsterPosition">Monster position.</param>
	/// <param name="monsterDirection">Monster direction.</param>
	/// <param name="currentCharacter">Current character will be loaded</param>
	/// <param name="monsterList">Monster list will be loded</param>
	public void loadCharacters(Vector3 characterPosition, Vector3 characterDirection, List<Vector3> monsterPosition, Vector3 monsterDirection, ref Monster currentCharacter, ref List<Monster> monsterList)
	{
		currentCharacter = loadCharacter(characterPosition, characterDirection);
		monsterList 	=  loadMonsterList(monsterPosition, monsterDirection);
	}

}