using UnityEngine;
using System.Collections;

public class BumpLightMap : MonoBehaviour {
	public Color lightColor;
	public Texture specularBake;
	// Update is called once per frame
	void Update () {
		Color col = Color.white;
		Vector3 fwd = transform.forward;
		col.r = -fwd.x;
		col.g = -fwd.y;
		col.b = -fwd.z;
		Shader.SetGlobalColor ("_LightmapDir", col);
		Shader.SetGlobalColor ("_LightmapColor", lightColor);
		Shader.SetGlobalTexture ("_SpecCube", specularBake);
	}
	
	void OnDrawGizmos () {
		Update ();	
	}
	void OnDrawGizmosSelected () {
		Update ();	
	}
}
