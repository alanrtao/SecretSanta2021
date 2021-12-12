Shader "Unlit/Post_Processing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DepthTex("Depth", 2D) = "white" {}
        _Fill("Edge Fill", Color) = (1, 1, 1, 1)
        _Width("Thickness (Screen Pixel)", Float) = 3
        _Sensitivity("Sensitivity", Float) = 10
        _Uniformity("Uniformity", Float) = 5
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _DepthTex;
            float4 _MainTex_ST;

            float _Width;
            float _Sensitivity;
            float _Uniformity;

            float4 _Fill;

            fixed sobel(sampler2D t, float2 i) {
                float u0 = i.x - _Width / _ScreenParams.x;
                float u1 = i.x + _Width / _ScreenParams.x;
                float v0 = i.y - _Width / _ScreenParams.y;
                float v1 = i.y + _Width / _ScreenParams.y;

                fixed p00 = LinearEyeDepth(tex2D(t, float2(u0, v0)).r);
                fixed p10 = LinearEyeDepth(tex2D(t, float2(i.x, v0)).r);
                fixed p20 = LinearEyeDepth(tex2D(t, float2(u1, v0)).r);
                fixed p01 = LinearEyeDepth(tex2D(t, float2(u0, i.y)).r);
                // fixed p11 = LinearEyeDepth(tex2D(t, float2(i.x, i.y)).r);
                fixed p21 = LinearEyeDepth(tex2D(t, float2(u1, i.y)).r);
                fixed p02 = LinearEyeDepth(tex2D(t, float2(u0, v1)).r);
                fixed p12 = LinearEyeDepth(tex2D(t, float2(i.x, v1)).r);
                fixed p22 = LinearEyeDepth(tex2D(t, float2(u1, v1)).r);

                fixed g2 = (p00 - p20 + 2 * p01 - 2 * p21 + p02 - p22) * (p00 - p20 + 2 * p01 - 2 * p21 + p02 - p22)
                    + (p00 + 2 * p10 + p20 - p02 - 2 * p12 - p22) * (p00 + 2 * p10 + p20 - p02 - 2 * p12 - p22);
                g2 -= _Sensitivity;
                g2 = clamp(g2, 0, 1);

                return pow(g2, 1/_Uniformity);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);

                fixed sob = sobel(_DepthTex, i.uv);

                // return col;
                return col + sob * _Fill;
            }


            ENDCG
        }
    }
}
