﻿Shader "LearnShader/LightModel/LamberBlinnPhongLight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SpecularColor ("SpecularColor", Color) = (1, 1, 1, 1)
		_Specular ("Specular", Range(0, 8)) = 1
		_Gloss ("Gloss", Range(0, 2)) = 1
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
				float2 uv : TEXCOORD0;
				float3 worldVertex: TEXCOORD1;
				float3 worldNormal: TEXCOORD2;
				float3 worldViewDir: TEXCOORD3;
				UNITY_FOG_COORDS(4)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _SpecularColor;
			half _Specular;
			half _Gloss;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldViewDir = UnityWorldSpaceViewDir(o.worldVertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldVertex));
				float3 worldNormal = normalize(i.worldNormal);

				float3 lDotN = max(0, dot(lightDir, worldNormal));
				float3 dirDiff = _LightColor0 * lDotN;
				float3 inDirDiff = UNITY_LIGHTMODEL_AMBIENT.rgb;
				fixed4 mainColor = tex2D(_MainTex, i.uv);
				fixed3 diffColor = mainColor * (dirDiff + inDirDiff);

				float3 worldViewDir = normalize(i.worldViewDir);
				half3 h = normalize(lightDir + worldViewDir);
				half3 nh = max(0, dot(worldNormal, h));
				float spec = pow(nh, _Specular * 128.0) * _Gloss;
				fixed3 specColor = _LightColor0 * _SpecularColor * spec;

				fixed4 finalColor;
				finalColor.rgb = diffColor + specColor;
				finalColor.a = mainColor.a;
				UNITY_APPLY_FOG(i.fogCoord, finalColor);
				return finalColor;
			}
			ENDCG
		}
	}
}
