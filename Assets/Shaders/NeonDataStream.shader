Shader "Custom/NeonChevronStream"
{
    Properties
    {
        [HDR] _BaseColor("Base Color", Color) = (0, 1, 1, 1)
        _FlowSpeed("Flow Speed", Float) = 3.0
        _DashDensity("Dash Density", Float) = 5.0
        _ArrowSharpness("Arrow Sharpness", Range(0, 1)) = 0.5
        _BeamWidth("Beam Width", Range(0, 1)) = 0.8
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

        Pass
        {
            Name "Unlit"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _FlowSpeed;
                float _DashDensity;
                float _BeamWidth;
                float _ArrowSharpness;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                // --- 1. CHEVRON MATH ---
                // Calculate how far we are from the center line (0.5)
                // "abs" creates symmetry for top and bottom
                float distFromCenter = abs(uv.y - 0.5);
                
                // Create a "Lag" based on distance. 
                // Edges will lag behind the center, creating a ">" shape.
                // Multiplied by Sharpness to control how pointy the arrow is.
                float skew = distFromCenter * _ArrowSharpness;

                // Apply skew to the dash position
                float dashPos = (uv.x * _DashDensity) - (_Time.y * _FlowSpeed) + skew;
                
                // Generate the repeating pattern
                float dashSignal = frac(dashPos);
                float dashMask = step(0.5, dashSignal); // 0 or 1

                // --- 2. TUBE FADE (The "Ribbon Fix") ---
                // We calculate a soft mask for the top and bottom edges
                // to make it look cylindrical instead of flat.
                
                // Calculate safe zone based on BeamWidth
                float edgeSize = (1.0 - _BeamWidth) * 0.5;
                
                // Smoothly fade the alpha from the edge inward
                float vMask = smoothstep(0.0, edgeSize, uv.y) * smoothstep(1.0, 1.0 - edgeSize, uv.y);

                // --- 3. COMBINE ---
                half4 col = _BaseColor;
                col.a *= dashMask * vMask;

                return col;
            }
            ENDHLSL
        }
    }
}