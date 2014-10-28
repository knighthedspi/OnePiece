using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InputKeyChecker : Singleton<InputKeyChecker> {

    void Update() {

        //アンドロイドの場合のみ、バックキーなど対応する
        if (Application.platform == RuntimePlatform.Android) {
            //バックキーの対応
            if ( Input.GetKey(KeyCode.Escape) ) {
                //****未実装
            }
        }

    }

}
