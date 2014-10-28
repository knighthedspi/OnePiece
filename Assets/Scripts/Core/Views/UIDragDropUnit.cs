using UnityEngine;
using System.Collections;

public class UIDragDropUnit : UIDragDropItem  {

	[HideInInspector]
	public GameObject PrevParent;

	protected override void OnDragDropStart (){
		base.OnDragDropStart();
	}

	protected override void StartDragging ()
	{
		if (!mDragging)
		{
			if (cloneOnDrag)
			{
				GameObject clone = NGUITools.AddChild(transform.parent.gameObject, gameObject);
				clone.transform.localPosition = transform.localPosition;
				clone.transform.localRotation = transform.localRotation;
				clone.transform.localScale = transform.localScale;
				
				UIButtonColor bc = clone.GetComponent<UIButtonColor>();
				if (bc != null) bc.defaultColor = GetComponent<UIButtonColor>().defaultColor;
				
				UICamera.currentTouch.dragged = clone;
				
				UIDragDropUnit item = clone.GetComponent<UIDragDropUnit>();
				item.mDragging = true;
				item.Start();
				item.OnDragDropStart();
			}
			else
			{
				mDragging = true;
				OnDragDropStart();
			}
		}
	}

	protected override void OnDragDropMove (Vector2 delta){
		base.OnDragDropMove(delta);
	}

	private int CountUnitOnDeck(){
		int i = 0;
		foreach(Transform parentChild in mTrans.parent.parent.transform) {
			i += parentChild.childCount;
		}
		return i;
	}

	private Transform FindSameUnit(){
		foreach(Transform parentChild in mTrans.parent.parent.transform) {
			var unitObj = parentChild.FindChild(gameObject.name.Replace("(Clone)", ""));
			if(unitObj != null)
				return unitObj;
		}
		return null;
	}
	private void MoveUnit(Transform targetTrans){
		foreach(Transform parentChild in targetTrans) {
			int i = 1;
			int childCount = parentChild.childCount;
			foreach(Transform child in parentChild){
				if(childCount > i){
					if(PrevParent != null){
						var dropUnit = child.GetComponent<UIDragDropUnit>();
						dropUnit.PrevParent = PrevParent;
						child.transform.parent = PrevParent.transform;
						child.transform.localPosition = new Vector3(-50f, 0f,0f);//Vector3.zero;
					}else{
						Destroy(child.gameObject);
					}
				}
				if(!mTrans.parent.gameObject.Equals(child.transform.parent.gameObject) && gameObject.name.Replace("(Clone)", "").Equals(child.gameObject.name)){
					Destroy(child.gameObject);
				}
				i++;
			}
		}
	}
	protected override void OnDragDropRelease (GameObject surface){

		mTouchID = int.MinValue;

		if (mButton != null) mButton.isEnabled = true;
		else if (mCollider != null) mCollider.enabled = true;
		else if (mCollider2D != null) mCollider2D.enabled = true;

		UIDragDropContainer container = surface ? NGUITools.FindInParents<UIDragDropContainer>(surface) : null;

		if (container != null){
			mTrans.parent = (container.reparentTarget != null) ? container.reparentTarget : container.transform;
			Destroy(gameObject.GetComponent<UIDragScrollView>());
			this.restriction = Restriction.None; 
			this.cloneOnDrag = false;

			int count = CountUnitOnDeck();

			if(count <= 5){
				MoveUnit(mTrans.parent.parent.transform);
			}else{
				Transform sameChildTrans = FindSameUnit();
				if(sameChildTrans != null){
					var dropUnit = sameChildTrans.gameObject.GetComponent<UIDragDropUnit>();
					PrevParent = dropUnit.PrevParent;
					Destroy(sameChildTrans.gameObject);
					MoveUnit(mTrans.parent.parent.transform);
				}else{
					int childCount = mTrans.parent.childCount;
					foreach(Transform child in mTrans.parent){
						if(!child.gameObject.Equals(gameObject))
							Destroy(child.gameObject);
					}
					if(childCount < 2)
						Destroy(gameObject);
				}
			}

			gameObject.name = gameObject.name.Replace("(Clone)", "");
			var target = gameObject.GetComponent<UI2DSprite>();
			target.sprite2D = Resources.Load<Sprite>("Sprites/dk_" + gameObject.name);
			target.MakePixelPerfect();
			 


			Vector3 pos = mTrans.localPosition;
			pos.z = 0f;

			mTrans.localPosition = Vector3.zero;
			if (mDragScrollView != null)
				StartCoroutine(EnableDragScrollView());
			
			NGUITools.MarkParentAsChanged(gameObject);
			
			if (mTable != null) mTable.repositionNow = true;
			if (mGrid != null) mGrid.repositionNow = true;


			OnDragDropEnd();
			GameObject info= GameObjectUtility.AddChild(gameObject,"UI/PartyUnitInfo");
			info.transform.localPosition = new Vector3(88f, 0f,0f);
			transform.localPosition = new Vector3(-50f, 0f,0f);
			PrevParent = mTrans.parent.gameObject;
		} else {
			Destroy(gameObject);
		}
    }

}
