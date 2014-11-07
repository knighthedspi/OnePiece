
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OPCharacterDAO : OPCharacter{
	
	private static int KINDID_CHARACTER = 1;
	//#TODO get class name static from base class
	private static string tableName = "OPCharacter";
	private static OPCharacterDAO sInstance;
	private static SQLiteDB db;

	public static OPCharacterDAO Instance {
		get {
			if(sInstance == null){
				sInstance = new OPCharacterDAO();
				db = DBManager.MasterDb;
			}
			return sInstance;
		}
	}

	/// <summary>
	/// Gets the character base on Userlevel.
	/// #TODO user QueryCondition
	/// </summary>
	/// <returns>The character by level.</returns>
	/// <param name="level">Level.</param>
	public OPCharacter getCharacterByLevel(int level)
	{
		string query = "SELECT * FROM " + tableName + " where kindId = " + KINDID_CHARACTER + " and evolution = " + level;
		List<OPCharacter> result = GenericDao<OPCharacter>.Instance.Get(db, query);
		if(result.Count < 1)
		{
			throw new Exception("Character not found at level: " + level);
		}
		return result[0];
	}

	//#TODO must design how to get monster
	//#TODO add order column not using id order
	public OPCharacter getNextMonster(int currentMonsterID = 0)
	{
		string query = "SELECT * FROM " + tableName + " where kindId != " + KINDID_CHARACTER  + " and id > " + currentMonsterID + " order by id ASC";
		List<OPCharacter> result = GenericDao<OPCharacter>.Instance.Get(db, query);
		if(result.Count < 1)
		{
			throw new Exception("Character not found at level: ");
		}
		return result[0];
	}
}