using UnityEngine;
using System.Collections;

public class Attribution {

	private int? attributionId;
	private string attributionName;

	public int? AttributionId {
		get {
			return this.attributionId;
		}
		set {
			attributionId = value;
		}
	}

	public string AttributionName {
		get {
			return this.attributionName;
		}
		set {
			attributionName = value;
		}
	}

}
