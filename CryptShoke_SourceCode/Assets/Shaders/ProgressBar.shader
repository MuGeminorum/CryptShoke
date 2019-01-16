// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'

Shader "Custom/ProgressBar" {

Properties {
	_Color ("Color", Color) = (1,1,1,1)
	_BackgroundTex ("Background (RGBA)", 2D) = "white" {}
	_ForegroundTex ("Foreground (RGBA)", 2D) = "white" {}
	_PositionAndScale ("Position and Scale", Vector) = (0,0,1,1)
	_Progress ("Progress", Range(0.0,1.0)) = 0.0
}

SubShader {
		//Tags { "Queue"="Transparent" }
		Tags { "Queue"="Overlay+1" }
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
        Pass {
 
CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain both vertex and fragment programs.
#pragma exclude_renderers gles
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

uniform sampler2D _BackgroundTex;
uniform sampler2D _ForegroundTex;

uniform float4 _Color;
uniform float _Progress;
uniform float4 _PositionAndScale;

struct v2f {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
};

v2f vert (appdata_base v)
{
    v2f o;
	// if this matrix is used, parent the progress bar to the camera:
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	// else if this transformation is used, you can have the progress bar anywhere:
	//o.pos.xy = v.vertex.xy * _PositionAndScale.zw + _PositionAndScale.xy;
	//o.pos.z = -0.5;
	//o.pos.w = 1;
	//o.pos = mul (glstate.matrix.projection, o.pos);
	// endif
    
    o.uv = TRANSFORM_UV(0);
		
    return o;
}


half4 frag( v2f i ) : COLOR
{
	half4 b = tex2D( _BackgroundTex, i.uv);
	half4 f = tex2D( _ForegroundTex, i.uv);
	
	half4 color = b;
	if( i.uv.x < _Progress ){
		color = (1.0 - f.a)*color + f.a*f;
	}
	
    return color*_Color;
}



ENDCG

    }
}

}