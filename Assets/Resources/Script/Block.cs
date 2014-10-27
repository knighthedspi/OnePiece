using UnityEngine;
using System;
using System.Collections;

/// <summary>
// One Block in 3 types game 
// Block,Tile same word in this package 
/// </summary>
public class Block : MonoBehaviour {
	public int _type;
	public Vector2 _posInBoard;
	public bool _dieAfterAnim = false;
	public Color _particleColor;

	////////private variables///////////
	Vector2 _moveTo = new Vector2(0,0);
	Vector2 _scale;
	bool _isAnimation = false;
	float _speed = 0.3f;
	string spriteName = "";
	UISprite _sprite;

	// Use this for initialization
	void Start () {
		_sprite = GetComponent<UISprite>();
		spriteName = _sprite.spriteName;
	}
	
	//check is now transform-tweening
	public bool isAnimation(){
		return _isAnimation;
	}

	//Block mouse clicked or touched 
	public void touchDown(){
		GetComponent<UISprite>().spriteName = spriteName+"_down";
		_scale = this.transform.localScale;
		this.transform.localScale = new Vector2(
			_scale.x + _scale.x/30,
			_scale.y + _scale.y/30
		);
	}

	//Block mouse click release or touch end
	public void touchUp(){
		GetComponent<UISprite>().spriteName = spriteName;
		if(_scale.x == 0) _scale = this.transform.localScale;
		this.transform.localScale = new Vector2(_scale.x,_scale.y);
	}

	// Update is called once per frame
	// transform-tweening. if _dieAfterAnim is True, Destroy object after Tweening end.
	void Update () {
		if (_isAnimation == false){
			if(_dieAfterAnim){
				Destroy(this.gameObject);
			}
			return;
		}

		float x = (this.transform.localPosition.x - _moveTo.x) * _speed;
		float y = (this.transform.localPosition.y - _moveTo.y) * _speed; 

		this.transform.localPosition = new Vector3 (this.transform.localPosition.x - x,
		                                            this.transform.localPosition.y - y);

		if ( Math.Abs(x) < 0.1 && Math.Abs(y) < 0.1)
			_isAnimation = false;
	}

	//start transform-tweening. go to argument x-position from now position (only move x axis)
	public void moveToX(float x){
		this.moveTo(new Vector2(x,this.transform.localPosition.y));
	}
	
	//start transform-tweening. go to argument y-position from now position (only move y axis) 
	public void moveToY(float y){
		this.moveTo(new Vector2(this.transform.localPosition.x,y));
	}

	//start transform-tweening. go to argument position from now position 
	public void moveTo(Vector2 vec){
		_moveTo = vec;
		_isAnimation = true;
	}

}
