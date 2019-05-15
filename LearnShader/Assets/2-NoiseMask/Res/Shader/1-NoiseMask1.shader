Shader "LearnShader/1-NoiseMask1"
{
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" {}
		_MaskTex ("MaskTexture", 2D) = "white" {}
		_MaskColor ("MaskColor", Color) = (0.5,0.5,0.5,1)
		_ExpandDis("ExpandDis", Range(0, 2)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
    			float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				half4 mask_color : Color;
			};

			sampler2D _MainTex;
			sampler2D _MaskTex;
			half4 _MaskColor;
			float4 _MainTex_ST;
			float _ExpandDis;

			fixed blendScreen(fixed base, fixed blend) {
				return 1.0-((1.0-base)*(1.0-blend));
			}

			fixed4 blendScreen(fixed4 base, fixed4 blend) {
				return fixed4(blendScreen(base.r,blend.r),blendScreen(base.g,blend.g),blendScreen(base.b,blend.b), base.a);
			}

			v2f vert (appdata v)
			{
				v2f o;
				float4 mask_tex_color = tex2Dlod(_MaskTex, float4(v.uv.x, v.uv.y + _Time.y, 0, 0));
				o.mask_color = mask_tex_color * _MaskColor;
				v.vertex.xyz += (v.normal * _ExpandDis * mask_tex_color);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = blendScreen(tex2D(_MainTex, i.uv), i.mask_color);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}


			ENDCG
		}
	}
}
