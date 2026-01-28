Shader "Custom/NeonSun"
{
    Properties
    {
        [HDR] _CoreColor ("Core Color", Color) = (1, 1, 1, 1)
        [HDR] _GlowColor ("Glow Color", Color) = (0, 1, 1, 1)
        _CoreRadius ("Core Radius", Range(0.05, 10)) = 0.15
        _CoronaRadius ("Corona Radius", Range(0.1, 10)) = 0.5
        _PulseSpeed ("Pulse Speed", Float) = 2.0
        _PulseAmount ("Pulse Amount", Range(0, 0.2)) = 0.05
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "NeonSunPass"
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _CoreColor;
                float4 _GlowColor;
                float _CoreRadius;
                float _CoronaRadius;
                float _PulseSpeed;
                float _PulseAmount;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // Center UVs and calculate distance
                float2 uv = IN.uv - 0.5;
                float dist = length(uv);
                
                // Breathing animation
                float pulse = sin(_Time.y * _PulseSpeed) * _PulseAmount;
                float coreRadius = _CoreRadius;
    
                // 1. Create the Sharp Core
                // smoothstep helps with anti-aliasing the edge of the circle
                float coreMask = 1.0 - smoothstep(coreRadius - 0.005, coreRadius + 0.005, dist);
    
                // 2. Create the Corona/Glow
                // We calculate distance starting from the core's edge outward
                float glowDist = max(0, dist - coreRadius);
    
                // Exponential falloff: intensity = e^(-distance * falloff)
                // I'm using _CoronaRadius as a "thickness" control here
                float glowFalloff = 10.0 / _CoronaRadius; 
                float glowIntensity = exp(-glowDist * glowFalloff);
    
                // Add pulsing to the glow intensity
                glowIntensity *= (1.0 + pulse);

                // 3. Combine Colors
                // Core is CoreColor, Glow is GlowColor. 
                // We use the coreMask to "punch" the core color on top.
                float3 finalRGB = lerp(_GlowColor.rgb * glowIntensity, _CoreColor.rgb, coreMask);
    
                // Alpha logic: Core is solid, Glow fades out
                float finalAlpha = max(coreMask, glowIntensity);
    
                // Optional: Saturate alpha to keep it in 0-1 range
                return float4(finalRGB, saturate(finalAlpha));
            }
            ENDHLSL
        }
    }
}
