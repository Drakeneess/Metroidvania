Shader "Custom/AshesFineAlpha"
{
    Properties
    {
        _Color ("Ash Color", Color) = (0.9,0.7,0.5,1)   // tono de cenizas
        _Background ("Background Color", Color) = (0,0,0,1)
        _Speed ("Scroll Speed", Float) = 0.3
        _Scale ("Noise Scale", Float) = 15.0
        _Intensity ("Ash Intensity", Float) = 3.5
        _FlickerSpeed ("Flicker Speed", Float) = 2.0
        _Alpha ("Global Alpha", Range(0,1)) = 1.0        // 👈 Nuevo parámetro
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float4 _Background;
            float _Speed;
            float _Scale;
            float _Intensity;
            float _FlickerSpeed;
            float _Alpha; // 👈 Global alpha

            // ----------- Hash & Noise helpers -----------
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);

                float a = hash21(i);
                float b = hash21(i + float2(1.0, 0.0));
                float c = hash21(i + float2(0.0, 1.0));
                float d = hash21(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(a, b, u.x) +
                       (c - a) * u.y * (1.0 - u.x) +
                       (d - b) * u.x * u.y;
            }

            // ----------- Fractal noise (fbm) para más detalle -----------
            float fbm(float2 uv)
            {
                float v = 0.0;
                float a = 0.5;
                for (int i = 0; i < 4; i++) // más octavas = más fino
                {
                    v += a * noise(uv);
                    uv *= 2.0;
                    a *= 0.5;
                }
                return v;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _Scale;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Movimiento vertical
                float2 uv = i.uv + float2(0, _Time.y * _Speed);

                // Ruido fractal
                float n = fbm(uv);

                // Definir partículas brillantes (cenizas finas)
                float ash = pow(n, _Intensity);

                // Flicker para que no sea estático
                float flicker = 0.8 + 0.2 * sin(_Time.y * _FlickerSpeed + n * 10.0);

                // Mezcla final con alpha global
                fixed4 col = lerp(_Background, _Color, ash * flicker);
                col.a *= _Alpha; // 👈 Control del fade

                return col;
            }
            ENDCG
        }
    }
}
