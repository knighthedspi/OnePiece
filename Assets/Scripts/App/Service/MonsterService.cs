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

	/// <summary>
	/// Creates the monster/ character
	/// </summary>
	/// <returns>The monster.</returns>
	/// <param name="model">OPCharacter object</param>
	/// <param name="tag">Tag, if create character set to Config.TAG_CHARACTER, otherwise set to Config.TAG_UNIT</param>
	/// <param name="parent">Parent game object</param>
	/// <param name="localPosition">Local position</param>
	/// <param name="direction">Direction</param>
	public Monster createMonster(OPCharacter model, string tag, GameObject parent, Vector3 localPosition, Vector3 direction)
	{
		Debug.Log(model.CharacterName);
		// TODO : fix monster name
		if(!model.CharacterName.Equals("monster1") && !model.CharacterName.Equals("luffy1"))
			model.CharacterName = "monster1";
		GameObject monsterObj = (GameObject)GameObject.Instantiate(Resources.Load(Config.RESOURCE_PREFIX + model.CharacterName));
		monsterObj.transform.parent = parent.transform;
		monsterObj.transform.localPosition = localPosition;
		monsterObj.transform.localRotation = Quaternion.Euler(direction);
		monsterObj.transform.localScale = new Vector3(0.8f,0.8f,1);
		monsterObj.tag = tag;
		Monster monster = monsterObj.GetComponent<Monster>();
		monster.setProperties(model);
		if(monster == null)
			throw new UnityException("Could not load monster");
		return monster;
	}

	public List<Monster> createListMonster(List<OPCharacter> listMonster, GameObject parent , List<Vector3> localPosition, Vector3 direction){
		List<Monster> monsterList = new List<Monster>();
		foreach(OPCharacter character in listMonster){
			monsterList.Add(createMonster(character, Config.TAG_UNIT, parent, localPosition[listMonster.IndexOf(character)], direction ));
		}
		return monsterList;
	}
}