using UnityEngine;
using System.Collections;

public enum BlockAnimType
{
	Empty = 1,
	ChangeBlock = 2,
}

public class BlockAnim : MonoBehaviour {

	private Animator _animator;
	private Block _block;
	public delegate void OnAnimComplete();
	public OnAnimComplete onComplete;

	void Awake()
	{
		_animator 	= GetComponent<Animator> ();
		_block = GetComponentInParent<Block> ();
	}
	public void OnAnimationFinish()
	{
		onComplete ();
	}
}
