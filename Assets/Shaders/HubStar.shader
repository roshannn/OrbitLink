Shader "Custom/HubStar"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,0.9,0.5,1)
        _CoreColor ("Core Color", Color) = (1,1,1,1)
        _PulseSpeed ("Pulse Speed", Float) = 2.0
        _EmissionIntensity ("Emission Intensity", Float) = 2.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _CoreColor;
                float _PulseSpeed;
                float _EmissionIntensity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float pulse = (sin(_Time.y * _PulseSpeed) + 1.0) * 0.5;
                half3 finalColor = lerp(_Color.rgb, _CoreColor.rgb, pulse);
                // multiply by intensity for bloom
                finalColor *= _EmissionIntensity * (1.0 + pulse * 0.5);
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}