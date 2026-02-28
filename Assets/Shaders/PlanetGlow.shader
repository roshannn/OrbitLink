Shader "Custom/PlanetGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _RimColor("Rim Color", Color) = (0.2, 0.6, 1.0, 1.0)
        _RimPower("Rim Power", Float) = 3.0
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
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                float3 viewDirWS    : TEXCOORD2;
            };

            sampler2D _MainTex;
            
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _RimColor;
                float _RimPower;
                float4 _MainTex_ST;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.viewDirWS = normalize(GetCameraPositionWS() - positionWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, input.uv) * _Color;
                
                half3 normalWS = normalize(input.normalWS);
                half rimDot = 1.0 - saturate(dot(input.viewDirWS, normalWS));
                half3 rimEmission = _RimColor.rgb * pow(rimDot, _RimPower);
                
                half3 finalColor = texColor.rgb + rimEmission;
                return half4(finalColor, texColor.a);
            }
            ENDHLSL
        }
    }
}