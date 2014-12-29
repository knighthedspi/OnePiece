using UnityEngine;
using System.Collections;

public enum EffectType
{
	Explosion = 1,
	DamageEffect,
	DestroyEffect,
	RainbowEffect,

}
public class EffectData {

	public EffectType type;
	public int numPool = 0;

	public delegate void OnComplete(EffectBase sender);
	public delegate void OnStart();

	public OnComplete onComplete;
	public OnStart onStart;


	public void Complete(EffectBase sender)
	{
		if (onComplete == null)
						return;
		onComplete (sender);
	}


	public void Start()
	{
		if (onStart == null)
						return;
		onStart ();

	}



}
