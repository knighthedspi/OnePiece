using UnityEngine;
using System.Collections;

public class DestroyBlockEffect : EffectBase {

	private UISpriteAnimation _spriteAnimation;
	private EffectTextData _effectData;

	void Awake()
	{
		_spriteAnimation = GetComponent<UISpriteAnimation> ();
		_data = _data as EffectTextData;
	}


	protected override void StartEffect ()
	{
		if(!(_data is EffectTextData))
		{
			Debug.LogError(" Text effect only accept text data");
			return;
		}
		_effectData = _data as EffectTextData;
		transform.localPosition = _effectData.targetPosition;
		transform.localScale = new Vector3 (1, 1, 1);
		_spriteAnimation.Play ();
	}


	protected override void OnComplete ()
	{
		base.OnComplete ();
		if (!_effectData.isDestroyOnFinish)
			return;
		if(gameObject.CountPooled() > 0)
		{
			gameObject.Recycle();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	protected void OnAnimationFinish()
	{
		OnComplete ();
	}

	public static void Create(Vector3 target)
	{
		EffectTextData data = new EffectTextData ();
		data.numPool 		= 10;
		data.targetPosition = target;
		data.type = EffectType.DestroyEffect;
		EffectManager.Instance.PlayEffect<DestroyBlockEffect> (data);
	}
}
