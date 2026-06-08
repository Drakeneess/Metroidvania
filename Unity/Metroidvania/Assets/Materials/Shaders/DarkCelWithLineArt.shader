Shader "ShadowOfSouls/DarkLowpolyCelLineArt"
{
    Properties
    {
        [Header(Base)]
        _MainColor ("Main Color", Color) = (0.42, 0.40, 0.38, 1)
        _ShadowColor ("Shadow Color", Color) = (0.12, 0.11, 0.12, 1)
        _DeepShadowColor ("Deep Shadow Color", Color) = (0.035, 0.032, 0.04, 1)

        [Header(Lighting)]
        _LightPos ("Light Position", Vector) = (0, 2.18, -0.77, 0)
        _LightIntensity ("Light Intensity", Range(0,2)) = 0.85
        _AmbientIntensity ("Ambient Intensity", Range(0,1)) = 0.08

        [Header(Cel Bands)]
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.48
        _ShadowHardness ("Shadow Hardness", Range(0.001,0.2)) = 0.025
        _DeepShadowThreshold ("Deep Shadow Threshold", Range(0,1)) = 0.22
        _DeepShadowHardness ("Deep Shadow Hardness", Range(0.001,0.2)) = 0.025

        [Header(Subtle Edge Light)]
        _EdgeColor ("Edge Color", Color) = (0.16, 0.03, 0.045, 1)
        _EdgeIntensity ("Edge Intensity", Range(0,1)) = 0.18
        _EdgePower ("Edge Power", Range(1,8)) = 4.5
        _EdgeThreshold ("Edge Threshold", Range(0,1)) = 0.72

        [Header(Line Art)]
        _LineThickness ("Line Thickness", Range(0,0.06)) = 0.018
        _LineColor ("Line Color", Color) = (0.005, 0.004, 0.006, 1)

        [Header(Art Direction)]
        _Desaturation ("Desaturation", Range(0,1)) = 0.35
        _Darkness ("Darkness", Range(0,1)) = 0.18
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }

            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex OutlineVert
            #pragma fragment OutlineFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _LineThickness;
            float4 _LineColor;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings OutlineVert(Attributes input)
            {
                Varyings output;

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));

                positionWS += normalWS * _LineThickness;

                output.positionHCS = TransformWorldToHClip(positionWS);
                return output;
            }

            half4 OutlineFrag(Varyings input) : SV_Target
            {
                return half4(_LineColor.rgb, 1);
            }

            ENDHLSL
        }

        Pass
        {
            Name "DarkLowpolyCel"
            Tags { "LightMode"="UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex CelVert
            #pragma fragment CelFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _MainColor;
            float4 _ShadowColor;
            float4 _DeepShadowColor;

            float3 _LightPos;
            float _LightIntensity;
            float _AmbientIntensity;

            float _ShadowThreshold;
            float _ShadowHardness;
            float _DeepShadowThreshold;
            float _DeepShadowHardness;

            float4 _EdgeColor;
            float _EdgeIntensity;
            float _EdgePower;
            float _EdgeThreshold;

            float _Desaturation;
            float _Darkness;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            Varyings CelVert(Attributes input)
            {
                Varyings output;

                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));

                return output;
            }

            float3 Desaturate(float3 color, float amount)
            {
                float gray = dot(color, float3(0.299, 0.587, 0.114));
                return lerp(color, float3(gray, gray, gray), amount);
            }

            half4 CelFrag(Varyings input) : SV_Target
            {
                float3 N = normalize(input.normalWS);
                float3 V = normalize(_WorldSpaceCameraPos.xyz - input.positionWS);
                float3 L = normalize(_LightPos - input.positionWS);

                float ndotl = dot(N, L) * 0.5 + 0.5;

                float lightBand = smoothstep(
                    _ShadowThreshold - _ShadowHardness,
                    _ShadowThreshold + _ShadowHardness,
                    ndotl
                );

                float deepBand = smoothstep(
                    _DeepShadowThreshold - _DeepShadowHardness,
                    _DeepShadowThreshold + _DeepShadowHardness,
                    ndotl
                );

                float3 shadowBase = lerp(_DeepShadowColor.rgb, _ShadowColor.rgb, deepBand);
                float3 color = lerp(shadowBase, _MainColor.rgb, lightBand);

                // Ambient very controlled, because this is not a Pixar funeral.
                color += _MainColor.rgb * _AmbientIntensity;

                // Very subtle edge light, mostly for silhouette separation.
                float rim = pow(1.0 - saturate(dot(N, V)), _EdgePower);
                rim = step(_EdgeThreshold, rim);
                color += _EdgeColor.rgb * rim * _EdgeIntensity;

                color *= _LightIntensity;

                // Art direction: muted and depressive.
                color = Desaturate(color, _Desaturation);
                color *= 1.0 - _Darkness;

                return half4(saturate(color), 1);
            }

            ENDHLSL
        }
    }

    FallBack Off
}