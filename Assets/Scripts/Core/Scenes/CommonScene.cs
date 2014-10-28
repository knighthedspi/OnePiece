using UnityEngine;
using System.Collections;

public class CommonScene : MonoBehaviour {

    void Start () {
		QualitySettings.vSyncCount  = Config.VSYNC_COUNT;
        Application.targetFrameRate = Config.TARGET_FRAMERATE;
    }

}
