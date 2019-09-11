Shader "LearnShader/LambertLight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "Lighting.cginc"
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldVertex: TEXCOORD1;
				float3 worldNormal: TEXCOORD2;
				UNITY_FOG_COORDS(3)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float3 lightDir = UnityWorldSpaceLightDir(i.worldVertex);
				lightDir = normalize(lightDir);
				float3 worldNormal = normalize(i.worldNormal);
				float3 lDotN = max(0, dot(lightDir, worldNormal));
				float3 dirDiff = _LightColor0 * lDotN;
				float3 inDirDiff = UNITY_LIGHTMODEL_AMBIENT.rgb;

				fixed4 mainColor = tex2D(_MainTex, i.uv);
				fixed4 finalColor;
				finalColor.xyz = mainColor.xyz * (dirDiff + inDirDiff);
				finalColor.a = mainColor.a;
				UNITY_APPLY_FOG(i.fogCoord, finalColor);
				return finalColor;
			}
			ENDCG
		}
	}
}
