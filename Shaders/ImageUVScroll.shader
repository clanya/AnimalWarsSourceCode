Shader "MyShader/ImageUVScroll"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _ScrollSpeedX("Scroll Speed X", Range(-5.0,5.0)) = 0
        _ScrollSpeedY("Scroll Speed Y", Range(-5.0,5.0)) = 0
    }
    SubShader
    {        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float _ScrollSpeedX;
        float _ScrollSpeedY;
        CBUFFER_END
        ENDHLSL
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct Attributes
            {
                float3 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            Varyings vert (Attributes input)
            {
                Varyings output;
                output.position = TransformObjectToHClip(input.position);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            float4 frag (Varyings input) : SV_Target
            {
                float2 scrolledUV = input.uv;
                scrolledUV.x = frac(input.uv.x + _ScrollSpeedX * _Time.x);
                scrolledUV.y = frac(input.uv.y + _ScrollSpeedY * _Time.x);
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, scrolledUV);
                return col;
            }
            ENDHLSL
        }
    }
}
