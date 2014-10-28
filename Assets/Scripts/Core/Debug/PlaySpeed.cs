using UnityEngine;
using System.Collections;

public class PlaySpeed : MonoBehaviour {
#if UNITY_EDITOR
    public float playSpeed = 1f;

    void Update() {
        Time.timeScale = playSpeed;
    }
#endif
}
