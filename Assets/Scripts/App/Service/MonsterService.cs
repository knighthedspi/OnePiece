using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterService{
	
	public readonly static MonsterService Instance = new MonsterService();
	
	//create monster function
	public GameObject createMonster(GameObject prefab, GameObject parent, Vector3 localPosition, Vector3 direction){
		GameObject monster = (GameObject)UnityEngine.GameObject.Instantiate(prefab);
		monster.transform.parent = parent.transform;
		monster.transform.localPosition = localPosition;
		monster.transform.localRotation = Quaternion.Euler(direction);
		monster.transform.localScale = new Vector3(0.8f,0.8f,1);
		return monster;
	}

}