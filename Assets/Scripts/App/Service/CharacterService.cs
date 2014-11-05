using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterService{

	public readonly static CharacterService Instance = new CharacterService();

	//create monster function
	public GameObject createMonster(GameObject prefab, GameObject parent, Vector3 localPosition = default(Vector3)){
		GameObject monster = (GameObject)UnityEngine.GameObject.Instantiate(prefab);
		monster.transform.parent = parent.transform;
		monster.transform.localPosition = localPosition;
		monster.transform.localScale = new Vector3(0.8f,0.8f,1);
		return monster;
	}

	public OPCharacter getCharacter(int id)
	{

	}
}