using UnityEngine;
using System.Collections;

public class UserParty {

	private int?   userPartyId;
	private string userId;
	private int?   partyId;
	private int?   userUnitId ;
	private int?   sortOrder;

	public int? UserPartyId {
		get {
			return this.userPartyId;
		}
		set {
			userPartyId = value;
		}
	}

	public string UserId {
		get {
			return this.userId;
		}
		set {
			userId = value;
		}
	}

	public int? PartyId {
		get {
			return this.partyId;
		}
		set {
			partyId = value;
		}
	}

	public int? UserUnitId {
		get {
			return this.userUnitId;
		}
		set {
			userUnitId = value;
		}
	}

	public int? SortOrder {
		get {
			return this.sortOrder;
		}
		set {
			sortOrder = value;
		}
	}

}
