Shader "Unlit/HsvTintShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	    _HueOffset("HueOffset", float) = 0
		_SaturationOffset("SaturationOffset", float) = 0
		_BrightnessOffset("BrightnessOffset", float) = 0
	}
		SubShader
	{
		Tags { "Queue" = "Transparent"
		"RenderType" = "Transparent" }
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _HueOffset;
			float _SaturationOffset;
			float _BrightnessOffset;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			float3 Unity_ColorspaceConversion_RGB_HSV_float(float3 In)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 P = lerp(float4(In.bg, K.wz), float4(In.gb, K.xy), step(In.b, In.g));
				float4 Q = lerp(float4(P.xyw, In.r), float4(In.r, P.yzx), step(P.x, In.r));
				float D = Q.x - min(Q.w, Q.y);
				float  E = 1e-10;
				return float3(abs(Q.z + (Q.w - Q.y) / (6.0 * D + E)), D / (Q.x + E), Q.x);
			}

			float3 Unity_ColorspaceConversion_HSV_RGB_float(float3 In)
			{
				float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
				return In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			    clip(col.a - 0.01);
				float3 hsv = Unity_ColorspaceConversion_RGB_HSV_float(col.rgb);
				hsv.x += _HueOffset;
				hsv.y += _SaturationOffset;
				hsv.z += _BrightnessOffset;
				return float4(Unity_ColorspaceConversion_HSV_RGB_float(hsv), col.a);
			}
			ENDCG
		}
	}
}
