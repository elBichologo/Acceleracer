Shader "Custom/ToonShader_ECGOutline"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _ToonThreshold ("Toon Threshold", Range(0,1)) = 0.5
        _Steps ("Light Steps", Range(1,5)) = 3
        _AmbientLight ("Ambient Light", Range(0,1)) = 0.3
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Base Outline Thickness", Float) = 0.05
        _WaveFrequency ("Wave Frequency", Float) = 5.0
        _WaveAmplitude ("Wave Amplitude", Float) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        // CONTORNO (ECG)
        Pass
        {
            Name "ECGOutline"
            Tags { "LightMode"="SRPDefaultUnlit" }

            Cull Front
            ZWrite On
            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float _OutlineThickness;
                float4 _OutlineColor;
                float _WaveFrequency;
                float _WaveAmplitude;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);

                float wave = sin(_Time.y * _WaveFrequency + worldPos.x * _WaveAmplitude);
                float thickness = _OutlineThickness + wave * 0.01;

                worldPos += worldNormal * thickness;
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }

        // CUERPO PRINCIPAL
        Pass
        {
            Name "ToonBody"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _ToonThreshold;
                float _Steps;
                float _AmbientLight;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float3 normal = normalize(IN.normalWS);
                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.positionWS);
                float3 lightColor = float3(0, 0, 0);

                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float NdotL = max(0, dot(normal, lightDir));
                float toonNdotL = floor(NdotL * _Steps) / _Steps;
                float3 mainContribution = toonNdotL * mainLight.color.rgb;
                lightColor += mainContribution;

                // Add additional lights
                int count = GetAdditionalLightsCount();
                for (int i = 0; i < count; i++)
                {
                    Light light = GetAdditionalLight(i, IN.positionWS);
                    float3 dir = normalize(light.direction);
                    float diff = max(0, dot(normal, dir));
                    float toonDiff = floor(diff * _Steps) / _Steps;
                    float3 contribution = toonDiff * light.color.rgb;
                    lightColor += contribution;
                }

                float3 finalColor = _Color.rgb * max(lightColor, _AmbientLight);
                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Diffuse"
}
