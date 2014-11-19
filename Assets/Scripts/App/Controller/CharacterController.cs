using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

	public delegate void OnFinished(string type);
	
	private Animator _animator;


	////////////////////////////////////////
	//character attributes, set from model
	////////////////////////////////////////

	[HideInInspector]
	public int id {get; private set;}

	public float hp {get; private set;}
	public float maxhp {get; private set;}
	
	public float attackPoint {get; private set;}

	// finish event after each animation
	public OnFinished Finish;
	
	private bool overridePlaying = true;

	// Use this for initialization
	void Start () {
		initialize();
	}
	
	// initialize monster values
	void initialize(){
		_animator = gameObject.GetComponent<Animator>();
	}

	/// <summary>
	/// Sets the properties for monster: max_hp, hp, attackPoint.
	/// Should call b4 playing any animation
	/// </summary>
	/// <param name="obj">Object.</param>
	public void setProperties(OPCharacter obj){
		this.hp = Random.Range(obj.InitialHP, obj.MaxHP);
		this.maxhp = this.hp;
		this.attackPoint = obj.AttackPoint;
		this.id = obj.Id;
	}

	//start animation entry
	public void entryPlay(){
		if(_animator == null){
			initialize();
		}
		if(overridePlaying == false)return ;
		
		_animator.Play(Config.ENTRY_ANIM);
	}

	//start animation attacked
	public void attackedPlay(){
		if(_animator == null){
			initialize();
		}
		if(overridePlaying == false)return ;

		_animator.Play(Config.ATTACKED_ANIM);
	}
	
	//start animation die
	public void diePlay(){
		if(_animator == null){
			initialize();
		}
		_animator.Play(Config.DIE_ANIM);
		overridePlaying = false;
	}
	
	/// <summary>
	/// Gets the current state of the current animation.
	/// </summary>
	/// <returns>The current animation state.</returns>
	public string getCurrentAnimationState(){
		AnimatorStateInfo _currentStateInfo = _animator.GetCurrentAnimatorStateInfo(Config.LAYER_MONSTER);
		string entry_anim = "Monster." + Config.ENTRY_ANIM;
		string attacked_anim = "Monster." + Config.ATTACKED_ANIM;
		string idle_anim = "Monster." + Config.IDLE_ANIM;
		string die_anim = "Monster." + Config.DIE_ANIM;
		if(_currentStateInfo.IsName(entry_anim))
			return Config.ENTRY_ANIM;
		else if(_currentStateInfo.IsName(attacked_anim))
			return Config.ATTACKED_ANIM;
		else if(_currentStateInfo.IsName(idle_anim))
			return Config.IDLE_ANIM;
		else if(_currentStateInfo.IsName(die_anim))
			return Config.DIE_ANIM;
		else
			return null;
	}

	/// <summary>
	/// Raises the finish event after each animation, should be set from animation file
	/// </summary>
	/// <param name="type">Type.</param>
	public void OnFinish(){
		overridePlaying = true;
		if(Finish != null)
			Finish(getCurrentAnimationState());
	}
}
