Shader "Unlit/HsvColorCircle"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100
		Cull Off

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 texcoord : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			float LengthScaleValue(float2 UV)
			{
				float2 delta = float2(UV.x - 0.5f, UV.y - 0.5f);
				return atan2(delta.x, delta.y) / 6.283;
			}

			float Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax)
			{
				return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
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
			float lengthScale = LengthScaleValue(i.uv);
			lengthScale = Unity_Remap_float(lengthScale, float2(-0.5f, 0.5f), float2(1, 0));
			float3 rgbTint = Unity_ColorspaceConversion_HSV_RGB(float3(lengthScale, 1, 1));
			return col * float4(rgbTint, 1);
			}

		ENDCG
	}
	}
}
