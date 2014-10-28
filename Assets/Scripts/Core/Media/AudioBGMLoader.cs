using UnityEngine;
using System.Collections;

public class AudioBGMLoader : MonoBehaviour {

    public AudioClip clip;

    void OnEnable() {
        SoundManager.Instance.PlayBGM(clip);
    }

	public void Play() {
		SoundManager.Instance.PlayBGM(clip);
	}
}
