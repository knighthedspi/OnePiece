using UnityEngine;
using System.Collections;

public class UIUnitScrollView : UIScrollView {
	
	public bool ShouldMove{
		get{
			return shouldMove;
		}
	}

	public Transform Trans{
		get{
			return mTrans;
		}
	}

}
