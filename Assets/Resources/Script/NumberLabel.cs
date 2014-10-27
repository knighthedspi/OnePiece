using UnityEngine;
using System.Collections;

/// <summary> 
//  Number Label
//  number text is not just set. number incrementing to target number 
/// </summary>
public class NumberLabel : MonoBehaviour {

	private UILabel _uiLabel;
	private int _number;
	private int _toN;

	// Use this for initialization
	void Start () {
		_uiLabel = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
		if(_toN == _number)return ;

		int i = (_toN - _number)/50;
		if(i <= 0)i = 1;

		if(_toN > _number){
			setNumber(_number + i);
		}else{
			setNumber(_number - i);
		}

	}

	//set text
	public void setText(string str){
		if(_uiLabel == null){
			_uiLabel = GetComponent<UILabel>();
		}
		_uiLabel.text = str;
	}

	//just set number
	public void setNumber(int n){
		_number = n;
		setText(_number.ToString());
	}

	//get number
	public int number(){
		return _number;
	}

	//set target number. Update Function checked _toN and increments now number
	public void setNumberTo(int toN){
		_toN = toN;
	}

	//add target number.
	public void addNumberTo(int n){
		_toN += n;
	}
}
