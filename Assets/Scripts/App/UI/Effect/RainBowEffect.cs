using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RainBowEffect : EffectBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void Create(Block origin, List<Block> targets)
	{
		EffectBlockData data 	= new EffectBlockData ();
		data.type 				= EffectType.RainbowEffect;
		data.listTargets 		= targets;
		data.orgin = origin;
		EffectManager.Instance.PlayEffect<RainBowEffect> (data);
	}
}
