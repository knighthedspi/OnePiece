using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
// Particle Effect. 
// will use when Player Attack to monster 
// this particle effect moved usign bezier curve
/// </summary>
public class PlayerAttackParticle : MonoBehaviour {
	public delegate void OnFinished(Vector2 lastPos);

	public GameObject _imgPrefab;
	private List<Particle> _particles;

	private bool _isPlay = false;

	private Vector2 _center;

	private Vector2 _start;
	private Vector2 _end;
	private Vector2 _c1;
	private Vector2 _c2;

	private Color _color;

	private float _t = 0;
	private float _speed = UnityEngine.Random.Range(0,10)/90f;

	public OnFinished Finish;

	// update particle effect moving using bezier curve function
	void updateParticle(){
		if( _t < 0.95 ){
			_t += (1-_t)*(0.08f+_speed);
			_center = bezierCurve(_start,_end,_c1,_c2,_t);

			//making; to center img
			GameObject particle = (GameObject)Instantiate(_imgPrefab);
			particle.transform.parent = this.transform.parent;
			particle.transform.localPosition = new Vector3(_center.x,_center.y);
#if(_NGUI_PRO_VERSION_)
			particle.transform.localScale = new Vector3(1,1);
#else
			particle.transform.localScale = new Vector3(30,30);
#endif

			float rand = UnityEngine.Random.Range(0,614)/100.0f;
			float radius = UnityEngine.Random.Range(30,60);

			float x = Mathf.Sin( rand ) * radius + _center.x;
			float y = Mathf.Cos( rand ) * radius + _center.y;

			float scale = UnityEngine.Random.Range(0,30);

			Particle p = particle.GetComponent<Particle>();
			p._moveTo = new Vector2(x,y);
			p._colorTo = new Color(_color.r,_color.g,_color.b,0.0f);
			p._scaleTo = new Vector2(scale,scale);

			p.setColor(new Color(_color.r,_color.g,_color.b,1));
			p.play();

			_particles.Add(p);
		}else{
			if(_isPlay){
				Finish(_center);
				_isPlay = false;
			}
		}
	}
	
	// Update is called once per frame
	// check particle die, updating particle, destroy die particle
	void Update () {
		for(int i =_particles.Count - 1 ; i >= 0; i--) {
			Particle _p = _particles[i];
			if(_p._die){
				_particles.Remove(_p);
				Destroy(_p.gameObject);
			}
		}

		if(_isPlay){
			updateParticle();
		}

		if( _particles.Count == 0 ){
			Destroy(this.gameObject);
		}

	}

	//bezier curve function 
	Vector2 bezierCurve(Vector2 start, Vector2 end, Vector2 a1, Vector2 a2,float t){
		float cx = 3.0f * (a1.x - start.x);
		float bx = 3.0f * (a2.x - a1.x) - cx;
		float ax = end.x - start.x - cx - bx;

		float cy = 3.0f * (a1.y - start.y);
		float by = 3.0f * (a2.y - a1.y);
		float ay = end.y - start.y - cy - by;

		float tSquared = t*t;
		float tCubed = tSquared * t;

		return new Vector2( (ax * tCubed) + (bx * tCubed) + (cx * t) + start.x,
							(ay * tCubed) + (by * tCubed) + (cy * t) + start.y );
	}

	// generate Particle's Effect
	public void generate(GameObject panel, Block b,Vector2 v1,Vector2 v2){
		this.transform.parent = panel.transform;
		this.transform.localPosition = new Vector3(0,0,0);
		this.transform.localScale = new Vector3(1,1,1);
		_color = b.particleColor;

		float r = UnityEngine.Random.Range(0,400)-200;

		_center = v1;
		_start = v1;
		_end = v2;
		_c1 = new Vector2(v1.x - r,v1.y);
		_c2 = new Vector2(v2.x - r,v2.y);

		_particles = new List<Particle>();

		_isPlay = true;
	}
}
