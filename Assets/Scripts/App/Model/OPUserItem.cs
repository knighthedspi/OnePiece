using UnityEngine;
using System.Collections;

public class OPUserItem {
	private int id;
	private int user_id;
	private int item_id;
	private int num;

	public int Id {
		get {
			return this.id;
		}
		set {
			id = value;
		}
	}

	public int User_id {
		get {
			return this.user_id;
		}
		set {
			user_id = value;
		}
	}

	public int Item_id {
		get {
			return this.item_id;
		}
		set {
			item_id = value;
		}
	}

	public int Num {
		get {
			return this.num;
		}
		set {
			num = value;
		}
	}

}