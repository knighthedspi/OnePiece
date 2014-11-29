using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
// Particle Effect. 
// will use when Block Destroy 
/// </summary>
public class BlockDestroyParticle : MonoBehaviour {
	public GameObject _imgPrefab;
	private List<Particle> _particles;

	// generate Particle's Effect
	// argument _b is block that will be destroy
	public void generate(Block _b) {
		this.transform.localPosition = new Vector3(_b.transform.localPosition.x,_b.transform.localPosition.y,0);
		this.transform.localScale = new Vector3(1,1,1);

		Color color = _b.particleColor;

		_particles = new List<Particle>();
		for(int i = 0; i < Config.PARTICLE_NUM; i++){
			GameObject particle = (GameObject)Instantiate(_imgPrefab);
			particle.transform.parent = this.transform.parent;
			particle.transform.localPosition = this.transform.localPosition;
#if(_NGUI_PRO_VERSION_)
			particle.transform.localScale = new Vector3(1,1);
#else
			particle.transform.localScale = new Vector3(30,30);
#endif
			float rand = UnityEngine.Random.Range(0,614)/100.0f;
			float radius = UnityEngine.Random.Range(30,60);

			float x = Mathf.Sin( rand ) * radius;
			float y = Mathf.Cos( rand ) * radius;

			float scale = UnityEngine.Random.Range(0,30);

			Particle p = particle.GetComponent<Particle>();
			p._moveTo = new Vector2(x + this.transform.localPosition.x,y + this.transform.localPosition.y);
			p._colorTo = new Color(color.r,color.g,color.b,0.0f);
			p._scaleTo = new Vector2(scale,scale);

			p.setColor(new Color(color.r,color.g,color.b,1));
			p.play();

			_particles.Add(p);
		}
	}
	
	// Update is called once per frame
	//particle die check and if all particle die, destroy self 
	void Update () {
		foreach(Particle _p in _particles){
			if(_p._die == false)return ;
		}
		foreach(Particle _p in _particles){
			Destroy(_p.gameObject);
		}
		Destroy(gameObject);
	}
}
