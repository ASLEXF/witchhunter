using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelateFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Material material;
        [Range(1, 32)]
        public float pixelSize = 4;
    }

    public Settings settings = new Settings();
    private PixelatePass pixelatePass;

    public override void Create()
    {
        pixelatePass = new PixelatePass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null) return;

        // 跳过Scene视图的预览相机等
        if (renderingData.cameraData.cameraType != CameraType.Game) return;

        renderer.EnqueuePass(pixelatePass);
    }

    protected override void Dispose(bool disposing)
    {
        pixelatePass?.Dispose();
    }
}

public class PixelatePass : ScriptableRenderPass
{
    private PixelateFeature.Settings settings;
    private RTHandle tempTexture;
    private const string PassName = "Pixelate";

    public PixelatePass(PixelateFeature.Settings settings)
    {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        var descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;
        RenderingUtils.ReAllocateIfNeeded(ref tempTexture, descriptor, name: "_TempPixelateTexture");

        // 配置目标和清除
        ConfigureTarget(renderingData.cameraData.renderer.cameraColorTargetHandle);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (settings.material == null) return;

        CommandBuffer cmd = CommandBufferPool.Get(PassName);

        RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

        settings.material.SetFloat("_PixelSize", settings.pixelSize);

        // 使用 Blitter API（Unity 2022+ 推荐方式）
        Blitter.BlitCameraTexture(cmd, source, tempTexture, settings.material, 0);
        Blitter.BlitCameraTexture(cmd, tempTexture, source);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
    }

    public void Dispose()
    {
        tempTexture?.Release();
    }
}