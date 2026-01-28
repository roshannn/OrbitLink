Shader "Custom/NeonPlanet"
{
    Properties
    {
        [HDR]_MainColor ("Main Color (HDR)", Color) = (0, 1, 1, 1) // Cyan
        _Radius ("Radius", Float) = 0.45
        _RingThickness ("Ring Thickness", Float) = 0.05
        _FillOpacity ("Fill Opacity", Float) = 0.1
        _Softness ("Softness (AA)", Float) = 0.01
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
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

            CBUFFER_START(UnityPerMaterial)
                half4 _MainColor;
                float _Radius;
                float _RingThickness;
                float _FillOpacity;
                float _Softness;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

           half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv - 0.5;
                float dist = length(uv);
                
                // 1. Ring Logic
                float ringRadius = _Radius;
                float ringInnerRadius = ringRadius - _RingThickness;
                
                float outerEdge = 1.0 - smoothstep(ringRadius - _Softness, ringRadius, dist);
                float innerEdge = smoothstep(ringInnerRadius - _Softness * 2.0, ringInnerRadius, dist);
                float ringMask = outerEdge * innerEdge;
    
                // 2. Fill Logic
                float fillMask = (1.0 - smoothstep(ringInnerRadius - _Softness, ringInnerRadius, dist)) * _FillOpacity;
    
                // 3. Smooth Color Transition
                // Instead of 'if', we lerp the color. 
                // This makes the ring glow 'bleed' into the fill area naturally.
                half3 ringColor = _MainColor.rgb;
                half3 fillColor = _MainColor.rgb * 0.5; // Dimmed interior
    
                // Use ringMask as the driver for color brightness
                half3 finalColor = lerp(fillColor, ringColor, ringMask);
    
                // 4. Alpha Composition
                float finalAlpha = max(ringMask, fillMask);
    
                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
}
