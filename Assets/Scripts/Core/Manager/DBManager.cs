using UnityEngine;
using System.Collections;
using System.IO;

public class DBManager : Singleton<DBManager>
{

    private static string MASTER_DB_STREAMING_PATH;
    private static string MASTER_DB_PERSISTENT_PATH;
    private static string USER_DB_STREAMING_PATH;
    private static string USER_DB_PERSISTENT_PATH;
    private static string STREAMING_ASSET_PATH;
    private static string PERSISTENT_DATA_PATH;

    public readonly static SQLiteDB MasterDb = new SQLiteDB();
    public readonly static SQLiteDB UserDb = new SQLiteDB();

    public delegate void OnInitComplete();
    public OnInitComplete onInitComplete;

    void Awake()
    {
        SetAssetPath();
        SetDBPath();
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(UpdateDatabase());
        OPDebug.Log("Load database");
        MasterDb.Open(MASTER_DB_PERSISTENT_PATH);
        UserDb.Open(USER_DB_PERSISTENT_PATH);
        yield return new WaitForSeconds(1);
        onInitComplete();
    }

    private bool checkUpdate()
    {
        //TODO : check 4 update
        return true;
    }

    /// <summary>
    /// Updates the database.
    /// </summary>
    /// <returns>The database.</returns>
    private IEnumerator UpdateDatabase()
    {
        OPDebug.Log("update database");
        if(checkUpdate()) {
            yield return StartCoroutine(CopyFile(MASTER_DB_STREAMING_PATH, MASTER_DB_PERSISTENT_PATH));	
			//yield return StartCoroutine(CopyFile(USER_DB_STREAMING_PATH, USER_DB_PERSISTENT_PATH));
        } else {
            OPDebug.Log("dont need to update " + MASTER_DB_PERSISTENT_PATH + ";" + USER_DB_PERSISTENT_PATH);
        }

        OPDebug.Log("update " + MASTER_DB_PERSISTENT_PATH + ";" + USER_DB_STREAMING_PATH + " done");
    }

    /// <summary>
    /// Sets the asset path.
    /// </summary>
    private void SetAssetPath()
    {
#if UNITY_EDITOR
        STREAMING_ASSET_PATH = Application.dataPath + "/StreamingAssets/";
        PERSISTENT_DATA_PATH = Application.dataPath + "/PersistentDataPath/";
#elif UNITY_ANDROID
		STREAMING_ASSET_PATH = "jar:file://" + Application.dataPath + "!/assets/";
		PERSISTENT_DATA_PATH = Application.persistentDataPath + "/";
#elif UNITY_IPHONE
		STREAMING_ASSET_PATH = "file://" + Application.streamingAssetsPath + "/";
		PERSISTENT_DATA_PATH = Application.persistentDataPath + "/";
#endif
    }

    /// <summary>
    /// Sets the DB path.
    /// </summary>
    private void SetDBPath()
    {
        MASTER_DB_STREAMING_PATH = STREAMING_ASSET_PATH + "master.db";
        MASTER_DB_PERSISTENT_PATH = PERSISTENT_DATA_PATH + "master.db";
        USER_DB_STREAMING_PATH = STREAMING_ASSET_PATH + "user.db";
        USER_DB_PERSISTENT_PATH = PERSISTENT_DATA_PATH + "user.db";
    }

    /// <summary>
    /// Copies the file.
    /// </summary>
    /// <param name="originalPath">Original path.</param>
    /// <param name="copyPath">Copy path.</param>
    IEnumerator CopyFile(string originalPath,string copyPath)
    {
        OPDebug.Log("copy data from " + originalPath + " to " + copyPath);
        byte[] data;
        if(originalPath.Contains("://")) {
            WWW www = new WWW(originalPath);
            yield return www;
            data = www.bytes;
        } else {
            data = System.IO.File.ReadAllBytes(originalPath);
        }
        File.WriteAllBytes(copyPath, data);
    }

    void OnDestroy()
    {
        MasterDb.Close();
        UserDb.Close();
    }

}
