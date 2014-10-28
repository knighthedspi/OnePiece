using UnityEngine;                                                                                                                                                                                             
using UnityEditor;
using System.Collections;
using System.IO;
 
public class BatchBuild
{
	private static string[] scenes = GetScenes ();

    [UnityEditor.MenuItem("Build/Android Development")]
    public static void AndroidDevelopmentBuild(){
        AndroidBuild (true);
    }
        
    private static bool AndroidBuild (bool isDebug){
        FileUtil.DeleteFileOrDirectory ("Target/androidBuild");
        Directory.CreateDirectory ("Target/androidBuild");
        EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.Android);
        BuildOptions opt = BuildOptions.None;
        if (isDebug)
            opt |= BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
 
        string errorMsg = BuildPipeline.BuildPlayer (scenes, "Target/androidBuild/nc.apk", BuildTarget.Android, opt);
        if (string.IsNullOrEmpty (errorMsg)) {
            Debug.Log ("Build Android succeeded");
            return true;
        }
        Debug.Log ("Build Android failed");
        Debug.LogError (errorMsg);
        return false;
    }
 
    private static string[] GetScenes(){
        ArrayList levels = new ArrayList ();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
            if (scene.enabled) {
                levels.Add (scene.path);
            }
        }
        return (string[])levels.ToArray (typeof(string));
    }
}