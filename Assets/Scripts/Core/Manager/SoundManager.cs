using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundManager  : Singleton<SoundManager> {

	private SoundVolume volume = new SoundVolume();

	public static bool isSound = false;
	public int numberOfConcurrentSE = 30;
	public int numberOfConcurrentENV = 3;

	private AudioSource BGMsource;
    private LinkedList<AudioSE> enabledSEsources;
    private Queue<AudioSE> cachedSEsources;

	private LinkedList<AudioSource> enabledENVsources;
	private Queue<AudioSource> cachedENVsources;

	public AudioClip[] BGM;
	public AudioClip[] SE;
	public AudioClip[] ENV;

	public List<string> BGM_list;
	public List<string> SE_list;
	public List<string> ENV_list;

	private GameObject audioSources;

	void Awake (){

		DontDestroyOnLoad(gameObject);
		audioSources = GameObjectUtility.AddChild("Audio Sources", gameObject);
		BGMsource    = GameObjectUtility.AddChild<AudioSource>("BGM", audioSources);
		BGMsource.loop = true;

		isSound = PlayerPrefs.GetInt("isSound",1) == 1 ? true : false;
//		volume.BGM	 = PlayerPrefs.GetFloat("bgm_value",   1f);
//		volume.SE    = PlayerPrefs.GetFloat("se_value",    1f);
//		volume.VOICE = PlayerPrefs.GetFloat("voice_value", 1f);
	
        enabledSEsources = new LinkedList<AudioSE>();
        cachedSEsources = new Queue<AudioSE>();
		
		enabledENVsources = new LinkedList<AudioSource>();
		cachedENVsources = new Queue<AudioSource>();
		
		for (int i = 0, il = BGM.Length; i < il; i++) {
			BGM_list.Add(BGM[i].name);
		}
		for (int i = 0, il = SE.Length; i < il; i++) {
			SE_list.Add(SE[i].name);
		}
		for (int i = 0, il = ENV.Length; i < il; i++) {
			ENV_list.Add(ENV[i].name);
		}
	}

	public bool ContainsBGM(string name) {
		int index = BGM_list.IndexOf(name);
		if (index >= 0)
			return true;
		return false;
	}
	
	public bool ContainsSE(string name) {
		int index = SE_list.IndexOf(name);
		if (index >= 0)
			return true;
		return false;
	}
	
	public void PlayBGM(string name,float vol=1.0f) {
		int index = BGM_list.IndexOf(name);
		if (index >= 0)
			PlayBGM(index,vol);
	}
	
	public void PlayBGM(int index,float vol){
		if(0 > index || BGM.Length <= index)
			return;
		
        PlayBGM(BGM[index],vol);
	}

	public void PlayBGM(AudioClip clip,float vol=1.0f) {
		if(!isSound)
			return;
		if(BGMsource.clip == clip)
			return;
		BGMsource.Stop();
		BGMsource.clip = clip;
		BGMsource.Play();
		BGMsource.volume = vol;
//		BGMsource.mute = true;
	}

	public void StopBGM(){
		BGMsource.Stop();
		BGMsource.clip = null;
	}

	public void PlaySE(string name){
		int index = SE_list.IndexOf(name);
		if (index >= 0)
			PlaySE(index);
	}

	public void PlaySE(string name, float vol){
		int index = SE_list.IndexOf(name);
		if (index >= 0)
			PlaySE(index, vol);
	}

	public void PlaySE(int index, float vol){
		if( 0 > index || SE.Length <= index )
			return;
		
		PlaySE(SE[index], vol);
	}

	public void PlaySE(int index){
		if( 0 > index || SE.Length <= index )
			return;

        PlaySE(SE[index]);
	}

    public AudioSE PlaySE(AudioClip clip, float volume = 1f) {
		if(!isSound)
			return null;
        AudioSE se;
        if(enabledSEsources.Count >= numberOfConcurrentSE) {
            se = enabledSEsources.First.Value;
            enabledSEsources.RemoveFirst();
        } else {
            if(cachedSEsources.Count > 0) {
                se = cachedSEsources.Dequeue();
                se.enabled = true;
            } else {
                se = GameObjectUtility.AddChild<AudioSE>("SE", audioSources);
                se.enableCache = true;
            }
        }
        enabledSEsources.AddLast(se);
        se.Init(clip, volume);
        se.Play(this.volume.Mute, this.volume.SE, this.volume.VOICE);
        return se;
    }

    public void PlaySE(AudioSE se) {
		if(!isSound)
			return;
        if(enabledSEsources.Count >= numberOfConcurrentSE) {
            se = enabledSEsources.First.Value;
            enabledSEsources.RemoveFirst();
            se.Stop();
            if(se.enableCache) cachedSEsources.Enqueue(se);
        }

        enabledSEsources.AddLast(se);
        se.Play(this.volume.Mute, this.volume.SE, this.volume.VOICE);
    }

	public void StopSE(){
        foreach(AudioSE observer in enabledSEsources) {
            cachedSEsources.Enqueue(observer);
            AudioSource source = observer.audio;
			source.Stop();
			source.clip = null;
		}
		enabledSEsources.Clear();
	}
	
	public void PlayENV(string name) {
		int index = ENV_list.IndexOf(name);
		if (index >= 0)
			PlayENV(index);
	}

	public void PlayENV(int index){
		if(0 > index || ENV.Length <= index)
			return;
        PlayENV(ENV[index]);
	}

    public void PlayENV(AudioClip clip) {
		if(!isSound)
			return;
        AudioSource source;
        if(enabledENVsources.Count >= numberOfConcurrentENV) {
            source = enabledENVsources.First.Value;
            source.Stop();
            enabledENVsources.RemoveFirst();
        } else {
            if(cachedENVsources.Count > 0) {
                source = cachedENVsources.Dequeue();
            } else {
                source = GameObjectUtility.AddChild<AudioSE>("ENV", audioSources).audio;
                source.loop = true;
            }
            source.mute = this.volume.Mute;
            source.volume = this.volume.ENV;
        }
        enabledENVsources.AddLast(source);
        source.clip = clip;
        source.Play();
    }
	
	public void StopENV(){
		foreach(AudioSource source in enabledENVsources) {
			source.Stop();
			source.clip = null;
			cachedENVsources.Enqueue(source);
		}
		enabledENVsources.Clear();
	}

    public void StopENV(AudioClip clip) {
        foreach(AudioSource source in enabledENVsources) {
            if(source.clip != clip) continue;
            enabledENVsources.Remove(source);
            source.clip = null;
            cachedENVsources.Enqueue(source);
            break;
        }
    }
	
	public SoundVolume VOLUME {
		get {
			return this.volume;
		}
		set {
			this.volume = VOLUME;
			this.BGMsource.mute = this.volume.Mute;
            this.BGMsource.volume = this.volume.BGM;
            foreach(AudioSE se in enabledSEsources) {se.OnChangeVolume(this.volume.Mute, this.volume.SE, this.volume.VOICE);
			}
			foreach(AudioSource source in enabledENVsources ){
				source.mute = this.volume.Mute;
                source.volume = this.volume.ENV;
			}
		}
	}

	public void OnEnableAudioListener(AudioListener listener) {
		audioSources.transform.parent = listener.transform;
		GameObjectUtility.Reset(audioSources.gameObject);
	}

	public void OnDisableAudioListener(AudioListener listener) {
		if(audioSources.transform.parent != listener.transform) return;
		audioSources.transform.parent = transform.parent;
		GameObjectUtility.Reset(audioSources.gameObject);
	}

    public void OnStopSE(AudioSE se) {
		enabledSEsources.Remove(se);
        if(se.enableCache) cachedSEsources.Enqueue(se);
    }

	public AudioSELoader CreateSELoader(string name, float vol) {
		AudioClip clip = Resources.Load<AudioClip>("Audio/SE/"+ name);
		AudioSELoader loader = GameObjectUtility.AddChild<AudioSELoader>(name, gameObject);
		loader.gameObject.SetActive(false);
		loader.clip = clip;
		loader.volume = vol;
		return loader;
	}

	public AudioSELoader CreateSELoader(string name, AudioClip clip, float vol) {
		AudioSELoader loader = GameObjectUtility.AddChild<AudioSELoader>(name, gameObject);
		loader.gameObject.SetActive(false);
		loader.clip = clip;
		loader.volume = vol;
		return loader;
	}

	public AudioBGMLoader CreateBGMLoader(string name) {
		AudioClip clip = Resources.Load<AudioClip>("Audio/BGM/"+ name);
		AudioBGMLoader loader = GameObjectUtility.AddChild<AudioBGMLoader>(name, gameObject);
		loader.clip = clip;
		return loader;
	}

	public AudioBGMLoader CreateBGMLoader(string name, AudioClip clip) {
		AudioBGMLoader loader = GameObjectUtility.AddChild<AudioBGMLoader>(name, gameObject);
		loader.clip = clip;
		return loader;
	}

}

[Serializable]
public class SoundVolume{
	public float BGM = 1.0f;
	public float ENV = 1.0f;
	public float SE = 1.0f;
    public float VOICE = 1.0f;
	public bool Mute = false;

	public void Init(){
		BGM = 1.0f;
		ENV = 1.0f;
		SE = 1.0f;
		Mute = false;
	}
}

