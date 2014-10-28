using UnityEngine;
using System.Collections;

public class AudioENVLoader : MonoBehaviour {

    public AudioClip clip;

    void OnEnable() {
        SoundManager.Instance.PlayENV(clip);
    }

    void OnDisable() {
        SoundManager.Instance.StopENV(clip);
    }
}
