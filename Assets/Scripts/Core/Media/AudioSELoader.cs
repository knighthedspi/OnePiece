using UnityEngine;
using System.Collections;

public class AudioSELoader : MonoBehaviour {

    public enum InstanceType {
        Child,
        Sibiling,
    }

    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    public bool stopOnDisable;
    public bool enablePositionEffect;
    public InstanceType type;
    public GameObject target;
    [HideInInspector]
    public bool isVoice;

    private AudioSE _se;

//	public delegate void EndHandler(string clipName);
//	public event EndHandler endHandler = delegate{ };

    void Awake() {
        if(!enablePositionEffect) return;

        _se = GameObjectUtility.AddChild<AudioSE>("SE", target);
        if(type == InstanceType.Sibiling) _se.transform.parent = target.transform.parent;
        
        _se.enableCache = false;
        _se.Init(clip, volume);
        _se.audio.rolloffMode = AudioRolloffMode.Linear;
        _se.isVoice = isVoice;
    }

    void OnEnable() {
        Play();
    }

    void OnDisable() {
        if(!stopOnDisable) return;
        _se.StopWithNotice();
    }

    void OnDestory() {
        if(!enablePositionEffect) return;
        
        if(_se.audio.isPlaying) _se.autoDestory = true;
        else Destroy(_se.gameObject);
    }

	public void Play() {
		if (enablePositionEffect) SoundManager.Instance.PlaySE(_se);
		else _se = SoundManager.Instance.PlaySE(clip, volume);

//		if(_se.audio.clip != null)
//			SoundManager.Instance.endHandler += (string clipName) => endHandler(clipName);
	}
}
