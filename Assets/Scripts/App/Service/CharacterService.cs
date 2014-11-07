using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterService{

	public readonly static CharacterService Instance = new CharacterService();
	
	public OPCharacter initCharacter()
	{
//		int id, string characterName, int levelID, Vector3 direction, Vector3 position, int soundId
//		return new OPCharacter(
//			1,
//			"Luffy 1",
//			1,
//			new Vector3(0, 0, 0), //direction
//			new Vector3(100, 290, 0), //positon
//			1
//			);
		return null;
	}

	public OPCharacter initUnit()
	{
//		return new OPCharacter(
//			2,
//			"monster 1",
//			1,
//			new Vector3(0, 0, 0),
//			new Vector3(-100, 290, 0),
//			1
//			);
		return null;
	}

	/// <summary>
	/// Gets the character by level.
	/// </summary>
	/// <returns>The character by level.</returns>
	/// <param name="level">Level.</param>
	public OPCharacter getCharacterByLevel(int level)
	{
		return OPCharacterDAO.Instance.getCharacterByLevel(level);
	}

	public OPCharacter getCurrentUnit(int exp = 1)
	{
		return OPCharacterDAO.Instance.getMonsterByExp(exp);
	}
	
}