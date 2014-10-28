using UnityEngine;
using System;

[RequireComponent(typeof(UI2DSprite))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class UI2DSpriteAnim : MonoBehaviour
{
	System.WeakReference spriteRenderer_ = new System.WeakReference(null);
	SpriteRenderer spriteRenderer {
		get { return (spriteRenderer_.Target ?? (spriteRenderer_.Target = GetComponent<SpriteRenderer>())) as SpriteRenderer; }
	}

	System.WeakReference ui2dSprite_ = new System.WeakReference(null);
	UI2DSprite ui2dSprite {
		get { return (ui2dSprite_.Target ?? (ui2dSprite_.Target = GetComponent<UI2DSprite>())) as UI2DSprite; }
	}

	void Reset () {
		if (spriteRenderer) { spriteRenderer.enabled  = false; }
	}

	void Update () {
		if (!spriteRenderer || !ui2dSprite) { return; }

		spriteRenderer.enabled = false;
		ui2dSprite.nextSprite = spriteRenderer.sprite; // NOT ui2dSprite_.sprite2D = spriteRenderer_.sprite;
	}
}
