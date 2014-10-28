using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioSE : MonoBehaviour {

    [HideInInspector]
    public bool isVoice;

    public bool enableCache {
        get { return _enableCache; }
        set { _enableCache = value; }
    }

    public bool autoDestory {
        set { _autoDestory = value; }
    }

    private bool _enableCache;
    private bool _autoDestory;
    private float _volume;

	public delegate void EndHandler(string clipName);
	public event EndHandler endHandler = delegate{ };

    void Update() {
        if(audio.isPlaying) return;
        StopWithNotice();
    }

    public void Init(AudioClip clip, float volume) {
        Stop();
        audio.clip = clip;
        _volume = volume;
    }

    public void Play(bool mute, float seVolume, float voiceVolume) {
        enabled = true;
        audio.enabled = true;
        audio.mute = mute;
        audio.volume = isVoice ? voiceVolume : seVolume * _volume;
        audio.Play();
    }

    public void OnChangeVolume(bool mute, float seVolume, float voiceVolume) {
        audio.mute = mute;
        audio.volume = isVoice ? voiceVolume : seVolume * _volume;
    }

    public void Stop() {
        enabled = false;
        audio.Stop();
        audio.enabled = false;
    }

    public void StopWithNotice() {
        Stop();
        SoundManager.Instance.OnStopSE(this);
		if(audio.clip != null)
			endHandler(audio.clip.name);
        if(_autoDestory) Destroy(gameObject);
    }
}
