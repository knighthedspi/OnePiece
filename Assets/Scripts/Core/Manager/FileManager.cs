using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class FileManager {

    private readonly static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void CreateDirectory(string path){
        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public static void DeleteFile(string path){
        if(File.Exists(path))
            File.Delete(path);
    }

    public static bool IsDirectory(string path){
        return Directory.Exists(path);
    }

    public static bool Exists(string path){
        return File.Exists(path);
    }
	
    public static void WriteBinary(string path, byte[] bytes) {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
            CreateDirectory(dir);

        File.WriteAllBytes(path, bytes);
    }
	
    public static void WriteBinary(string path, byte[] bytes, long timeStamp) {
        WriteBinary(path, bytes);
        SetTimeStamp(path, timeStamp);
    }
	
    public static byte[] ReadBinary(string path){
		if (!Exists(path))
            return null;
        return File.ReadAllBytes(path);
    }

    public static long GetTimeStamp(string path){
        if(!File.Exists(path))
			return 0;
        DateTime dtUpdate =  File.GetLastWriteTimeUtc(path);
        long timestamp = (long)dtUpdate.Subtract(UNIX_EPOCH).TotalSeconds;
        return timestamp;
    }

	public static void SetTimeStamp(string path, long timeStamp){
        if(!File.Exists(path))
			return;

        DateTime lastWriteTime = UNIX_EPOCH.AddSeconds(timeStamp);
        File.SetLastWriteTimeUtc(path, lastWriteTime);
    }

    public static string GetDataPath(string path) {
        string full_path = Application.dataPath + "/" + path;
        return full_path;
    }
	
    public static string GetPersistentDataPath(string path) {
        string full_path = Application.persistentDataPath + "/" + path;

#if UNITY_IPHONE
        iPhone.SetNoBackupFlag(full_path);
#endif
        return full_path;
    }

    public static string GetCachePath(string path) {
        return Application.temporaryCachePath + "/" + path;
    }
}