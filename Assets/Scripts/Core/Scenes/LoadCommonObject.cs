using UnityEngine;
using System.IO;
using System.Collections;

public class LoadCommonObject : MonoBehaviour {

    private void Awake(){
        if(GameObject.FindGameObjectWithTag(Config.TAG_COMMON) == null)
			Application.LoadLevelAdditive(Config.SCENE_COMMON);
    }
    
}
