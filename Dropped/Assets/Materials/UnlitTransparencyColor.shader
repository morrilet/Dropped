Shader "Unlit/UnlitTransparencyColor"
{
     Properties
     {
         _Color ("Color Tint", Color) = (1,1,1,1)   
         _MainTex ("Base (RGB) Alpha (A)", 2D) = "white"
     }
 
     Category
     {
         Lighting Off
         ZWrite Off
         //ZWrite On  // uncomment if you have problems like the sprite disappear in some rotations.

         Cull back
         Blend SrcAlpha OneMinusSrcAlpha
         //AlphaTest Greater 0.001  // uncomment if you have problems like the sprites or 3d text have white quads instead of alpha pixels.

         Tags {Queue=Transparent}
     }
 
         SubShader
         {
             Pass
             {
                 SetTexture [_MainTex]
                 {
                    ConstantColor [_Color]
                    Combine Texture * constant
                 }
             }
         }
}


//	Properties
//	{
//		_MainTex ("Texture", 2D) = "white" {}
//	}
//	SubShader
//	{
//		Tags { "RenderType"="Opaque" }
//		LOD 100
//
//		Pass
//		{
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			// make fog work
//			#pragma multi_compile_fog
//			
//			#include "UnityCG.cginc"
//
//			struct appdata
//			{
//				float4 vertex : POSITION;
//				float2 uv : TEXCOORD0;
//			};
//
//			struct v2f
//			{
//				float2 uv : TEXCOORD0;
//				UNITY_FOG_COORDS(1)
//				float4 vertex : SV_POSITION;
//			};
//
//			sampler2D _MainTex;
//			float4 _MainTex_ST;
//			
//			v2f vert (appdata v)
//			{
//				v2f o;
//				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//				UNITY_TRANSFER_FOG(o,o.vertex);
//				return o;
//			}
//			
//			fixed4 frag (v2f i) : SV_Target
//			{
//				// sample the texture
//				fixed4 col = tex2D(_MainTex, i.uv);
//				// apply fog
//				UNITY_APPLY_FOG(i.fogCoord, col);
//				return col;
//			}
//			ENDCG
//		}
//	}
//}
