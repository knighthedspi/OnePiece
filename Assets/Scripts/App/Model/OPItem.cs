
using UnityEngine;
using System.Collections;

public class OPItem {
	
	private int id;
	private string name;
	private int category;
    private int unlockLevel;
	private string effectName;
	private int consumItem;
	private int cost;

	public int Id {
		get {
			return this.id;
		}
		set {
			id = value;
		}
	}

	public string Name {
		get {
			return this.name;
		}
		set {
			name = value;
		}
	}

	public int Category {
		get {
			return this.category;
		}
		set {
			category = value;
		}
	}

    public int UnlockLevel {
        get {
            return this.unlockLevel;
        }
        set {
            unlockLevel = value;
        }
    }

	public string EffectName {
		get {
			return this.effectName;
		}
		set {
			effectName = value;
		}
	}

	public int ConsumItem {
		get {
			return this.consumItem;
		}
		set {
			consumItem = value;
		}
	}

	public int Cost {
		get {
			return this.cost;
		}
		set {
			cost = value;
		}
	}

}