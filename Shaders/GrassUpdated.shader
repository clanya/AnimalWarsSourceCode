Shader "Custom/GrassUpdated"
{
    Properties
    {
        [NoScaleOffset] _Main_Texture ("Main Texture", 2D) = "white" {}
        _Main_Color ("Main Color",Color) = (1,1,1,1)
        _SwingWidth("Swing Width",range(0,1)) = 0.2
        _Speed ("Speed",float) = 1
        _Border ("Clip Strength",range(-1,1)) = 0.2
    }
    SubShader
    {
        Cull Off
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "RenderPipeline"="UniversalPipeline"}
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_Main_Texture);
        SAMPLER(sampler_Main_Texture);
        
        CBUFFER_START(UnityPerMaterial)
        float4 _Main_Texture_ST;
        float4 _Main_Color;
        float _SwingWidth;
        float _Speed;
        float _Border;
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
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float3 Swing(float3 objectPos)
            {
                float3 position = objectPos;
                float canSwing = 1 - step(objectPos.y, _Border);  //オブジェクト空間のy座標の大きさによってswingさせるか決める Borderより大きければSwingさせる.
                float swingValue = _SinTime.w * _Speed * canSwing * _SwingWidth;
                position.x += swingValue;
                return position;
            }
            
            Varyings vert (Attributes input)
            {
                Varyings output;
                output.position = TransformObjectToHClip(Swing(input.position));
                output.uv = TRANSFORM_TEX(input.uv, _Main_Texture);
                return output;
            }

            float4 frag (Varyings input) : SV_Target
            {
                float4 textureRGBA = SAMPLE_TEXTURE2D(_Main_Texture, sampler_Main_Texture, input.uv);
                clip(step( _Border,textureRGBA.a)-0.1);
                // if(step(textureRGBA.a, _Border))
                // {
                //     discard;
                // }
                float4 col = textureRGBA * _Main_Color;
                return col;
            }
            ENDHLSL
        }
    }
}