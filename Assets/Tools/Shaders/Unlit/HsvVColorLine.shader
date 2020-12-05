Shader "Unlit/HsvVColorLine"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_hValue("HValue", float) = 0
		_sValue("SValue", float) = 1
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
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
				float _hValue;
				float _sValue;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}


				float3 Unity_ColorspaceConversion_HSV_RGB(float3 In)
				{
					float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
					float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
					return In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					float3 rgbTint = Unity_ColorspaceConversion_HSV_RGB(float3(_hValue, _sValue, i.uv.x));
					return col * float4(rgbTint, 1);
				}

			ENDCG
		}
		}
}
