using UnityEngine;
using System.Collections;

public class UserUnitParty {

	private UserParty userParty;
	private UserUnit userUnit;

	public UserParty UserParty {
		get {
			return this.userParty;
		}
		set {
			userParty = value;
		}
	}
	
	public UserUnit UserUnit {
		get {
			return this.userUnit;
		}
		set {
			userUnit = value;
		}
	}

}
