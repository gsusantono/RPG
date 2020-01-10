Shader "Unlit/GroundShader"
{
	Properties
	{
		_Base ("Base", 2D) = "white" {}
		_LayerR ("LayerR", 2D) = "white" {}
		_LayerG ("LayerG", 2D) = "white" {}
		_LayerB ("LayerB", 2D) = "white" {}
		_Rate ("Rate", 2D) = "white" {}

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;			
				float2 uv_Base : TEXCOORD1;				
				float4 uv_layerRG : TEXCOORD2;				
				float4 uv_layerBA : TEXCOORD3;
				UNITY_FOG_COORDS(4)
				float4 vertex : SV_POSITION;
			};

			sampler2D _Base;
			float4 _Base_ST;

			sampler2D _LayerR;
			float4 _LayerR_ST;

			sampler2D _LayerG;
			float4 _LayerG_ST;

			sampler2D _LayerB;
			float4 _LayerB_ST;

			sampler2D _Rate;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv_Base = TRANSFORM_TEX(v.uv, _Base);
				o.uv_layerRG.xy = TRANSFORM_TEX(v.uv, _LayerR);
				o.uv_layerRG.zw = TRANSFORM_TEX(v.uv, _LayerG);
				o.uv_layerBA.xy = TRANSFORM_TEX(v.uv, _LayerB);

				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col  = tex2D(_Base, i.uv_Base);
				fixed4 colR = tex2D(_LayerR, i.uv_layerRG.xy);
				fixed4 colG = tex2D(_LayerG, i.uv_layerRG.zy);
				fixed4 colB = tex2D(_LayerB, i.uv_layerBA.xy);

				fixed4 rate = tex2D(_Rate, i.uv);

				col = lerp(col,colR,rate.r);
				col = lerp(col,colG,rate.g);
				col = lerp(col,colB,rate.b);

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
