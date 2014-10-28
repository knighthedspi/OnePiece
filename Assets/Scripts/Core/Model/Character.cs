using UnityEngine;
using System.Collections;

[System.Serializable]
public class Character{

	[SerializeField]
	private string characterId;
	[SerializeField]
	private string characterName;
	[SerializeField]
	private float interval;
	[SerializeField]
	private float toughness;
	[SerializeField]
	private int numberOfAttacks;
	[SerializeField]
	private float speed;
	[SerializeField]
	private float targetAreaOfAttack;
	[SerializeField]
	private int hp;
	private int initialHp;
	[SerializeField]
	private int offense;
	[SerializeField]
	private int sp;
	private int initialSp;
	[SerializeField]
	private Vector3 direction;
	[SerializeField]
	private Vector3 position;
	[SerializeField]
	private float sortingOrder;
	[SerializeField]
	private float hitAnimationTimeLag;
	[SerializeField]
	private bool wait;
	[SerializeField]
	private float timeScale;
	[SerializeField]
	private int soundId;


	public Character (string characterId, 
	                       string characterName, 
	                       float interval, 
	                       float toughness,
	                       int numberOfAttacks,
	                       float speed, 
	                       float targetAreaOfAttack, 
	                       int hp, 
	                       int offense, 
	                       int sp, 
	                       Vector3 direction, 
	                       Vector3 position,
	                       float sortingOrder,
	                       float attackAnimationTimeLag,
	                       bool guard,
	                       float timeScale,
	                       int soundId){
		this.characterId = characterId;
		this.characterName = characterName;
		this.interval = interval;
		this.toughness = toughness;
		this.numberOfAttacks = numberOfAttacks;
		this.speed = speed;
		this.targetAreaOfAttack = targetAreaOfAttack;
		this.hp = hp;
		this.initialHp = hp;
		this.offense = offense;
		this.sp = sp;
		this.initialSp = sp;
		this.direction = direction;
		this.position = position;
		this.sortingOrder = sortingOrder;
		this.hitAnimationTimeLag = attackAnimationTimeLag;
		this.wait = guard;
		this.timeScale = timeScale;
		this.soundId = soundId;
	}

	public string CharacterId {
		get {return characterId;}
		set {characterId = value;}
	}
	
	public string CharacterName {
		get {return characterName;}
		set {characterName = value;}
	}
	
	public float Interval {
		get {return interval / timeScale;}
		set {interval = value;}
	}
	
	public float Toughness {
		get {return toughness;}
		set {toughness = value;}
	}

	public int NumberOfAttacks {
		get {return numberOfAttacks;}
		set {numberOfAttacks = value;}
    }
	
	public float Speed {
		get {return speed * timeScale;}
		set {speed = value;}
	}
	
	public float TargetAreaOfAttack {
		get {return targetAreaOfAttack;}
		set {targetAreaOfAttack = value;}
	}
	
	public int Hp {
		get {return hp;}
		set {hp = value;}
	}
	public int InitialHp {
		get {return initialHp;}
	} 

	public int Offense {
		get {return offense;}
		set {offense = value;}
	}
	
	public int Sp {
		get {return sp;}
		set {sp = value;}
	}

	public int InitialSp {
		get {return initialSp;}
	}
	
	public Vector3 Direction {
		get {return direction;}
		set {direction = value;}
	}
	
	public Vector3 Position {
		get {return position;}
		set {position = value;}
	}
	
	public float SortingOrder {
		get {return sortingOrder;}
		set {sortingOrder = value;}
	}

	public float HitAnimationTimeLag {
		get {return hitAnimationTimeLag;}
		set {hitAnimationTimeLag = value;}
	}

	public bool Wait {
		get {return wait;}
		set {wait = value;}
	}

	public  float TimeScale {
		get {return timeScale;}
		set {timeScale = value;}
	}

	public int SoundId{
		get {return soundId;}
		set {soundId = value;}
	}
}
