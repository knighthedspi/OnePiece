
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
                    Debug.LogError("table not exist");
                    initUser();
                }
            }
            return sInstance;
        }
    }


    public OPUser getCurrentUser()
    {
        string query = "select * from OPUser limit 1";
        List<OPUser> users = GenericDao<OPUser>.Instance.Get(db, query);
        if(users.Count < 0) {
            throw new EntryPointNotFoundException("user not found");
        }
        return users[0];
    }

    public bool save(OPUser user)
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
    private static void initUser()
    {
        //Create table
        createTable();
        //create user
        OPUser user = new OPUser();
        user.UserName = "luffy"; //#TODO random some meaning name with each language
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
//		string query = "select name from sqlite_master where type='table' and name = " + tableName;
        //#TODO must implement right way
        return true;
    }
}