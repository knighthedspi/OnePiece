using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace OPUnitTest{
	
	[TestFixture()]
	public class OPCharacterTest
	{
		[Test()]
		public void TestSelectCharacter ()
		{
			string dbpath  = Application.streamingAssetsPath + "/test_master.db";
			var db = new SQLiteDB();
			db.Open(dbpath);
			GenericDao<OPCharacter>.Instance.Drop(db);
			Debug.Log("Create db");
			GenericDao<OPCharacter>.Instance.Create(db);

			OPCharacter character = new OPCharacter();
			character.Id = 1;
			character.CharacterName = "Luffy";
//			character.LevelID = 1;
			
			GenericDao<OPCharacter>.Instance.Put(db, character);
//			
//			List<OPCharacter> result = GenericDao<OPCharacter>.Instance.Get(db, "select * from OPCharacter where id = 1");
//			
//			Assert.AreEqual(1, result[0].Id);
//			Assert.AreEqual("Luffy", result[0].CharacterName);
//			Assert.AreEqual(1, result[0].LevelID);
		}
	}
}