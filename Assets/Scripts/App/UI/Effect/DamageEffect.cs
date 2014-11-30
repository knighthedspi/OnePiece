using UnityEngine;
using System.Collections;

public class DamageEffect : EffectBase {

	private UILabel _label;
	private EffectTextData _textData;
	void Awake()
	{
		_label = GetComponent<UILabel> ();
		_data = _data as EffectTextData;
	}
	protected override void OnComplete ()
	{
		base.OnComplete ();
		if (!_textData.isDestroyOnFinish)
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

	protected override void StartEffect ()
	{
		if(!(_data is EffectTextData))
		{
			Debug.LogError(" Text effect only accept text data");
			return;
		}
		_textData = _data as EffectTextData;
		base.StartEffect ();
		transform.localPosition = (_textData.originPosition == null) ? new Vector3 (0, 0, 0) : _textData.originPosition;
		transform.localScale = new Vector3 (3, 3, 1);
		StartCoroutine("animStart");

	}

	//aniamtion corouine!
	IEnumerator animStart(){
		Vector3 v = gameObject.transform.localPosition;
		
		TweenAlpha.Begin( gameObject, 0.5f,  1f);
		Vector3 to = (_textData.targetPosition == null) ? new Vector3(0,0,0) : v + new Vector3(0,40,0) ;
		TweenPosition.Begin( gameObject, 0.5f, to)	;
		yield return new WaitForSeconds(0.8f);
		
		TweenAlpha.Begin( gameObject, 0.2f,  0f);
		yield return new WaitForSeconds(0.3f);
		
		OnComplete ();
	}

	public static void Create(string mainText, Vector3 pos, Vector3 target)
	{
		EffectTextData data = new EffectTextData ();
		data.originPosition = pos;
		data.targetPosition = target;
		data.mainText 		= mainText;
		data.numPool 		= 10;
		data.type 			= EffectType.DamageEffect;
		EffectManager.Instance.PlayEffect<DamageEffect> (data);
	}


}
