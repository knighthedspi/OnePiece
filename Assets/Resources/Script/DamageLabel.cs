using UnityEngine;
using System.Collections;

/// <summary> 
// when player attacking monster, appear Damage Text. 
/// </summary>
public class DamageLabel : MonoBehaviour {

	//start Damage Text animation.
	// Fade-out and some move y axis
	public void go(float damage,GameObject panel,Vector2 lastPos){
#if(_NGUI_PRO_VERSION_)
		gameObject.transform.parent = GameObject.Find("drawAfter").transform;
		gameObject.transform.localScale = new Vector3(3,3,1);
#else
		gameObject.transform.parent = panel.transform;
		gameObject.transform.localScale = new Vector3(50,50,1);
#endif	
		gameObject.transform.localPosition = new Vector3(lastPos.x,lastPos.y,1);

		StartCoroutine("animStart");

		gameObject.GetComponent<UILabel>().text = damage.ToString();
	}

	//aniamtion corouine!
	IEnumerator animStart(){
		Vector3 v = gameObject.transform.localPosition;

		TweenAlpha.Begin( gameObject, 0.5f,  1f);
		TweenPosition.Begin( gameObject, 0.5f,  new Vector3(v.x,v.y+40,0));
		yield return new WaitForSeconds(0.8f);

		TweenAlpha.Begin( gameObject, 0.2f,  0f);
		yield return new WaitForSeconds(0.3f);

		Destroy(gameObject);
	}
}
