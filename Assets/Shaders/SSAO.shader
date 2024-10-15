Shader "Custom/SSAOShader"
{
    Properties
    {
        _InputTexture("Input Texture", 2D) = "white" {}
        _SSAOTexture("SSAO Texture", 2D) = "white" {}
        _R_OFFSET("R Offset", Float) = 0.0
        _SSAO_RADIUS("SSAO Radius", Float) = 0.003
        _SSAO_STRENGTH("SSAO Strength", Float) = 1.5
        _DisplayMode("Display Mode", Range(0, 2)) = 0
    }

    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 200
        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            TEXTURE2D(_InputTexture);
            SAMPLER(_InputSampler) = sampler_state
            {
                Texture = <_InputTexture>;
            };

            TEXTURE2D(_SSAOTexture);
            SAMPLER(_SSAOSampler) = sampler_state
            {
                Texture = <_SSAOTexture>;
                MinFilter = Linear;
                MagFilter = Linear;
                MipFilter = Linear;
                AddressU = Clamp;
                AddressV = Clamp;
            };
            half4 _MainTex_ST;
            float _R_OFFSET;
            float _SSAO_RADIUS;
            float _SSAO_STRENGTH;
            int _DisplayMode;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = TransformObjectToHClip(v.vertex);
                return o;
            }

            float4 DepthMapPixelShader(v2f i) : SV_Target
            {
                float4 InputColor = SAMPLE_TEXTURE2D(_InputTexture, _InputSampler, i.uv);
                if (InputColor.a == 0)
                {
                    return float4(0, 0, 0, 0);
                }
                else
                {
                    return float4(_R_OFFSET, i.uv.y, 0, InputColor.a);
                }
            }

            float4 SSAOPixelShader(v2f i) : SV_Target
            {
                float Occlusion = 1.0f;
                float4 D_Buffer = SAMPLE_TEXTURE2D(_SSAOTexture, _SSAOSampler, i.uv);
                
                if (D_Buffer.r == 0)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        float4 DD_Buffer = SAMPLE_TEXTURE2D(_InputTexture, _InputSampler, float2(i.uv.x, i.uv.y - (j * _SSAO_RADIUS)));
                        if (DD_Buffer.r > 0)
                        {
                            Occlusion -= (_SSAO_STRENGTH / (4.0f * (j + 1))) * (1 - DD_Buffer.b);
                        }
                    }
                }
                else
                {
                    for (int j = -2; j <= 2; j++)
                    {
                        for (int k = -2; k <= 2; k++)
                        {
                            float4 DD_Buffer = SAMPLE_TEXTURE2D(_InputTexture, _InputSampler, float2(i.uv.x + (j * _SSAO_RADIUS), i.uv.y + (k * _SSAO_RADIUS)));
                            float Dist = abs(j + k);
                            if (DD_Buffer.r != 0 && D_Buffer.r < DD_Buffer.r)
                            {
                                Occlusion -= ((_SSAO_STRENGTH) / 32.0f * (Dist / 1.5f)) * (0.5 - DD_Buffer.b);
                            }
                        }
                    }
                    for (int j = 1; j <= 4; j++)
                    {
                        float4 DD_Buffer = _InputTexture.Sample(_InputSampler, float2(i.uv.x, i.uv.y + (j * _SSAO_RADIUS)));
                        if (DD_Buffer.r == 0)
                        {
                            Occlusion -= (_SSAO_STRENGTH / (4.0f * (j + 1))) * (1 - D_Buffer.b);
                        }
                    }
                }
                return float4(Occlusion, Occlusion, Occlusion, 1);
            }

            float4 CompositePixelShader(v2f i) : SV_Target
            {
                float4 SSAO_Color = _SSAOTexture.Sample(_SSAOSampler, i.uv);
                float4 Scene_Color = _InputTexture.Sample(_InputSampler, i.uv);
                
                if (_DisplayMode == 0)
                    return SSAO_Color + (Scene_Color * 0.0000000001);
                else if (_DisplayMode == 1)
                    return SSAO_Color * Scene_Color;
                else
                    return float4(0, 0, 0, 0);
            }

            float4 frag(v2f i) : SV_Target
            {
                return CompositePixelShader(i);
            }

            ENDHLSL
        }
    }
}
