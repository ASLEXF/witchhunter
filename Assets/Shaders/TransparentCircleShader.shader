Shader "Unlit/TransparentCircleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Center ("Center", Vector) = (0,0,0,0)
        _Radius ("Radius", Float) = 1
        _Transparency ("Transparency", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        ZWrite Off

        LOD 200
        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            struct appdata_t
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            half4 _MainTex_ST;
            float4 _Center;
            float _Radius;
            float _Transparency;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.positionCS = TransformObjectToHClip(v.positionOS);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                float2 screenPos = i.positionCS.xy / i.positionCS.w * 0.5 + 0.5;
                float2 centerPos = _Center.xy * 0.5 + 0.5;

                float distance = length(screenPos - centerPos);

                if (distance < _Radius / 1.2)
                {
                    col.a = 0;
                }
                else if (distance < _Radius)
                {
                    col.a = lerp(_Transparency, 0, (6 * _Radius - 5 * distance) / _Radius);
                }

                return col;
            }
            ENDHLSL
        }
    }
}
