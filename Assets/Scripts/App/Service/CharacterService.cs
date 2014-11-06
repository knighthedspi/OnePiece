using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterService{

	public readonly static CharacterService Instance = new CharacterService();
	
	public OPCharacter initCharacter()
	{
//		int id, string characterName, int levelID, Vector3 direction, Vector3 position, int soundId
		return new OPCharacter(
			1,
			"Luffy 1",
			1,
			new Vector3(0, 0, 0), //direction
			new Vector3(100, 290, 0), //positon
			1
			);
	}

	public OPCharacter initUnit()
	{
		return new OPCharacter(
			Random.Range(0,2),
			"monster 1",
			1,
			new Vector3(0, 0, 0),
			new Vector3(-100, 290, 0),
			1
			);
	}


	public OPCharacter getCharacterByLevel(int levelId)
	{
		//#TODO get from db
		return initCharacter();
	}

	public OPCharacter getCurrentUnit()
	{
		//#TODO get from db
		return initUnit();
	}
	
}