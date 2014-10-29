
using UnityEngine;
using System.Collections;

public class OPItem {
	
	private int id;
	private int level_id;
	private string character_id;

	public int Id {
		get {
			return this.id;
		}
		set {
			id = value;
		}
	}

	public int Level_id {
		get {
			return this.level_id;
		}
		set {
			level_id = value;
		}
	}

	public string Character_id {
		get {
			return this.character_id;
		}
		set {
			character_id = value;
		}
	}
}