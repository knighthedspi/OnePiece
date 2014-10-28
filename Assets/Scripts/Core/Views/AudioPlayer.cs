using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioPlayer{

	public Dictionary<string, AudioSELoader>               SELoaders        = new Dictionary<string, AudioSELoader>();
	public Dictionary<string, AudioBGMLoader>              BGMLoaders       = new Dictionary<string, AudioBGMLoader>();

	public readonly static string                          AUDIO_NAME_MAIN  = "main";
	public readonly static string                          AUDIO_NAME_WIN   = "win";
	public readonly static string                          AUDIO_NAME_LOSE  = "lose";
	public readonly static string                          AUDIO_NAME_CLASH = "clash";

	public void PlaySE(string soundName, float vol) {
		if(SoundManager.Instance.ContainsSE(soundName)){
			SoundManager.Instance.PlaySE(soundName);
			return;
		}
		AudioSELoader loader;
		SELoaders.TryGetValue(soundName, out loader);
		if (loader == null) {
			loader = SoundManager.Instance.CreateSELoader(soundName, vol);
			SELoaders[soundName] = loader;
		}
		loader.Play();
	}

	public void PlaySE(string soundName) {
		PlaySE(soundName, 1f);
	}
	
	public void PlayBGM(string soundName) {
		if(SoundManager.Instance.ContainsBGM(soundName)){
			SoundManager.Instance.PlayBGM(soundName);
			return;
		}
		AudioBGMLoader loader;
		BGMLoaders.TryGetValue(soundName, out loader);
		if(loader == null){
			loader = SoundManager.Instance.CreateBGMLoader(soundName);
			BGMLoaders[soundName] = loader;
		}
		loader.Play();
	}
}
