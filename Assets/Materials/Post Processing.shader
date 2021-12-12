Shader "Unlit/Post Processing"
{
	//show values to edit in inspector
	Properties
	{
		_MainTex("Main  Texture", 2D) = "white" {}
		_DepthTex("Depth Texture", 2D) = "white" {}
	}
	SubShader
	{
		// markers that specify that we don't need culling 
		// or comparing/writing to the depth buffer
		Cull Off
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			//include useful shader functions
			#include "UnityCG.cginc"

			//define vertex and fragment shader
			#pragma vertex vert
			#pragma fragment frag

			//the rendered screen so far
			sampler2D _MainTex;
			sampler2D _DepthTex;

			//the depth texture
			sampler2D _CameraDepthTexture;

			//the object data that's put into the vertex shader
			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			//the data that's used to generate fragments and can be read by the fragment shader
			struct v2f {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//the vertex shader
			v2f vert(appdata v) {
				v2f o;
				//convert the vertex positions from object space to clip space so they can be rendered
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			//the fragment shader
			fixed4 frag(v2f i) : SV_TARGET
			{

				//get depth from depth texture
				fixed r = tex2D(_DepthTex, i.uv).r;

				fixed4 col = tex2D(_MainTex, i.uv);

				return col; // Linear01Depth(r);
			}
			ENDCG
		}
	}
}