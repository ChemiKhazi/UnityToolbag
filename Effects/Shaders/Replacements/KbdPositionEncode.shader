// unlit, vertex colour, alpha blended
// cull off

Shader "Kbd/Replacements/Position Encode" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Cutoff", Float) = 0.5
	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off
		Lighting Off
		Cull Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 110
		
		Pass 
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f_vct members scrPos)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex, _FogTexture;
			float4 _MainTex_ST;
			float _Cutoff, _EffectCeiling;

			struct vin_vct 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_vct
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				float3 worldPos = mul (_Object2World, v.vertex).xyz;
				o.worldPos = float4(worldPos, 0);
				o.worldPos.w = COMPUTE_DEPTH_01;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
				//float d = smoothstep(_WorldSpaceCameraPos.z + _ProjectionParams.y, _WorldSpaceCameraPos.z + _ProjectionParams.z, round(i.worldPos.z));
				float2 height = EncodeFloatRG(smoothstep(0, _EffectCeiling, i.worldPos.y));
				float cutoff = col.a;
				//if (col.a > 0.85)
				//	cutoff = col.a;
				return float4(i.worldPos.w, height.x,height.y, cutoff);
			}
			
			ENDCG
		}

	}
 
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off Fog { Mode Off }
		LOD 100

		BindChannels 
		{
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
			Bind "Color", color
		}

		Pass 
		{
			Lighting Off
			SetTexture [_MainTex] { combine texture * primary } 
		}
	}
}
