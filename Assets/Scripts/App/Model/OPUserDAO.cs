
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OPUserDAO : OPUser
{

    private static string tableName = "OPUser";
    private static OPUserDAO sInstance;
    private static SQLiteDB db;
    private const int MAX_HEALTH = 5;

    public static OPUserDAO Instance {
        get {
            if(sInstance == null) {
                sInstance = new OPUserDAO();
                db = DBManager.UserDb;
                if(!_isTableExist()) {
					createTable();
                }
            }
            return sInstance;
        }
    }


    public static OPUser GetUser(string fbID)
    {
		string query = "select * from OPUser where fbId = '" + fbID + "'";
		List<OPUser> users = GenericDao<OPUser>.Instance.Get(db, query);
		if(users.Count < 0) {
			return null;
		}
		return users[0];
    }

	public void Save (Dictionary<string,string> userFB)
	{
		OPUser user = new OPUser();
		user = GetUser (userFB["id"]);
		if (user != null)
		{
			user.UserName = userFB["name"];
			user.LastName = userFB["last_name"];
			user.FirstName = userFB["first_name"];
			Update (user);
		}
		else
		{
			CreateNewUserFB (userFB);
		}

	}
    public bool Update(OPUser user)
    {
        try {
            GenericDao<OPUser>.Instance.Update(db, user, new QueryCondition<OPUser>());
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    /// <summary>
    /// create user
    /// </summary>
    private void CreateNewUserFB(Dictionary<string,string> userFB)
    {
        //create user
        OPUser user = new OPUser();
		user.UserName = userFB["name"];
		user.LastName = userFB["last_name"];
		user.FirstName = userFB["first_name"];
		user.FbId = userFB["id"];
		user.Exp = 0;
        user.LevelId = 1;
        user.Health = MAX_HEALTH;
        user.Score = 0;
        user.HighScore = 0;
        user.CurrentMonsterID = 0;
        user.CreatedAt = TimeStampUtility.convertTimeToInt(DateTime.UtcNow);
        user.UpdatedAt = TimeStampUtility.convertTimeToInt(DateTime.UtcNow);
        GenericDao<OPUser>.Instance.Put(db, user);
    }
	
    private static void createTable()
    {
        GenericDao<OPUser>.Instance.Create(db);
    }

    private static bool _isTableExist()
    {
        return false;
    }
}