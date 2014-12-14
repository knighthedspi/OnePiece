
using UnityEngine;
using System.Collections;

public class OPLevel {
	
	private int id;
	private string level;
	private int exp;
    private int diff;

	public int Id {
		get {
			return this.id;
		}
		set {
			id = value;
		}
	}

	public string Level {
		get {
			return this.level;
		}
		set {
			level = value;
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

    public int Diff {
        get {
            return this.diff;
        }
        set {
            diff = value;
        }
    }
}