
using UnityEngine;
using System.Collections;

public class OPUser {
	
	private int? id;
	private string user_name;
	private string facebook_token;
	private int exp;
	private int level_id;
	private string lasted_health_res;
	private int health;
	private int score;
	private int high_score;
	private string updated_at;
	private string created_at;

	public int? Id {
		get {
			return this.id;
		}
		set {
			id = value;
		}
	}

	public string User_name {
		get {
			return this.user_name;
		}
		set {
			user_name = value;
		}
	}
	
	public string Facebook_token {
		get {
			return this.facebook_token;
		}
		set {
			facebook_token = value;
		}
	}

	public int Exp {
		get {
			return this.exp;
		}
		set {
			exp = value;
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

	public string Lasted_health_res {
		get {
			return this.lasted_health_res;
		}
		set {
			lasted_health_res = value;
		}
	}

	public int Health {
		get {
			return this.health;
		}
		set {
			health = value;
		}
	}

	public int Score {
		get {
			return this.score;
		}
		set {
			score = value;
		}
	}

	public int High_score {
		get {
			return this.high_score;
		}
		set {
			high_score = value;
		}
	}

	public string Updated_at {
		get {
			return this.updated_at;
		}
		set {
			updated_at = value;
		}
	}

	public string Created_at {
		get {
			return this.created_at;
		}
		set {
			created_at = value;
		}
	}
}
