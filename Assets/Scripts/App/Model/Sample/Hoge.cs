using UnityEngine;
using System.Collections;

public class Hoge {

	[CustomField(Statement = "PRIMARY KEY AUTOINCREMENT")]
	private int? hogeId;
	private string hogeName;

	public int? HogeId {
		get {
			return this.hogeId;
		}
		set {
			hogeId = value;
		}
	}

	public string HogeName {
		get {
			return this.hogeName;
		}
		set {
			hogeName = value;
		}
	}

}
