Shader "KBD/Refract Shader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Refract ("Refraction", 2D) = "black" {}
		_Strength ("Refract Strength", Range(0,1)) = 0.1
	}

	SubShader {
		Pass {
		CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Refract;
			float _Strength;
			
			float4 frag(v2f_img IN) : COLOR {
				half4 refract = tex2D(_Refract, IN.uv);
				refract.rg = (((refract.rg) * 2.0) - 1.0) * refract.a * _Strength;
				half4 color = tex2D(_MainTex, IN.uv + refract.rg);
				return color;
			}
		ENDCG
		}
	}
}
