using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MonsterService{
	
	public readonly static MonsterService Instance = new MonsterService();

	private List<OPCharacter> _listTroopModel;		
	private Dictionary<OPCharacter, GameObject> _troopDict ;

	public GameObject createMonster(GameObject prefab, GameObject parent, Vector3 localPosition, Vector3 direction){
		GameObject monster = (GameObject)UnityEngine.GameObject.Instantiate(prefab);
		monster.transform.parent = parent.transform;
		monster.transform.localPosition = localPosition;
		monster.transform.localRotation = Quaternion.Euler(direction);
		monster.transform.localScale = Config.COMMON_LOCAL_SCALE;
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
	public MonsterController createMonster(OPCharacter model, string tag, GameObject parent, Vector3 localPosition, Vector3 direction)
	{
		OPDebug.Log(model.CharacterName + " is loaded!!!");
		GameObject monsterObj = (GameObject) GameObject.Instantiate(Resources.Load(Config.MONSTER_RESOURCE_PREFIX + model.CharacterName));
		monsterObj.transform.parent = parent.transform;
		monsterObj.transform.localPosition = localPosition;
		monsterObj.transform.localRotation = Quaternion.Euler(direction);
		monsterObj.transform.localScale = Config.COMMON_LOCAL_SCALE;
		monsterObj.tag = tag;
		MonsterController monster = monsterObj.GetComponentInChildren<MonsterController>();
		if(monster == null)
			throw new UnityException("Could not load monster " + model.CharacterName);
		monster.monsterModel = model;
		return monster;
	}

	/// <summary>
	/// Loads the current monster from previous battle
	/// </summary>
	/// <returns>The current monster.</returns>
	/// <param name="monsterName">Monster name.</param>
	/// <param name="initialHP">Initial HP</param>
	/// <param name="currentHP">Current HP</param>
	/// <param name="tag">Tag.</param>
	/// <param name="parent">Parent game object</param>
	/// <param name="localPosition">Local position.</param>
	/// <param name="direction">Direction.</param>
	public MonsterController loadCurrentMonster(string monsterName, float initialHP, float currentHP, string tag, GameObject parent, Vector3 localPosition, Vector3 direction)
	{
		OPDebug.Log("Load current monster " + monsterName);
		GameObject monsterObj = (GameObject) GameObject.Instantiate(Resources.Load(Config.MONSTER_RESOURCE_PREFIX + monsterName));
		monsterObj.transform.parent = parent.transform;
		monsterObj.transform.localPosition = localPosition;
		monsterObj.transform.localRotation = Quaternion.Euler(direction);
		monsterObj.transform.localScale = Config.COMMON_LOCAL_SCALE;
		monsterObj.tag = tag;
		MonsterController monster = monsterObj.GetComponentInChildren<MonsterController>();
		if(monster == null)
			throw new UnityException("Could not load monster " + monsterName);
		monster.isRestored = true;
		OPCharacter monsterModel = new OPCharacter();
		monsterModel.CharacterName = monsterName;
		monsterModel.Id	  = AppManager.Instance.user.CurrentMonsterID;	
		monster.initialHP = initialHP;
		monster.currentHP = currentHP;
		monster.monsterModel = monsterModel;
		return monster;
	}

	private GameObject createTroopObject(OPCharacter model)
	{
		OPDebug.Log("Load troop: " + model.CharacterName);
		GameObject troopObj = Resources.Load(Config.MONSTER_RESOURCE_PREFIX + model.CharacterName) as GameObject;
		if(troopObj == null)
			throw new UnityException("Could not load troop " + model.CharacterName);
		troopObj.CreatePool(Config.COUNT_OF_TROOPS);
		return troopObj;
	}

	/// <summary>
	/// Creates the list of monster.
	/// </summary>
	/// <returns>The list of monster.</returns>
	/// <param name="listMonster">List monster.</param>
	/// <param name="parent">Parent GameObject</param>
	/// <param name="localPosition">Local position of monster object</param>
	/// <param name="direction">Direction of monster object</param>
	public List<MonsterController> createListMonster(List<OPCharacter> listMonster, GameObject parent , List<Vector3> localPosition, Vector3 direction){
		List<MonsterController> monsterList = new List<MonsterController>();
		foreach(OPCharacter character in listMonster){
			monsterList.Add(createMonster(character, Config.TAG_UNIT, parent, localPosition[listMonster.IndexOf(character)], direction ));
		}
		return monsterList;
	}

	/// <summary>
	/// Initializes the troop pools.
	/// </summary>
	/// <param name="listTroopModel">List troop model</param>
	public void createTroops(List<OPCharacter> listTroopModel)
	{
		_listTroopModel = listTroopModel;
		_troopDict = new Dictionary<OPCharacter, GameObject>();
		foreach(OPCharacter troopModel in _listTroopModel)
		{
			_troopDict.Add(troopModel, createTroopObject(troopModel));
		}
	}

	/// <summary>
	/// Creates the list of troop controllers
	/// </summary>
	/// <returns>The list of troop controllers</returns>
	/// <param name="parent">Parent game object</param>
	/// <param name="localPosition">Local position of troop</param>
	/// <param name="direction">Direction of troop</param>
	public List<CharacterController> loadTroops(GameObject parent, List<Vector3> localPosition, Vector3 direction)
	{
		int countOfTroop = UnityEngine.Random.Range(1, Config.COUNT_OF_TROOPS + 1);
		OPDebug.Log("Load " + countOfTroop + " troops");
		List<CharacterController> troopList = new List<CharacterController>();
		for(int i = 0; i < countOfTroop; i++)
		{
			int troopKind = UnityEngine.Random.Range(0, _listTroopModel.Count);
			OPCharacter troopModel = _listTroopModel[troopKind];
			GameObject troopObj = _troopDict[troopModel].Spawn();
			CharacterController troopController = troopObj.GetComponentInChildren<CharacterController>();
			if(troopController == null)
			{
				throw new UnityException("Could not load " + troopModel.CharacterName + " from pools");
			}
			troopController.monsterModel = troopModel;
			troopObj.transform.parent = parent.transform;
			troopObj.transform.localPosition = localPosition[i];
			troopObj.transform.localScale = Config.COMMON_LOCAL_SCALE;
			troopList.Add(troopController);
		}
		return troopList;
	}

}