Shader "Hidden/ColourBlindFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Mode ("Mode", Int) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            int _Mode;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 ApplyProtanopia(float3 c)
            {
                return float3(
                    0.567 * c.r + 0.433 * c.g,
                    0.558 * c.r + 0.442 * c.g,
                    0.242 * c.g + 0.758 * c.b
                );
            }

            float3 ApplyDeuteranopia(float3 c)
            {
                return float3(
                    0.625 * c.r + 0.375 * c.g,
                    0.7   * c.r + 0.3   * c.g,
                    0.3   * c.g + 0.7   * c.b
                );
            }

            float3 ApplyTritanopia(float3 c)
            {
                return float3(
                    0.95 * c.r + 0.05 * c.g,
                    0.433 * c.g + 0.567 * c.b,
                    0.475 * c.g + 0.525 * c.b
                );
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                if (_Mode == 1)
                    col.rgb = ApplyProtanopia(col.rgb);
                else if (_Mode == 2)
                    col.rgb = ApplyDeuteranopia(col.rgb);
                else if (_Mode == 3)
                    col.rgb = ApplyTritanopia(col.rgb);

                return col;
            }
            ENDCG
        }
    }
}