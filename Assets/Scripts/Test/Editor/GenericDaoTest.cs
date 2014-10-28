using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System;


namespace UnityTest{

[TestFixture]
[Category ("GenericDao Tests")]
public class GenericDaoTest{

	[Test]
	[Category ("Select")]
	public void SelectTest () {
		string dbpath  = Application.streamingAssetsPath + "/master.db";
		var db = new SQLiteDB();
		db.Open(dbpath);

		GenericDao<Hoge>.Instance.Drop(db);
		GenericDao<Hoge>.Instance.Create(db);

		Hoge hoge = new Hoge();
		hoge.HogeId = 1;
		hoge.HogeName = "水";
		Debug.Log(hoge.HogeName);
		GenericDao<Hoge>.Instance.Put(db, hoge);

		var list = new List<Hoge>(){hoge};
		QueryCondition<Hoge> condition = new QueryCondition<Hoge>();
		condition.WithEqualConditions(list);
		int count = GenericDao<Hoge>.Instance.Count(db, condition);
		Debug.Log(count);

		condition.WithGroups(new string[]{"hogeId", "hogeName"})
					.WithOrders(new Dictionary<string, string>(){{"hogeId", "ASC"}, {"hogeName", "DESC"}});
		
		List<Hoge> hoges = GenericDao<Hoge>.Instance.Get(db, condition);
		
		foreach(var hg in hoges){
			Debug.Log (hg.HogeId);
			Debug.Log (hoge.HogeName);
		}

		Hoge hoge2 = new Hoge();
		hoge2.HogeId = 1;
		hoge2.HogeName = "火";
		GenericDao<Hoge>.Instance.Update(db, hoge2, condition);

		db.Close();
		
		string userDbpath  = Application.streamingAssetsPath + "/user.db";
		var userDb = new SQLiteDB();
		userDb.Open(userDbpath);
		var userParties = GenericDao<UserParty>.Instance.Get(userDb);
		var userUnitParties = new List<UserUnitParty>(); 
		foreach(var userParty in userParties){
			var userUnit = new UserUnit();
			userUnit.UserUnitId = userParty.UserUnitId;
			QueryCondition<UserUnit> cond = new QueryCondition<UserUnit>();
			cond.WithEqualConditions(new List<UserUnit>(){userUnit});
			List<UserUnit> userUnits = GenericDao<UserUnit>.Instance.Get(userDb, cond);
			
		}


	}
}

}