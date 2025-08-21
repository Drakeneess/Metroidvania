Shader "Custom/DarkCelWithLineArt"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.1,0.1,0.1,1)
        _LightPos ("Light Position", Vector) = (0,2.18000007,-0.769999981,0)
        _LightIntensity ("Light Intensity", Range(0,5)) = 1
        _LineThickness ("Line Thickness", Range(0,0.05)) = 0.04
        _LineColor ("Line Color", Color) = (0,0,0,1)
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _SpecularIntensity ("Specular Intensity", Range(0,5)) = 1
        _SpecularSize ("Specular Size", Range(0,1)) = 0.1
        _AmbientIntensity ("Ambient Intensity", Range(0,1)) = 0.1
        _OutlineZOffset ("Outline Z Offset", Range(0,0.1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass // Outline
        {
            Name "Outline"
            Cull Front
            ZWrite On
            ZTest LEqual
            Offset [_OutlineZOffset], [_OutlineZOffset]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _LineThickness;
            float4 _LineColor;

            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                v.vertex.xyz += norm * _LineThickness;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return _LineColor;
            }
            ENDHLSL
        }

        Pass // Cel shading
        {
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            float4 _MainColor;
            float4 _ShadowColor;
            float3 _LightPos;
            float _LightIntensity;
            float _ShadowThreshold;
            float _SpecularIntensity;
            float _SpecularSize;
            float _AmbientIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 N = normalize(i.worldNormal);
                float3 L = normalize(_LightPos - i.worldPos);
                float NdotL = dot(N, L) * 0.5 + 0.5; // half-Lambert
                float lightStep = smoothstep(_ShadowThreshold - 0.05, _ShadowThreshold + 0.05, NdotL);


                // Specular
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 H = normalize(L + V);
                float spec = pow(saturate(dot(N, H)), 1.0 / _SpecularSize) * _SpecularIntensity;

                float3 finalColor = lerp(_ShadowColor.rgb, _MainColor.rgb, lightStep);
                finalColor += spec;
                finalColor += _AmbientIntensity * _MainColor.rgb;
                finalColor *= _LightIntensity;

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}
