Shader "Spine/Distort Blank" {
	Properties {
		_MainTex ("Texture to blend", 2D) = "black" {}
	}
	// 2 texture stage GPUs
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "SpineShader"="DistortBlank"}
		LOD 100

		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
		
			struct appdata {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};
		
			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				float4 color : COLOR;
			};
		
			v2f vert (appdata v) {
				v2f o;
				o.pos =  mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;
				o.color = v.color;
				return o;
			}
		
			uniform sampler2D _MainTex;
		
			float4 frag(v2f i) : COLOR {
				half4 texColor = tex2D(_MainTex, i.uv);
				return half4(0.5, 0.5, 0.5, texColor.a);
			}
			ENDCG
		}
	}
}