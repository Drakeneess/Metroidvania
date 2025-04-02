Shader "Custom/ToonShader_DynamicLight"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.1,0.1,0.1,1)
        _RampThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularThreshold ("Specular Threshold", Range(0,1)) = 0.9
        _Shininess ("Shininess", Range(0.1, 10)) = 1
        _LightPos ("Point Light Position", Vector) = (0,5,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            float4 _Color;
            float4 _ShadowColor;
            float _RampThreshold;
            float4 _SpecularColor;
            float _SpecularThreshold;
            float _Shininess;
            float4 _LightPos; // Para luz puntual

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 lightDir;
                
                if (_LightPos.w == 0) // Si es una luz direccional
                {
                    lightDir = normalize(_WorldSpaceLightPos0.xyz);
                }
                else // Si es una luz puntual
                {
                    lightDir = normalize(_LightPos.xyz - i.worldPos);
                }

                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 halfDir = normalize(lightDir + viewDir);

                // Toon Shadow adaptado a la luz
                float NdotL = dot(i.worldNormal, lightDir);
                float shadowStep = smoothstep(_RampThreshold - 0.05, _RampThreshold + 0.05, NdotL);
                float4 baseColor = lerp(_ShadowColor, _Color, shadowStep);

                // Toon Specular
                float NdotH = dot(i.worldNormal, halfDir);
                float specStep = smoothstep(_SpecularThreshold - 0.05, _SpecularThreshold + 0.05, pow(NdotH, _Shininess * 100));
                float4 specular = specStep * _SpecularColor;

                return baseColor + specular;
            }
            ENDCG
        }
    }
}
