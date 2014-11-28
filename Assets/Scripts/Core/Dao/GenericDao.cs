using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Community.CsharpSqlite;
using System.Text;
using System.Reflection;
using System;

public class GenericDao<T>
{    
    
    protected static GenericDao<T>  sInstance;
    protected static FieldInfo[]    sFields;

    public static GenericDao<T> Instance {
        get {
            if(sInstance == null) {
                sInstance = new GenericDao<T>();
                sFields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            }
            return sInstance;
        }
    }

    public void Drop(SQLiteDB db)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("DROP TABLE IF EXISTS ").Append(typeof(T).Name);
        var qr = new SQLiteQuery(db, sb.ToString()); 
        qr.Step();                                                
        qr.Release();
    }

    public void Create(SQLiteDB db)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("CREATE TABLE IF NOT EXISTS ").Append(typeof(T).Name).Append("(");
        int i = 0;
        foreach(FieldInfo f in sFields) {
            if(i > 0)
                sb.Append(",");
            sb.Append(f.Name).Append(" ");

            if(f.FieldType == typeof(string)) {
                sb.Append("text ");
            } else if(f.FieldType == typeof(byte[])) {
                sb.Append("blob ");
            } else {
                sb.Append("integer ");
            }

            var attrs = (CustomField[])f.GetCustomAttributes(typeof(CustomField), false);
            foreach(var attr in attrs) {
                sb.Append(attr.Statement).Append(" ");
            }
            i++;
        }
        sb.Append(")");
        Debug.Log(sb.ToString());
        var qr = new SQLiteQuery(db, sb.ToString()); 
        qr.Step();                                                
        qr.Release();
    }
    
    public List<T> Get(SQLiteDB db,string statement)
    {
        var qr = new SQLiteQuery(db, statement);
        List<T> objs = new List<T>();
        while(qr.Step()) {
            T obj = (T)Activator.CreateInstance(typeof(T), new object[] {});
            foreach(FieldInfo f in sFields) {
                ModelObject<T>.SetField(obj, f, FieldData(qr, f));
            }
            objs.Add(obj);
        }
        qr.Release();
        qr = null;
        return objs;
    }

    public int Count(SQLiteDB db,string statement)
    {
        var qr = new SQLiteQuery(db, statement);
        int count = 0;
        while(qr.Step()) {
            count = qr.GetInteger("count");
        }
        qr.Release();
        qr = null;
        return count;
    }

    public List<T> Get(SQLiteDB db,QueryCondition<T> condition)
    {
        return Get(db, BuildSelectStatement(condition));
    }

    public List<T> Get(SQLiteDB db)
    {
        QueryCondition<T> condition = new QueryCondition<T>();
        return Get(db, BuildSelectStatement(condition));
    }


    public int Count(SQLiteDB db,QueryCondition<T> condition)
    {
        return Count(db, BuildSelectStatement(condition, true));
    }

    private string BuildSelectStatement(QueryCondition<T> condition,bool isCount = false)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("SELECT ");
        if(isCount)
            sb.Append("COUNT(");
        if(condition.Fields != null)
            sb.Append(BuildFieldStatement(condition.Fields));
        else
            sb.Append("*");
        if(isCount)
            sb.Append(") AS count");
        sb.Append(" FROM ")
            .Append(condition.Tablename)
                .Append(BuildWhereStatement(condition))
                .Append(BuildGroupStatement(condition.Groups))
                .Append(BuildOrderStatement(condition.Orders));
        if(!isCount) {
            sb.Append(" LIMIT ").Append(condition.Limit)
                .Append(" OFFSET ").Append(condition.Offset);
        }
        Debug.Log(sb.ToString());
        return sb.ToString();
    }
    
    public void Put(SQLiteDB db,T obj)
    {
        var qr = new SQLiteQuery(db, BuildInsertStatement(obj)); 
        qr.Step();
        qr.Release();
    }

    private string BuildInsertStatement(T obj)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("INSERT INTO ");
        sb.Append(typeof(T).Name);
        sb.Append("(");
        StringBuilder sbVal = new StringBuilder();
        int i = 0;
        var props = obj.GetType().GetProperties();
        foreach(PropertyInfo propInfo in props) {
            object value = propInfo.GetValue(obj, null);
            if(value == null)
                continue;
            if(i != 0 && i < props.Length) {
                sb.Append(", ");
                sbVal.Append(", ");
            }
            i++;
            sb.Append(propInfo.Name);
            if(propInfo.PropertyType == typeof(string)) {
                sbVal.Append("'").Append((string)propInfo.GetValue(obj, null)).Append("'");
            } else {
                sbVal.Append((int)propInfo.GetValue(obj, null));
            }
        }
        sb.Append(") "); 
        sb.Append("VALUES(");
        sb.Append(sbVal.ToString());
        sb.Append(")");
        Debug.Log(sb.ToString());
        return sb.ToString();
    }
    
    public void Update(SQLiteDB db,T obj,QueryCondition<T> condition)
    {
        var qr = new SQLiteQuery(db, BuildUpdateStatement(obj, condition)); 
        qr.Step();
        qr.Release();
    }

    private string BuildUpdateStatement(T obj,QueryCondition<T> condition)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("UPDATE ").Append(typeof(T).Name).Append(" SET ");
        var props = obj.GetType().GetProperties();
        int i = 0;
        foreach(PropertyInfo propInfo in props) {
            object value = propInfo.GetValue(obj, null);
            if(value == null)
                continue;

            if(i > 0)
                sb.Append(",");

            sb.Append(propInfo.Name).Append(" = ");
            if(propInfo.PropertyType == typeof(string)) {
                sb.Append("'").Append((string)propInfo.GetValue(obj, null)).Append("'");
            } else {
                sb.Append(propInfo.GetValue(obj, null));
            }
            sb.Append(" ");
            i++;
        }
        sb.Append(BuildWhereStatement(condition));
        Debug.Log(sb.ToString());
        return sb.ToString();
    }

    public void Delete(SQLiteDB db,List<T> conditions)
    {
        var qr = new SQLiteQuery(db, BuildDeleteStatement(conditions)); 
        qr.Step();
        qr.Release();
    }

    private string BuildDeleteStatement(List<T> conditions)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("DELETE FROM ").Append(typeof(T).Name).Append(" ").Append(BuildWhereStatement(conditions));
        return sb.ToString();
    }

    private String BuildWhereStatement(QueryCondition<T> condition)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("");
        int i = 0;

        StringBuilder sbcomp = new StringBuilder();
        sbcomp.Append(BuildComparingStatement(ref i, "=", condition.EqualConditions));
        sbcomp.Append(BuildComparingStatement(ref i, ">=", condition.EqualLessThanConditions));
        sbcomp.Append(BuildComparingStatement(ref i, "<=", condition.EqualGreaterThanConditions));
        sbcomp.Append(BuildComparingStatement(ref i, ">", condition.LessThanConditions));
        sbcomp.Append(BuildComparingStatement(ref i, "<", condition.GreaterThanConditions));

        if(i > 0)
            sb.Append(" WHERE ").Append(sbcomp.ToString());

        return sb.ToString();
    }
    
    private String BuildWhereStatement(List<T> conditions)
    {
        if(conditions == null || conditions.Count <= 0)
            return "";
        StringBuilder sb = new StringBuilder();
        sb.Append(" WHERE ");
        int i = 0;
        foreach(T obj in conditions) {
            var props = obj.GetType().GetProperties();
            foreach(PropertyInfo propInfo in props) {
                object value = propInfo.GetValue(obj, null);
                if(value == null)
                    continue;
                if(i > 0)
                    sb.Append(" And ");
                sb.Append(propInfo.Name).Append(" = ");
                if(propInfo.PropertyType == typeof(string)) {
                    sb.Append("'").Append(propInfo.GetValue(obj, null)).Append("'");
                } else {
                    sb.Append(propInfo.GetValue(obj, null));
                }
                sb.Append(" ");
                i++;
            }
        }
        return sb.ToString();
    }

    private String BuildComparingStatement(ref int i,string comparing,List<T> conditions)
    {
        if(conditions == null)
            return "";
        StringBuilder sb = new StringBuilder();
        foreach(T obj in conditions) {
            var props = obj.GetType().GetProperties();
            foreach(PropertyInfo propInfo in props) {
                object value = propInfo.GetValue(obj, null);
                if(value == null)
                    continue;
                if(i > 0)
                    sb.Append(" And ");
                sb.Append(propInfo.Name).Append(" ").Append(comparing).Append(" ");
                if(propInfo.PropertyType == typeof(string)) {
                    sb.Append("'").Append(propInfo.GetValue(obj, null)).Append("'");
                } else {
                    sb.Append(propInfo.GetValue(obj, null));
                }
                sb.Append(" ");
                i++;
            }
        }
        return sb.ToString();
    }
    
    private String BuildGroupStatement(string[] groups)
    {
        if(groups == null || groups.Length <= 0)
            return "";
        
        StringBuilder sb = new StringBuilder();
        int i = 1;
        foreach(string group in groups) {
            if(i == 1)
                sb.Append(" GROUP BY ");
            
            sb.Append(group);
            if(i < groups.Length)
                sb.Append(", ");
            
            i++;
        }
        return sb.ToString();
    }

    private String BuildOrderStatement(Dictionary<string, string> orders)
    {
        if(orders == null || orders.Count <= 0)
            return "";
        
        StringBuilder sb = new StringBuilder();
        int i = 1;
        foreach(var order in orders) {
            if(i == 1)
                sb.Append(" ORDER BY ");
            
            sb.Append(order.Key);
            sb.Append(" ");
            sb.Append(order.Value);
            if(i < orders.Count)
                sb.Append(", ");
            
            i++;
        }
        return sb.ToString();
    }

    
    private String BuildFieldStatement(string[] fields = null)
    {
        StringBuilder sb = new StringBuilder();
        int i = 1; 
        if(fields == null || fields.Length <= 0) {
            foreach(FieldInfo f in sFields) {
                sb.Append(f.Name);
                if(i < fields.Length)
                    sb.Append(", ");
                i++;
            }
        } else {
            foreach(string field in fields) {
                sb.Append(field);
                if(i < field.Length)
                    sb.Append(", ");
                i++;
            }    
        }
        return sb.ToString();
    }
    
    private object FieldData(SQLiteQuery qr,FieldInfo f)
    {
        if(Sqlite3.SQLITE_TEXT == qr.GetFieldType(f.Name))
            return (object)qr.GetString(f.Name);

        if(Sqlite3.SQLITE_INTEGER == qr.GetFieldType(f.Name))
            return (object)qr.GetInteger(f.Name);

        if(Sqlite3.SQLITE_FLOAT == qr.GetFieldType(f.Name))
            return (object)qr.GetDouble(f.Name);

        if(Sqlite3.SQLITE_BLOB == qr.GetFieldType(f.Name))
            return (object)qr.GetBlob(f.Name);

        return null;
    }
}
