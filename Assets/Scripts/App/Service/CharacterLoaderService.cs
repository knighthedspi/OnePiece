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
    public Monster loadCharacter(GameObject parent,Vector3 pos,Vector3 direction)
    {
        //#TODO get by user level
        OPCharacter characterObj = CharacterService.Instance.getCharacterByLevel(1);
        return MonsterService.Instance.createMonster(characterObj, Config.TAG_CHARACTER, parent, pos, direction);
    }

    /// <summary>
    /// Loads list of monster per game level
    /// </summary>
    /// <returns>The list of Monster Components</returns>
    /// <param name="parent">Parent.</param>
    /// <param name="pos">Position.</param>
    /// <param name="direction">Direction.</param>
    public List<Monster> loadMonsterList(GameObject parent,List<Vector3> pos,Vector3 direction)
    {
        // TODO get by user 's current monster id
        int cmID = 0;
        List<OPCharacter> listMonsterModel = OPCharacterDAO.Instance.getListMonster(cmID);
        List<Monster> listMonster = MonsterService.Instance.createListMonster(listMonsterModel, parent, pos, direction); 
        return listMonster;	
    }

}