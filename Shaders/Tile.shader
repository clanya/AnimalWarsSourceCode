Shader "Custom/Tile"
{
    Properties
    {
        _Main_Texture ("Main Texture", 2D) = "white" {}
        _Mask_Texture ("Alpha Map", 2D) = "white" {}
        _Alpha_Value ("Alpha Value",range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline"}
        Blend SrcAlpha OneMinusSrcAlpha
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_Main_Texture);
        SAMPLER(sampler_Main_Texture);
        TEXTURE2D(_Mask_Texture);
        SAMPLER(sampler_Mask_Texture);
        

        CBUFFER_START(UnityPerMaterial)
        float4 _Main_Texture_ST;
        float4 _Mask_Texture_ST;
        float _Alpha_Value;
        CBUFFER_END

        ENDHLSL
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_cbuffer
            
            struct Attributes
            {
                float3 position : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            Varyings vert (Attributes input)
            {
                Varyings output;
                output.position = TransformObjectToHClip(input.position);
                output.uv = TRANSFORM_TEX(input.uv, _Main_Texture);
                output.color = input.color;
                return output;
            }

            float4 frag (Varyings input) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_Main_Texture, sampler_Main_Texture, input.uv) * input.color;
                //warning:implicit truncation of vector type　を消すためのキャスト
                float maskTextureAlpha = (float)SAMPLE_TEXTURE2D(_Mask_Texture, sampler_Mask_Texture, input.uv);
                col.a = saturate(maskTextureAlpha + _Alpha_Value);
                return col;
            }
            ENDHLSL
        }
    }
}