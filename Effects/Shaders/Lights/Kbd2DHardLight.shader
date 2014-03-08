Shader "Kbd/Lighting/HardLight" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_PositionTex ("Position", 2D) = "white" {}
		_LightColor ("Light Color", Color) = (1, 1, 1, 1)
		_LightOverlay ("Light Overlay", 2D) = "white" {}
		_LightDepth ("Depth", Float) = 0
		_LightVolume ("Volume", Float) = 5
		_MasterOpacity ("Light Opacity", Float) = 1
	}

	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		Pass {
			CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Assets/Scripts/Level/Effects/Shaders/Photoshop.cginc"

				sampler2D _MainTex, _LightOverlay, _PositionTex;
				fixed4 _LightColor;
				float _LightDepth, _LightVolume, _MasterOpacity;
				
				float4 frag(v2f_img IN) : COLOR
				{
					float4 pos = tex2D(_PositionTex, IN.uv);
					
					half4 color = tex2D(_MainTex, IN.uv);
					half4 light = tex2D(_LightOverlay, IN.uv) * _LightColor;
					
					float depthDiff = abs(pos.x - _LightDepth);
					float lightIntensity = smoothstep(_LightVolume, 0, depthDiff - _LightVolume);

					return HardLight(color, light * lightIntensity * _MasterOpacity);
				}
			ENDCG
		}
	}
}
