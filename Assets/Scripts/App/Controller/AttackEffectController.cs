using UnityEngine;
using System;
using System.Collections;

public class AttackEffectController : MonoBehaviour {
	public delegate void OnFinished(Vector2 lastPos);
	public OnFinished Finish;


	private Vector2 _end;
	public	float	speed;
	private bool   _isPlay;

	// Use this for initialization
	void Start () {
		speed = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(_isPlay)
		{
			playAnimation();
		}
	}

	private void playAnimation()
	{
		float x = (this.transform.position.x - _end.x) * speed;
		float y = (this.transform.position.y - _end.y) * speed; 
		
		this.transform.position = new Vector3 (this.transform.position.x - x, this.transform.position.y - y);
		
		if ( Math.Abs(x) < 0.1f && Math.Abs(y) < 0.1f)
		{
			_isPlay = false;
			Finish(_end);
			CoroutineUtility.Instance.WaitAndExecute(1, Destroy);
		}
	}

	private void Destroy()
	{
		this.gameObject.Recycle();
	}

	// generate Particle's Effect
	public void generate(Vector2 start , Vector2 end){
		this.transform.position = start;
		_end = end;
		_isPlay = true;
	}
}
