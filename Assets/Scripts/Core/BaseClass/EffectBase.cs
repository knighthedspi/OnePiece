using UnityEngine;
using System.Collections;

public class EffectBase : MonoBehaviour {

	protected EffectData _data;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
