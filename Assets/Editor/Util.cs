using UnityEngine;
using System.Collections;
using UnityEditor;

public class Util : Editor {
	#if UNITY_EDITOR
	[UnityEditor.MenuItem("Edit/SavePrefab %&s")]
	static void SavePrefab(){
		AssetDatabase.SaveAssets();
	}
	#endif
}
