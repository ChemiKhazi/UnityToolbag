Shader "KBD/Mix Shader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OverLayer ("Base (RGB)", 2D) = "white" {}
		_Blend ("Refraction", 2D) = "black" {}
	}

	SubShader {
		Pass {
		CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _OverLayer;
			sampler2D _Blend;
			
			float4 frag(v2f_img IN) : COLOR {
				half4 base = tex2D(_MainTex, IN.uv);
				half4 over = tex2D(_OverLayer, IN.uv);
				half4 mix = tex2D(_Blend, IN.uv);
				return lerp(base, over, 1 - mix);
			}
		ENDCG
		}
	}
}
