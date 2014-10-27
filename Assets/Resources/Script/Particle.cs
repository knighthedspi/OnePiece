using UnityEngine;
using System;
using System.Collections;

/// <summary> 
// Particle Tweening Class
/// </summary> 
public class Particle : MonoBehaviour {
	private Vector2 _scale;
	private Vector2 _move;
	private Color _color;

	public Vector2 _scaleTo;
	public Vector2 _moveTo;
	public Color _colorTo;

	private UISprite _sprite;
	private bool _play = false;

	public bool _die = false;

	//update tweening informations and set values!
	void Update () {
		if(_play == false || _die)return ;
	
		float x = (transform.localPosition.x - _moveTo.x) * 0.1f;
		float y = (transform.localPosition.y - _moveTo.y) * 0.1f;

		float r = (_sprite.color.r - _colorTo.r) * 0.1f;
		float g = (_sprite.color.g - _colorTo.g) * 0.1f;
		float b = (_sprite.color.b - _colorTo.b) * 0.1f;
		float a = (_sprite.color.a - _colorTo.a) * 0.1f;

		transform.localPosition = new Vector3 (
			this.transform.localPosition.x - x,
		    this.transform.localPosition.y - y,
		    0
		);

		float sx=0;
		float sy=0;
#if(_NGUI_PRO_VERSION_)
		sx = ((float)_sprite.width - _scaleTo.x)*0.1f;
		sy = ((float)_sprite.height - _scaleTo.y)*0.1f;

		_sprite.width  = (int)((float)_sprite.width  - sx);
		_sprite.height = (int)((float)_sprite.height - sy);
#else
		sx = (transform.localScale.x - _scaleTo.x)*0.1f;
		sy = (transform.localScale.y - _scaleTo.y)*0.1f;

		transform.localScale = new Vector3(
			this.transform.localScale.x - sx,
			this.transform.localScale.y - sy
		);
#endif		
		_sprite.color = new Color(
			_sprite.color.r-r,
			_sprite.color.g-g,
			_sprite.color.b-b,
			_sprite.color.a-a
		);

		if( Math.Abs(x) < 0.1f && Math.Abs(y) < 0.1f && 
			Math.Abs(r) < 0.1f && Math.Abs(g) < 0.1f && Math.Abs(b) < 0.1f && Math.Abs(a) < 0.1f ){
			_die = true;
		}
	}

	//set now color.
	public void setColor(Color c){
		_sprite = gameObject.GetComponent<UISprite>();
		_sprite.color = c;
	}

	//play particle tweening
	public void play(){
		_sprite = gameObject.GetComponent<UISprite>();

		_move = transform.localPosition;
		_scale = transform.localScale;
		_color = _sprite.color;

		_play = true;
	}
}
