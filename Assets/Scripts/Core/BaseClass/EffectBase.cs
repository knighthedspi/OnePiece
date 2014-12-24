using UnityEngine;
using System.Collections;

public class EffectBase : MonoBehaviour {

	protected EffectData _data;

	public virtual void Init(EffectData data)
	{
		_data = data;
		StartEffect ();
	}

	protected virtual void StartEffect()
	{

	}

	protected virtual void OnComplete(){
		_data.Complete(this);
	}

	protected virtual void OnStart()
	{
		_data.Start();
	}
}
