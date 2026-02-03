Shader "Custom/Pixelate"
{
    Properties
    {
        _PixelSize ("Pixel Size", Float) = 4
    }
    
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        
        Pass
        {
            Name "Pixelate"
            ZWrite Off
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            float _PixelSize;
            
            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                
                // 获取屏幕尺寸
                float2 screenSize = _ScreenParams.xy;
                
                // 计算像素格子数量
                float2 pixelCount = screenSize / _PixelSize;
                
                // UV格子化：放大 → 取整 → 缩小
                uv = floor(uv * pixelCount) / pixelCount;
                
                // 用格子化后的UV采样
                return SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);
            }
            
            ENDHLSL
        }
    }
}