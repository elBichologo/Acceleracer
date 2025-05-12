Shader "Aceleracer/PlayerCarShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0, 0.8, 1, 1)
        _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
        _HighlightSpeed ("Highlight Speed", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float4 _BaseColor;
            float4 _HighlightColor;
            float _HighlightSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float highlight = abs(sin(_Time.y * _HighlightSpeed));
                float3 color = lerp(_BaseColor.rgb, _HighlightColor.rgb, highlight);
                return float4(color, 1.0);
            }

            ENDCG
        }
    }
}
