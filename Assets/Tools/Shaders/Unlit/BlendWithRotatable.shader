Shader "Unlit/BlendWithRotatable"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SecondTex("SecondTex", 2D) = "white" {}
		_AngleDeg("Angle", Range(0,360)) = 0
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" }
			LOD 100
			Blend SrcAlpha OneMinusSrcAlpha

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
				sampler2D _SecondTex;
				float4 _MainTex_ST;
				float _AngleDeg;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				float2 Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation)
				{
					Rotation = Rotation * (3.1415926f / 180.0f);
					UV -= Center;
					float s = sin(Rotation);
					float c = cos(Rotation);
					float2x2 rMatrix = float2x2(c, -s, s, c);
					rMatrix *= 0.5;
					rMatrix += 0.5;
					rMatrix = rMatrix * 2 - 1;
					UV.xy = mul(UV.xy, rMatrix);
					UV += Center;
					return UV;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float2 uv = i.uv;
					fixed4 col = tex2D(_MainTex, uv);
					uv = Unity_Rotate_Degrees_float(uv, float2(0.5f, 0.5f), _AngleDeg);
					fixed4 col2 = tex2D(_SecondTex, uv);
					return col * col2;
				}
			ENDCG
		}
		}
}
