using UnityEngine;
using System.Collections.Generic;

public class CharacterShadow : MonoBehaviour
{
	public Light lightSource;
	public GameObject target;
	public float castingDistance = 10.0f;
	public float lightness = 0.4f;
	public int textureSize = 256;
	public Texture fadeoutTexture;
	
	public Color shadowColor;
	public float shadeChangeSpeed = 5;
	//public CharacterShadow shadowCaster;
	
	private RenderTexture shadowmap;
	private GameObject child;
	private int savedPixelLightCount;
	private int oldTargetLayer;
	private int targetLayer;
	
	private float shade;
	private List<Material> allMaterials;
	private LayerMask mask;
	
	// Layer internally used for shadow rendering.
	// Should not be used for anything in the game.
	// By default this is the last user layer (31).
	private static int privateLayer = 31;
	
	
	private static string shadowMatString =
@"Shader ""Hidden/ShadowMat"" {
	Properties {
		_Color (""Color"", Color) = (0,0,0,0)
	}
	SubShader {
		Pass {
			ZTest Greater Cull Off ZWrite Off
			Color [_Color]
			SetTexture [_Dummy] { combine primary }
		}
	}
	Fallback off
}";

	private Material m_ShadowMaterial = null;
	private Material shadowMaterial {
		get {
			if (m_ShadowMaterial == null) {
				m_ShadowMaterial = new Material (shadowMatString);
				m_ShadowMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
				m_ShadowMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_ShadowMaterial;
		} 
	}
	
	private static string projectorMatString =
@"	Shader ""Hidden/ShadowProjectorMultiply"" { 
	Properties {
		_ShadowTex (""Cookie"", 2D) = ""white"" { TexGen ObjectLinear }
		_FalloffTex (""FallOff"", 2D) = ""white"" { TexGen ObjectLinear	}
	}
	Subshader {
		Pass {
			ZWrite off
			Offset -1, -1
			Fog { Color (1, 1, 1) }
			AlphaTest Greater 0
			ColorMask RGB
			Blend DstColor Zero
			SetTexture [_ShadowTex] {
				combine texture, ONE - texture
				Matrix [_Projector]
			}
			SetTexture [_FalloffTex] {
				constantColor (1,1,1,0)
				combine previous lerp (texture) constant
				Matrix [_ProjectorClip]
			}
		}
	}
}";

	private Material m_ProjectorMaterial = null;
	private Material projectorMaterial {
		get {
			if (m_ProjectorMaterial == null) {
				m_ProjectorMaterial = new Material (projectorMatString);
				m_ProjectorMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
				m_ProjectorMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_ProjectorMaterial;			
		}
	}
	
	void Start()
	{
		if (!lightSource) {
			GameObject go = GameObject.FindGameObjectWithTag("MainLight");
			if (go.light)
				lightSource = go.light;
		}
		
		if( !target )
		{
			Debug.Log("No target assigned! Disabling CharacterShadow script");
			enabled = false;
			return;
		}
		
		targetLayer = target.layer;
		if( targetLayer == 0 ) {
			Debug.Log("Warning: target object should use a separate layer");
			// still continue...
		}
		
		// create a child camera/projector object
		child = new GameObject("ChildCamProjector", typeof(Camera), typeof(Projector), typeof(CharacterShadowHelper));
		child.camera.clearFlags = CameraClearFlags.Color;
		child.camera.backgroundColor = Color.white;
		child.camera.cullingMask = (1<<privateLayer);
		child.camera.isOrthoGraphic = true;
		
		Projector proj = (Projector)child.GetComponent(typeof(Projector));
		proj.isOrthoGraphic = true;
		proj.ignoreLayers = (1<<targetLayer);
		proj.material = projectorMaterial;
		proj.material.SetTexture("_FalloffTex", fadeoutTexture);
		
		child.transform.parent = transform;
		child.transform.localPosition = Vector3.zero;
		child.transform.localRotation = Quaternion.identity;
		
		if (lightSource) {
			if (lightSource.type == LightType.Directional) {
				child.transform.rotation = lightSource.transform.rotation;
			}
			else {
				transform.position = lightSource.transform.position;
			}
		}
		
		allMaterials = new List<Material>();
		Object[] allRenderers = target.GetComponentsInChildren(typeof(Renderer));
		foreach (Object myRenderer in allRenderers) {
			Material[] theseMaterials = ((Renderer)myRenderer).materials;
			foreach (Material myMaterial in theseMaterials)
				allMaterials.Add(myMaterial);
		}
		
		// Add player's own layer to mask
		mask = 1 << gameObject.layer;
		// Add Igbore Raycast layer to mask
		mask |= 1 << LayerMask.NameToLayer("Ignore Raycast");
		// Invert mask
		mask = ~mask;
	}
	
	void LateUpdate()
	{
		if (!shadowmap) {
			shadowmap = new RenderTexture( textureSize, textureSize, 16 );
			shadowmap.isPowerOfTwo = true;

			child.camera.targetTexture = shadowmap;			
			Projector proj = (Projector)child.GetComponent(typeof(Projector));
			proj.material.SetTexture("_ShadowTex", shadowmap);
		}
		OrientToEncloseTarget();
		
		Vector3 center = transform.position;
		if (renderer)
			center = renderer.bounds.center;
		
		
		// Should we increase or decrease shade?
		float newShade;
		Vector3 lightDir = Vector3.up;
		if (lightSource)
			lightDir = -lightSource.transform.forward;
		if (Physics.Raycast (center, lightDir, Mathf.Infinity, mask))
			newShade = Mathf.Clamp01(shade + Time.deltaTime * shadeChangeSpeed);
		else 
			newShade = Mathf.Clamp01(shade - Time.deltaTime * shadeChangeSpeed);
		
		if (newShade != shade) {
			shade = newShade;
			
			// Calculate shade color
			Color color = Color.Lerp(Color.white, shadowColor, shade);
			
			// Set shade color in textures
			foreach (Material myMaterial in allMaterials)
				myMaterial.color = color;
			
			lightness = Mathf.Lerp(0.4f, 1, shade);
		}
	}
	
	void OnDisable()
	{
		Destroy( shadowmap );
		shadowmap = null;
	}
	
	private static void SetLayerRecursive (Transform tr, int layer, int whereLayer)
	{
		GameObject go = tr.gameObject;
		if (go.layer == whereLayer)
			go.layer = layer;
		foreach (Transform child in tr) {
			SetLayerRecursive (child, layer, whereLayer);
		}
	}
	
	public void OnPreCull()
	{
		savedPixelLightCount = QualitySettings.pixelLightCount;
		oldTargetLayer = target.layer;
		SetLayerRecursive (target.transform, privateLayer, oldTargetLayer);
	}
	
	public void OnPostRender()
	{
		SetLayerRecursive (target.transform, oldTargetLayer, privateLayer);
		
		//RenderTexture oldRT = RenderTexture.active;
		//RenderTexture.active = shadowmap;
		shadowMaterial.color = new Color(lightness,lightness,lightness,lightness);
		
		GL.PushMatrix();
		GL.LoadOrtho();
		// LoadOrtho loads -1..100 depth range in Unity's
		// conventions. We invert it for OpenGL
		// and put the depth at the very far end.
		const float depth = -99.99f;
		
		for (int i = 0; i < shadowMaterial.passCount; i++)
		{
			shadowMaterial.SetPass (i);
			GL.Begin (GL.QUADS);
			GL.TexCoord2(0,0); GL.Vertex3( 0, 0, depth );
			GL.TexCoord2(1,0); GL.Vertex3( 1, 0, depth );
			GL.TexCoord2(1,1); GL.Vertex3( 1, 1, depth );
			GL.TexCoord2(0,1); GL.Vertex3( 0, 1, depth );
			GL.End();
		}
		GL.PopMatrix ();
		
		//RenderTexture.active = oldRT;
		
		QualitySettings.pixelLightCount = savedPixelLightCount;
	}
	
	private void OrientToEncloseTarget()
	{
		Vector3 center = target.transform.position;
		float radius = 5.0f;
		if( target.renderer ) {
			Bounds b = target.renderer.bounds;
			center = b.center;
			radius = b.extents.magnitude * 1.2f;
		}
		
		if (lightSource) {
			if (lightSource.type == LightType.Directional) {
				child.transform.rotation = lightSource.transform.rotation;
				transform.position = center - child.transform.forward * radius;
			}
			else {
				transform.position = lightSource.transform.position;
				child.transform.LookAt( center );
			}
		}
		else {
			child.transform.LookAt( center );
		}
		
		float distance = (center - transform.position).magnitude;
		
		child.camera.orthographicSize = radius;
		child.camera.nearClipPlane = distance - radius;
		child.camera.farClipPlane = distance + radius;
		
		Projector proj = (Projector)child.GetComponent(typeof(Projector));
		proj.orthoGraphicSize = radius;
		proj.nearClipPlane = distance;
		proj.farClipPlane = distance + castingDistance;
		proj.material.SetTexture("_ShadowTex", shadowmap);
	}
}
