Shader "LearnShader/Parallax/Parallax-Base"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _HeightMap("Height Map", 2D) = "white" {}
        _Normal("Normal Map", 2D) = "bump" {}
		_Parallax("Parallax", Range(0,0.5)) = 0.1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase"  }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 mainUV : TEXCOORD0;
				half3 vertexWorld : TEXCOORD1;
				half3 normalWorld : TEXCOORD2;
				half3 tangentWorld : TEXCOORD3;
				half3 binormalWorld : TEXCOORD4;
				half3 viewDirWorld : TEXCOORD5;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _HeightMap;
			float4 _HeightMap_ST;
			sampler2D _Normal;
			float _Parallax;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.mainUV = TRANSFORM_TEX(v.uv, _MainTex);
				o.vertexWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalWorld = UnityObjectToWorldNormal(v.normal);
				o.tangentWorld = UnityObjectToWorldDir(v.tangent);
				o.binormalWorld = cross(o.normalWorld, o.tangentWorld) * v.tangent.w;
				o.viewDirWorld = UnityWorldSpaceViewDir(o.vertexWorld);
				// TANGENT_SPACE_ROTATION;

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float d = tex2D(_HeightMap, i.mainUV).r;
				half3x3 worldToTangent = half3x3(i.tangentWorld.xyz, i.binormalWorld, i.normalWorld);
				half3 viewDirForParallax = mul(worldToTangent, i.viewDirWorld);
				float2 parallax = ParallaxOffset(d, _Parallax, normalize(viewDirForParallax) );
				half3 normalMap = UnpackNormal(tex2D(_Normal, i.mainUV + parallax));
				half3 normalWorld = normalize(mul(normalMap, worldToTangent));
				float3 lightDirWorld = UnityWorldSpaceLightDir(i.vertexWorld);
				lightDirWorld = normalize(lightDirWorld);
				half nDotl = max(0, dot(normalWorld, lightDirWorld));
				fixed4 col = tex2D(_MainTex, i.mainUV + parallax );
				col.rgb = col.rgb * _LightColor0.rgb * nDotl;
				return col;
			}
			ENDCG
		}
	}
}
