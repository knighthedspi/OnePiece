using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : Singleton<EffectManager> {

	private GameObject _rootWindow;
//	private List<EffectBase> _effectQueues;
	private Dictionary<string,EffectBase> _effectDic;

	// Use this for initialization
	void Start () {
		_effectDic = new Dictionary<string,EffectBase> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/// <summary>
	/// play effect and add effect to pool
	/// </summary>
	/// <param name="effectData">Effect data.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public void PlayEffect <T> (EffectData effectData) where T : EffectBase
	{
		if (_rootWindow == null)
						_rootWindow = ViewManager.Instance.globalViewObject;
		string effectType = effectData.type.ToString ();
		T effect;
		T spawnEffect;
		if(_effectDic.ContainsKey(effectType))
		{
			effect 		=	 _effectDic[effectType] as T;

		}
		else
		{
			GameObject go 	= Resources.Load(Config.EFFECT_PATH + effectType,typeof(GameObject)) as GameObject;
			if(go == null) 
			{
				Debug.LogError("not found object effect " + effectType);
			}
			effect 			= go.GetComponent<T>();
			_effectDic.Add(effectType,effect);
		}

		// check if effect is pooled
		int numPooled 	=  effect.CountPooled<T>();
		if(numPooled > 0)
		{
			spawnEffect 	=	effect.Spawn<T>();
			spawnEffect.transform.parent = _rootWindow.transform;
		}
		else
		{
			// if it was not pooled, create new and push to pool
			GameObject go 	= NGUITools.AddChild(_rootWindow,effect.gameObject);
			spawnEffect 	= go.GetComponent<T>();
			if(effectData.numPool > 0)
			{
				effect.CreatePool<T>(effectData.numPool);
			}
		}

		spawnEffect.Init(effectData);
	} 



}
