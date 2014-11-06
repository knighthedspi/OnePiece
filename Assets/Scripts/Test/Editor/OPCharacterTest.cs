using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace OPUnitTest{
	
	[TestFixture()]
	public class OPCharacterTest
	{
		private static SQLiteDB db;

		[SetUp()]
		public void tearUp()
		{
			//setup DB
			string dbpath  = Application.streamingAssetsPath + "/test_master.db";
			db = new SQLiteDB();
			db.Open(dbpath);

			GenericDao<OPCharacter>.Instance.Create(db);
		}
		[TearDown()]
		public void tearDown()
		{
			//drop db and close connection
			GenericDao<OPCharacter>.Instance.Drop(db);
			db.Close();
		}

//		[Test()]
//		public void TestSelectCharacter ()
//		{
//
//			OPCharacter character = new OPCharacter();
//			character.Id = 1;
//			character.CharacterName = "Luffy";
//			GenericDao<OPCharacter>.Instance.Put(db, character);
//			
//			List<OPCharacter> result = GenericDao<OPCharacter>.Instance.Get(db, "select * from OPCharacter where id = 1");
//			
//			Assert.AreEqual(1, result[0].Id);
//			Assert.AreEqual("Luffy", result[0].CharacterName);
//			Assert.AreEqual(1, result[0].LevelID);
//		}
	
		[Test()]
		public void TestSelectCharacterByQueryCondition()
		{
			OPCharacterDAO.Instance.getCharacterByLevel(1);
//			OPDebug.Log(result[1]);
		}
	}
}