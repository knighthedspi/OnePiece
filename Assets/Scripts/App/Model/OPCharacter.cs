
using UnityEngine;
using System.Collections;

public class OPCharacter {
	private int id;
	private string characterName;
	private int hp;
	private int sp;
	private Vector3 direction;
	private Vector3 position;
	int soundId;

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

	public int Hp {
		get {
			return this.hp;
		}
		set {
			hp = value;
		}
	}

	public int Sp {
		get {
			return this.sp;
		}
		set {
			sp = value;
		}
	}

	public Vector3 Direction {
		get {
			return this.direction;
		}
		set {
			direction = value;
		}
	}

	public Vector3 Position {
		get {
			return this.position;
		}
		set {
			position = value;
		}
	}

	public int SoundId {
		get {
			return this.soundId;
		}
		set {
			soundId = value;
		}
	}

}