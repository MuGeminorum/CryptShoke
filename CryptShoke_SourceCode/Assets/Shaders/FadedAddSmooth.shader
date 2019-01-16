Shader "Particles/Faded Additive (Soft) No ColorMask" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_FallOff ("Falloff Cube", CUBE) = "" { TexGen CubeReflect }
	_Noise ("Particulates", 2D) = "" {TexGen EyeLinear}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend One OneMinusSrcColor
	//ColorMask RGB
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
		Bind "Normal", normal
	}
	
	// ---- Dual texture cards
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
			SetTexture [_MainTex] {
				combine previous * previous alpha, previous
			}
			SetTexture [_FallOff] {
				combine previous * texture	
			}
		}
	}
	
/*	// ---- Single texture cards (does not do particle colors)
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * texture alpha, texture
			}
		}
	}
*/
}
}