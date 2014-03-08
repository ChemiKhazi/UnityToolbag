Shader "Kbd/2DFog" {
	Properties {
		_MainTex ("Render Input", 2D) = "white" {}
		_PositionTex ("Position", 2D) = "white" {}
		_FogColor ("Fog Color", Color) = (0.1, 0.1, 0.1, 1)
		_FogColorFar ("Fog Color Far", Color) = (0.1, 0.1, 0.1, 1)
		_FogTex ("Fog Texture", 2D) = "white" {}
		_FogParams ("Fog Params", Vector) = (30, 45, 0.3, 0.9)
	}

	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		Pass {
			CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				sampler2D _PositionTex;
				sampler2D _FogTex;
				fixed4 _FogColor, _FogColorFar;
				float4 _FogParams;
				float _FogCutoff, _EffectCeiling;
				
				float4 frag(v2f_img IN) : COLOR {
					float4 pos = tex2D(_PositionTex, IN.uv);
					float height = DecodeFloatRG(pos.yz) * _EffectCeiling;
					half4 color = tex2D(_MainTex, IN.uv);
					half4 fog = tex2D(_FogTex, IN.uv);

					// Fog Z intensity
					float fogIntensity = smoothstep(_FogParams.x, _FogParams.y, pos.x);
					// Get fog color lerped
					half4 fogColor = lerp(_FogColor, _FogColorFar, fogIntensity);
					// Multiply intensity by height falloff
					float fogHeightMultiply = smoothstep(_FogParams.w + _FogParams.z, _FogParams.z, height);
					fogIntensity *= fogHeightMultiply;

					half4 outColor = lerp(color, fogColor * fog, fogIntensity * fogColor.a );
					outColor.a = color.a;
					
					return outColor;
				}
			ENDCG
		}
	}
}
