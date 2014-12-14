using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OPLevelDAO : OPLevel{

    private static string tableName = "OPLevel";
    private static OPLevelDAO sInstance;
    private static SQLiteDB db;
    
    public static OPLevelDAO Instance {
        get {
            if(sInstance == null){
                sInstance = new OPLevelDAO();
                db = DBManager.MasterDb;
            }
            return sInstance;
        }
    }

    public OPLevel GetLevelByID(int id)
    {
        string query = "SELECT * FROM " + tableName + " where id = " + id;
        List<OPLevel> result = GenericDao<OPLevel>.Instance.Get(db, query);
        if(result.Count < 1)
        {
            throw new Exception("Item not found");
        }
        return result[0];
    }

}
