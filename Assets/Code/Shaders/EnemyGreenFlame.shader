Shader "Aceleracer/GreenFlameAura"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _FlameColor ("Flame Color", Color) = (0, 1, 0.5, 1)
        _EmissionStrength ("Emission Strength", Float) = 2.5
        _NoiseScale ("Noise Scale", Float) = 5
        _Speed ("Scroll Speed", Float) = 1.5
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.2
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 300
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _FlameColor;
            float _EmissionStrength;
            float _NoiseScale;
            float _Speed;
            float _AlphaCutoff;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 noiseUV = i.uv * _NoiseScale + float2(_Time.y * _Speed, 0);
                float noise = tex2D(_NoiseTex, noiseUV).r;

                float alpha = saturate((noise - _AlphaCutoff) * 5);
                float3 color = _FlameColor.rgb * _EmissionStrength * alpha;

                return float4(color, alpha);
            }
            ENDCG
        }
    }
}
