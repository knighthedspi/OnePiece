using UnityEngine;
using System.Collections;

public class Arm {

	private int? armId;
	private string armName;

	public int? ArmId {
		get {
			return this.armId;
		}
		set {
			armId = value;
		}
	}

	public string ArmName {
		get {
			return this.armName;
		}
		set {
			armName = value;
		}
	}
}
