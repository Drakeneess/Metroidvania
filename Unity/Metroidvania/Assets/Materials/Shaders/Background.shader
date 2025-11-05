Shader "ShadowOfSouls/SoulBreathBackground_Base"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.04, 0.04, 0.055, 1)
        _TintColor("Tint Color", Color) = (0.15, 0.10, 0.15, 1)

        _NoiseScale("Noise Scale", Float) = 1.8
        _DistortStrength("Distortion Strength", Range(0,1)) = 0.12
        _Speed("Movement Speed", Float) = 0.10

        _VoronoiTex("Voronoi Texture", 2D) = "white" {}
        _UseFallback("Use Fallback If Missing", Float) = 1

        _GlobalAlpha("Global Alpha", Range(0,1)) = 1    // <-- NEW
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Background" }
        Pass
        {
            Name "SoulBreathBase"
            Blend One OneMinusSrcAlpha   // <-- Required for alpha fade

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _BaseColor;
            float4 _TintColor;
            float _NoiseScale;
            float _DistortStrength;
            float _Speed;

            TEXTURE2D(_VoronoiTex); SAMPLER(sampler_VoronoiTex);
            float _UseFallback;

            float _GlobalAlpha;    // <-- NEW

            // Simple procedural fallback noise (very soft)
            float hash(float2 p)
            { return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453); }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash(i);
                float b = hash(i + float2(1,0));
                float c = hash(i + float2(0,1));
                float d = hash(i + float2(1,1));
                float2 u = f*f*(3.0-2.0*f);
                return lerp(lerp(a,b,u.x), lerp(c,d,u.x), u.y);
            }

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

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                float t = _Time.y * _Speed;

                float2 dUV = uv * _NoiseScale;
                float distortion = (noise(dUV + t) - 0.5) * _DistortStrength;
                float2 uvDistorted = uv + distortion;

                float3 col = _BaseColor.rgb;

                if (_UseFallback > 0.5)
                {
                    float3 vCol = SAMPLE_TEXTURE2D(_VoronoiTex, sampler_VoronoiTex, uvDistorted).rgb;
                    col += vCol * _TintColor.rgb;
                }
                else
                {
                    float n = noise(uvDistorted * 3.0 + t);
                    col += (n - 0.5) * 0.15 + _TintColor.rgb * 0.5;
                }

                return float4(col, _GlobalAlpha);   // <-- NEW
            }
            ENDHLSL
        }
    }
}
