using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OPItemDAO : OPItem{

    private static int CONSUMTION_ITEM = 2;
    private static string tableName = "OPItem";
    private static OPItemDAO sInstance;
    private static SQLiteDB db;
    
    public static OPItemDAO Instance {
        get {
            if(sInstance == null){
                sInstance = new OPItemDAO();
                db = DBManager.MasterDb;
            }
            return sInstance;
        }
    }

    public Dictionary<OPItem, int> getUserItems(int level)
    {
        Dictionary<OPItem, int> listItems = new Dictionary<OPItem, int>();
        List<OPItem> allItems = getAllConsumptionItems();
        foreach(OPItem item in allItems){
            int unlock = 0;
            if(level >= item.UnlockLevel)
                unlock = 1;
            listItems.Add(item, unlock);
        }
        return listItems;
    }

    public List<OPItem> getAllConsumptionItems()
    {
        string query = "SELECT * FROM " + tableName + " where category = " + CONSUMTION_ITEM;
        List<OPItem> result = GenericDao<OPItem>.Instance.Get(db, query);
        if(result.Count < 1)
        {
            throw new Exception("Item not found");
        }
        return result;
    }

}
