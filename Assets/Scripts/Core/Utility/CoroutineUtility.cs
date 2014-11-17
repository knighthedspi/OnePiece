using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CoroutineUtility : Singleton<CoroutineUtility> {

	public void WaitAndExecute(float delay, Action action) {
		StartCoroutine(IWaitAndExecute(delay, action));
	}

	public void WaitOneFrame(Action action) {
		StartCoroutine(IWaitOneFrame(action));
	}

	public void WaitFramesAndExecute(int count, Action action) {
		StartCoroutine(IWaitFramesAndExecute(count, action));
	}

	public void WaitUntilDoneThenExecute(IEnumerator coroutine, Action action){
		StartCoroutine(IWaitUntilDoneThenExecute(coroutine, action));
	}

	private IEnumerator IWaitAndExecute(float delay, Action action) {
        yield return new WaitForSeconds(delay);
		if (action != null) action();
    } 

	private IEnumerator IWaitOneFrame(Action action) {
		yield return null;
		if (action != null) action();
	}

	private IEnumerator IWaitFramesAndExecute(int count, Action action) {
		for (int i = 0; i < count; i++) {
			yield return null;
		}
		if (action != null) action();
	}

	private IEnumerator IWaitUntilDoneThenExecute(IEnumerator coroutine, Action action) {
		if(coroutine != null)
			yield return StartCoroutine(coroutine);
		if(action != null)
			action();
	}
}
