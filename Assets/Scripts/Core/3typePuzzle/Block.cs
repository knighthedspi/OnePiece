using UnityEngine;
using System.Collections;
using System;

public enum BlockType
{
	luffy = 1,
	nami,
	robin,
	sanji,
	zoro,
	chopper,

	
}

public class Block : MonoBehaviour {
	
	public float speed;
	
	public BlockType blockType { set; get;}
	public Vector2 posInBoard { set; get;}
	public Color particleColor { set; get;}
	
	public bool isAnimation { get{return _isAnimation;}}
	
	protected UISprite 	_uiSprite;
	protected OPGameSetup _gameSetup;

	public UISprite uiSprite 
	{ 
		get{
			return _uiSprite;
		}
	}
	
	public string spriteName
	{
		get 
		{ 
			return _uiSprite.spriteName;
		}
		set 
		{ 
			_spriteName = value;
			_uiSprite.spriteName = value;
		}
	}
	
	private bool _isAnimation;
	private string 	_spriteName;
	private Vector2 _moveTo;
	private Vector2 _scale;
	
	
	void Awake()
	{
		_uiSprite 	= GetComponent<UISprite> ();
		_gameSetup 	= AppManager.Instance.gameSetup;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!_isAnimation) 
			return;
		float x = (this.transform.localPosition.x - _moveTo.x) * speed;
		float y = (this.transform.localPosition.y - _moveTo.y) * speed; 
		
		this.transform.localPosition = new Vector3 (this.transform.localPosition.x - x,
		                                            this.transform.localPosition.y - y);
		
		if ( Math.Abs(x) < 0.1f && Math.Abs(y) < 0.1f)
			_isAnimation = false;
	}
	
	public virtual void Init(BlockType type)
	{
		// Init Block here
		spriteName = "0" + type.ToString ();
		blockType = type;
		_uiSprite.width 	= (int)_gameSetup.blockSize.x;
		_uiSprite.height 	= (int)_gameSetup.blockSize.y;
		transform.localScale = new Vector3 (1, 1, 1);

	}

	public void InitRand ()
	{
		if (_gameSetup.listBlockTypes.Length <= 0)
		{
			Debug.LogWarning("Has no block type in game setup");
			return;
		}
			
		int rand = UnityEngine.Random.Range (0, _gameSetup.listBlockTypes.Length);
		Init (_gameSetup.listBlockTypes [rand]);

	}
	
	public virtual void TouchDown()
	{
		_uiSprite.spriteName = spriteName+"_down";
		_scale = this.transform.localScale;
		this.transform.localScale = new Vector2(
			_scale.x + _scale.x/30,
			_scale.y + _scale.y/30
			);
	}
	
	public virtual void TouchUp()
	{
		_uiSprite.spriteName = spriteName;
		if(_scale.x == 0) _scale = this.transform.localScale;
		this.transform.localScale = new Vector2(_scale.x,_scale.y);
	}
	
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
	
	public virtual void Destroy()
	{
		gameObject.Recycle ();
	}
	
}
