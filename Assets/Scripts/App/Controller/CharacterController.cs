using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

	public delegate void OnFinished(string type);

	public UISprite	UIMonsterHp;

	[HideInInspector]
	public OPCharacter monsterModel {get; set;}

	public GameObject entireMonsterObj 
	{
		get
		{
			return gameObject.transform.parent.gameObject;
		}
	}

	private Animator _animator;
	public OnFinished Finish;

	protected bool overridePlaying = true;
	public float initialHP {get; set;}
	public float currentHP {get; set;}
	// Use this for initialization
	void Start () {
		initialize();
	}
	
	// initialize monster values
	protected virtual void initialize(){
		_animator = gameObject.GetComponent<Animator>();
		initMonsterAttributes();
	}

	protected virtual void initMonsterAttributes(){
		this.initialHP = Random.Range(monsterModel.InitialHP, monsterModel.MaxHP );
		this.currentHP = this.initialHP;
		this.UIMonsterHp.fillAmount = 1;
	}

	/// <summary>
	/// Play entry animation
	/// </summary>
	public void entryPlay(){
		if(_animator == null){
			initialize();
		}
		if(overridePlaying == false)return ;
		
		_animator.Play(Config.ENTRY_ANIM);
	}

	/// <summary>
	/// Play attacked animation
	/// </summary>
	public void attackedPlay(){
		if(_animator == null){
			initialize();
		}
		if(overridePlaying == false)return ;

		_animator.Play(Config.ATTACKED_ANIM);
	}
	
	/// <summary>
	/// Play die animation
	/// </summary>
	public void diePlay(){
		if(_animator == null){
			initialize();
		}
		_animator.Play(Config.DIE_ANIM);
		overridePlaying = false;
	}

	/// <summary>
	/// Decrease the HP amount of Monster
	/// </summary>
	/// <param name="amount">HP amount of monster will be decreased</param>
	public void decreaseHPAmount(float amount)
	{
		this.currentHP -= amount;
		this.UIMonsterHp.fillAmount = this.currentHP / this.initialHP;
		OPDebug.Log("current HP is " + currentHP);
	}

	/// <summary>
	/// Gets the current state of the current animation.
	/// </summary>
	/// <returns>The current animation state.</returns>
	public string getCurrentAnimationState(){
		AnimatorStateInfo _currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		if(_currentStateInfo.IsName(Config.ENTRY_ANIM))
			return Config.ENTRY_ANIM;
		else if(_currentStateInfo.IsName(Config.ATTACKED_ANIM))
			return Config.ATTACKED_ANIM;
		else if(_currentStateInfo.IsName(Config.IDLE_ANIM))
			return Config.IDLE_ANIM;
		else if(_currentStateInfo.IsName(Config.DIE_ANIM))
			return Config.DIE_ANIM;
		else
			return null;
	}
	
	/// <summary>
	/// Raises the finish event after die animation, should be set from animation file
	/// </summary>
	public void OnFinish(){
		overridePlaying = true;
		if(Finish != null)
			Finish(getCurrentAnimationState());
	}
}
