using UnityEngine;
using System.Collections;

/// <summary> 
//  Monster in all types game
/// </summary>
public class Monster : MonoBehaviour {
	public delegate void OnFinished(string type);

	SkeletonAnimation _animation;

	////////////////////////////////////////
	//you can customizing game variable in inspector window
	////////////////////////////////////////
	public int _turn = 3;
	public int _maxturn = 3;
	
	public float _hp = 100;
	public float _maxhp = 100;

	public float _attackPoint = 10;

	public OnFinished Finish;

	private bool overridePlaying = true;

	// Use this for initialization
	void Start () {
		initialize();
	}
	
	// initialize monster values
	void initialize(){
		_animation = gameObject.GetComponent<SkeletonAnimation>();
		_animation.state.Event += Event;
		_animation.state.End += End;
	}

	//Animation Finished
	public void End(Spine.AnimationState state, int trackIndex) {
		overridePlaying = true;
		Finish(state.GetCurrent(trackIndex).ToString());
	}
	
	//Animation Event
	public void Event (Spine.AnimationState state, int trackIndex, Spine.Event e) {
	}

	//start animation entry
	public void entryPlay(){
		if(_animation == null){
			initialize();
		}
		if(overridePlaying == false)return ;

		_animation.state.SetAnimation(0, "entry", false);
		_animation.state.AddAnimation(0, "idle", true, 0);
	}

	//start animation attack
	public void attackPlay(){
		if(_animation == null){
			initialize();
		}
		_animation.state.SetAnimation(0, "attack", false);
		_animation.state.AddAnimation(0, "idle", true, 0);

		overridePlaying = false;
	}

	//start animation attacked
	public void attackedPlay(){
		if(_animation == null){
			initialize();
		}
		if(overridePlaying == false)return ;
		
		_animation.state.SetAnimation(0, "Attacked", false);
		_animation.state.AddAnimation(0, "idle", true, 0);
	}

	//start animation die
	public void diePlay(){
		if(_animation == null){
			initialize();
		}
		_animation.state.SetAnimation(0, "die", false);
		overridePlaying = false;
	}
}
