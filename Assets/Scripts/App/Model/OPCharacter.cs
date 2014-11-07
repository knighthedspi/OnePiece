
using UnityEngine;
using System.Collections;


public class OPCharacter{
	private int id;
	private string characterName;
	private int kindId;
	private int evolution;
	private int initialHP;
	private int maxHP;
	private int attackPoint;

	public int Id {
		get {
			return this.id;
		}
		set {
			id = value;
		}
	}

	public string CharacterName {
		get {
			return this.characterName;
		}
		set {
			characterName = value;
		}
	}

	public int KindId {
		get {
			return this.kindId;
		}
		set {
			kindId = value;
		}
	}

	public int Evolution {
		get {
			return this.evolution;
		}
		set {
			evolution = value;
		}
	}

	public int InitialHP {
		get {
			return this.initialHP;
		}
		set {
			initialHP = value;
		}
	}

	public int MaxHP {
		get {
			return this.maxHP;
		}
		set {
			maxHP = value;
		}
	}

	public int AttackPoint {
		get {
			return this.attackPoint;
		}
		set {
			attackPoint = value;
		}
	}

}