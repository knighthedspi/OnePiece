using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EditorScriptUtility {
    
    public static bool IsDirectory (Object obj){
        return Directory.Exists (AssetDatabase.GetAssetPath (obj));
    }
    
}
