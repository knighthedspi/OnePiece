using UnityEngine;
using System.Collections;
using System.IO;

public class DBManager : Singleton<DBManager> {

	private static string MASTER_DB_STREAMING_PATH;
	private static string MASTER_DB_PERSISTENT_PATH;
	private static string USER_DB_STREAMING_PATH;
	private static string USER_DB_PERSISTENT_PATH;

	public readonly static SQLiteDB MasterDb = new SQLiteDB();
	public readonly static SQLiteDB UserDb   = new SQLiteDB();

	void Awake(){
		// TODO : create and load db 
//		MASTER_DB_STREAMING_PATH  = Application.streamingAssetsPath + "/master.db";
//		MASTER_DB_PERSISTENT_PATH = Application.persistentDataPath  + "/master.db";
//		if(!File.Exists(MASTER_DB_PERSISTENT_PATH))
//			StartCoroutine(CopyFile(MASTER_DB_STREAMING_PATH, MASTER_DB_PERSISTENT_PATH));
//
//		USER_DB_STREAMING_PATH  = Application.streamingAssetsPath + "/user.db";
//		USER_DB_PERSISTENT_PATH = Application.persistentDataPath  + "/user.db";
//		if(!File.Exists(USER_DB_PERSISTENT_PATH))
//			StartCoroutine(CopyFile(USER_DB_STREAMING_PATH, USER_DB_PERSISTENT_PATH));
//
//		MasterDb.Open(MASTER_DB_STREAMING_PATH);
//		UserDb.Open(USER_DB_STREAMING_PATH);
//		MasterDb.Open(MASTER_DB_PERSISTENT_PATH);
//		UserDb.Open(USER_DB_PERSISTENT_PATH);
	}

	IEnumerator CopyFile(string originalPath, string copyPath){
		WWW www = new WWW("file://" + originalPath);
		yield return www;
		File.WriteAllBytes(copyPath, www.bytes);
	}

	void OnDestroy() {
		MasterDb.Close();
		UserDb.Close();
	}
    
}
