Shader "ShadowOfSouls/SoulBreathBackground_Base"
{
    Properties
    {
        [Header(Core Colors)]
        _BaseColor("Base Color", Color) = (0.035, 0.035, 0.05, 1)
        _TintColor("Mist Tint", Color) = (0.16, 0.08, 0.18, 1)
        _GlowColor("Soul Glow Color", Color) = (0.45, 0.05, 0.12, 1)

        [Header(Noise)]
        _NoiseScale("Noise Scale", Float) = 1.8
        _MistScale("Mist Scale", Float) = 3.5
        _DetailScale("Detail Scale", Float) = 12.0
        _DistortStrength("Distortion Strength", Range(0,1)) = 0.10
        _Speed("Movement Speed", Float) = 0.08

        [Header(Breathing)]
        _BreathStrength("Breath Strength", Range(0,1)) = 0.18
        _BreathSpeed("Breath Speed", Float) = 0.45

        [Header(Vignette)]
        _VignetteStrength("Vignette Strength", Range(0,2)) = 0.85
        _VignetteSoftness("Vignette Softness", Range(0.1,4)) = 1.65

        [Header(Center Glow)]
        _CenterGlowStrength("Center Glow Strength", Range(0,2)) = 0.35
        _CenterGlowRadius("Center Glow Radius", Range(0.1,2)) = 0.75

        [Header(Soul Veins)]
        _VeinStrength("Soul Vein Strength", Range(0,1)) = 0.16
        _VeinSharpness("Soul Vein Sharpness", Range(1,12)) = 5.5

        [Header(Texture Optional)]
        _VoronoiTex("Voronoi Texture", 2D) = "white" {}
        _UseTexture("Use Voronoi Texture", Float) = 0

        [Header(Output)]
        _GlobalAlpha("Global Alpha", Range(0,1)) = 1
        _GrainStrength("Grain Strength", Range(0,0.15)) = 0.035
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Background"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "SoulBreathEnhanced"

            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _BaseColor;
            float4 _TintColor;
            float4 _GlowColor;

            float _NoiseScale;
            float _MistScale;
            float _DetailScale;
            float _DistortStrength;
            float _Speed;

            float _BreathStrength;
            float _BreathSpeed;

            float _VignetteStrength;
            float _VignetteSoftness;

            float _CenterGlowStrength;
            float _CenterGlowRadius;

            float _VeinStrength;
            float _VeinSharpness;

            TEXTURE2D(_VoronoiTex);
            SAMPLER(sampler_VoronoiTex);
            float _UseTexture;

            float _GlobalAlpha;
            float _GrainStrength;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;

                [unroll]
                for (int i = 0; i < 4; i++)
                {
                    value += noise(p) * amplitude;
                    p *= 2.03;
                    amplitude *= 0.5;
                }

                return value;
            }

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.positionHCS);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y;

                float2 centeredUV = uv - 0.5;
                float distanceFromCenter = length(centeredUV);

                // Breathing pulse
                float breath = sin(time * _BreathSpeed) * 0.5 + 0.5;
                float breathIntensity = 1.0 + breath * _BreathStrength;

                // Organic distortion
                float flowTime = time * _Speed;

                float n1 = fbm(uv * _NoiseScale + float2(flowTime, -flowTime * 0.65));
                float n2 = fbm(uv * _NoiseScale * 1.7 + float2(-flowTime * 0.45, flowTime));

                float2 distortion;
                distortion.x = (n1 - 0.5) * _DistortStrength;
                distortion.y = (n2 - 0.5) * _DistortStrength;

                float2 warpedUV = uv + distortion;

                // Main mist
                float mist = fbm(warpedUV * _MistScale + float2(flowTime * 0.35, flowTime * 0.12));
                float detailMist = fbm(warpedUV * _DetailScale - float2(flowTime * 0.2, flowTime * 0.4));

                // Optional texture contribution
                float texPattern = 0.0;

                if (_UseTexture > 0.5)
                {
                    texPattern = SAMPLE_TEXTURE2D(_VoronoiTex, sampler_VoronoiTex, warpedUV).r;
                }
                else
                {
                    texPattern = fbm(warpedUV * 5.0 + flowTime);
                }

                // Soul veins: thin organic highlights
                float veinRaw = abs(texPattern - 0.5) * 2.0;
                float veins = pow(1.0 - saturate(veinRaw), _VeinSharpness);
                veins *= _VeinStrength;

                // Center glow
                float centerGlow = 1.0 - smoothstep(0.0, _CenterGlowRadius, distanceFromCenter);
                centerGlow *= _CenterGlowStrength * breathIntensity;

                // Vignette
                float vignette = smoothstep(0.15, _VignetteSoftness, distanceFromCenter);
                vignette *= _VignetteStrength;

                // Grain
                float grain = hash(uv * _ScreenParams.xy + time * 37.0);
                grain = (grain - 0.5) * _GrainStrength;

                float3 color = _BaseColor.rgb;

                // Mist coloration
                color += _TintColor.rgb * mist * 0.55;
                color += _TintColor.rgb * detailMist * 0.16;

                // Vein/glow coloration
                color += _GlowColor.rgb * veins;
                color += _GlowColor.rgb * centerGlow;

                // Breathing darkness shift
                color *= lerp(0.92, 1.08, breath * _BreathStrength);

                // Apply vignette
                color = lerp(color, color * 0.35, vignette);

                // Grain last
                color += grain;

                color = saturate(color);

                return half4(color, _GlobalAlpha);
            }

            ENDHLSL
        }
    }
}