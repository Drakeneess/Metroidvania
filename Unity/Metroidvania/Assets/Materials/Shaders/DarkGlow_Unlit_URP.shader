Shader "SoS/Eye_AuraAbsorb"
{
    Properties
    {
        // OJO NEGRO + GLOW INTERNO SUAVE (opcional)
        _BaseColor     ("Base Color", Color) = (0,0,0,1)
        [HDR]_GlowColor("Glow Color (HDR)", Color) = (3.5,3.5,3.5,1)
        _GlowStrength  ("Glow Strength", Range(0,10)) = 2

        // AURA EXTERNA (CÁSCARA)
        _ShellThickness("Shell Thickness (m)", Range(0,0.2)) = 0.04
        _AuraOpacity   ("Aura Opacity", Range(0,1)) = 1
        _AuraIntensity ("Aura Intensity (HDR)", Range(0,10)) = 4

        // PARTÍCULAS (polvo espectral, pequeño)
        _Density       ("Particle Density", Range(4,64)) = 28
        _Size          ("Particle Size", Range(0.001,0.05)) = 0.015
        _Speed         ("Inward Speed", Range(0,3)) = 0.7
        _PulseSpeed    ("Pulse Speed", Range(0,6)) = 1.2
        _PulseAmount   ("Pulse Amount", Range(0,1)) = 0.25
        _Jitter        ("Jitter", Range(0,1)) = 0.35

        // CENTRO DE ABSORCIÓN
        _UseCustomCenter ("Use Custom Center (0=Object,1=Custom)", Float) = 0
        _CenterWS        ("Custom Center (World)", Vector) = (0,0,0,0)

        // ATENUACIÓN RADIAL DE AURA
        _ShellFadeInner ("Shell Fade Inner (0-1)", Range(0,1)) = 0.10
        _ShellFadeOuter ("Shell Fade Outer (0-1)", Range(0,1)) = 0.95

        // “Humedad” sutil (especular leve) sin plasticoso
        _SpecSmoothness ("Spec Smoothness", Range(0,1)) = 0.08
        _SpecAmount     ("Spec Amount", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }

        // =========================
        // PASS 1: Ojo negro + glow
        // =========================
        Pass
        {
            Name "EyeCore"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
            };

            float4 _BaseColor, _GlowColor;
            float _GlowStrength;
            float _SpecSmoothness, _SpecAmount;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float3 wp = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(wp);
                OUT.worldPos = wp;
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // negro base + glow central muy sutil (para no competir con el aura)
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(_WorldSpaceCameraPos - IN.worldPos);
                float fresCenter = pow(saturate(dot(N, V)), 2);
                float3 emission = _GlowColor.rgb * _GlowStrength * fresCenter;

                // “humedad” sutil para ojo realista sin plástico
                float3 H = normalize(V); // sin luces, especular fake con view
                float spec = pow(saturate(dot(N,H)), lerp(32, 256, _SpecSmoothness)) * _SpecAmount;

                float3 col = _BaseColor.rgb + emission + spec;
                return float4(col, 1);
            }
            ENDHLSL
        }

        // ==========================================
        // PASS 2: AURA EXTERNA (cáscara + partículas)
        // ==========================================
        Pass
        {
            Name "AuraShell"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            ZWrite Off               // no escribir profundidad para no apagar la escena
            ZTest LEqual
            Blend One One            // aditivo (glow/bloom)
            // Nota: si prefieres “humo” menos intenso: Blend SrcAlpha One

            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex vert_shell
            #pragma fragment frag_shell
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float  shellMask   : TEXCOORD2; // 0..1 grosor relativo
            };

            // Props
            float _ShellThickness;
            float _AuraOpacity, _AuraIntensity;

            float4 _GlowColor;

            float _Density, _Size, _Speed, _PulseSpeed, _PulseAmount, _Jitter;

            float _UseCustomCenter;
            float4 _CenterWS; // xyz

            float _ShellFadeInner, _ShellFadeOuter;

            // helpers
            float2 hash22(float2 p){
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.xx + p3.yz) * p3.zy);
            }

            // Partículas en espacio mundo: campo procedural sobre grilla
            float particleField(float3 wp, float density, float size, float jitter)
            {
                // proyectamos a un plano estable respecto a la vista para “polvo” etéreo
                float2 p = float2(wp.x, wp.y) * density;
                float2 i = floor(p);
                float2 f = frac(p);

                float2 rnd = hash22(i);
                float2 center = rnd * (1.0 - jitter) + 0.5 * jitter;

                float d = distance(f, center);
                float c = smoothstep(size, size*0.5, d);
                c = 1.0 - c;
                c = saturate(c);

                float spawn = step(0.7, rnd.x); // ~30% de celdas activas
                return c * spawn;
            }

            Varyings vert_shell (Attributes IN)
            {
                Varyings OUT;
                float3 n = normalize(TransformObjectToWorldNormal(IN.normalOS));

                // expandimos la malla a una cáscara fina hacia afuera (mundo)
                float3 wp_core  = TransformObjectToWorld(IN.positionOS.xyz);
                float3 wp_shell = wp_core + n * _ShellThickness;

                OUT.worldPos = wp_shell;
                OUT.positionHCS = TransformWorldToHClip(wp_shell);
                OUT.normalWS = n;
                OUT.shellMask = 1.0; // (punto para fade radial en frag)
                return OUT;
            }

            half4 frag_shell (Varyings IN) : SV_Target
            {
                float3 wp = IN.worldPos;
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(_WorldSpaceCameraPos - wp);
                float t = _Time.y;

                // centro de absorción (invisible)
                float3 center = (_UseCustomCenter > 0.5) ? _CenterWS.xyz : GetObjectToWorldMatrix()[3].xyz;

                // dirección hacia el centro (succión)
                float3 toC = center - wp;
                float dist = max(length(toC), 1e-4);
                float3 dirN = toC / dist;

                // advección (movimiento hacia el centro) en espacio mundo
                float3 advect = dirN * (_Speed * t);

                // pulso sutil
                float pulse = 1.0 + _PulseAmount * sin(t * _PulseSpeed);

                // dos capas de polvo para riqueza
                float f1 = particleField(wp + advect, _Density, _Size * pulse, _Jitter);
                float f2 = particleField(wp * 1.37 + advect * 1.7 + 7.9, _Density * 0.7, _Size * 0.7 * pulse, _Jitter*0.9);
                float particles = f1 * 0.7 + f2 * 0.5;

                // fade radial de la cáscara (más fuerte medio, muere muy adentro/afuera)
                // usamos distancia al centro para limitar el aura a un anillo agradable
                float shellR = saturate((dist / (_ShellThickness * 8.0))); // escala simple
                float shellFade = smoothstep(_ShellFadeOuter, _ShellFadeInner, shellR);

                // fresnel suave para que el aura viva alrededor del contorno
                float fres = pow(1.0 - saturate(dot(N, V)), 1.5);

                // mezcla final (blanco muerto HDR listo para Bloom)
                float aura = particles * fres * shellFade * _AuraOpacity;

                float3 emission = _GlowColor.rgb * _AuraIntensity * aura;

                return float4(emission, 1);
            }
            ENDHLSL
        }
    }
}
