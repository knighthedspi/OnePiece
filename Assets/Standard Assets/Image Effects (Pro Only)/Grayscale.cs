using UnityEngine;

[ExecuteInEditMode]
public class Grayscale : ImageEffectBase {

	public float ratio = 1;
	
	void OnRenderImage (RenderTexture source, RenderTexture destination){
		material.SetFloat("_Ratio", ratio);
		Graphics.Blit (source, destination, material);
	}
}