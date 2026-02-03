using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DrawDepthRenderFuture : ScriptableRendererFeature
{
    [SerializeField] private Material DrawDepthMaterial_;
    private DrawDepthRenderPass renderPass_;
    public override void Create()
    {
        renderPass_ = new
            DrawDepthRenderPass(DrawDepthMaterial_);

        renderPass_.renderPassEvent =
            RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(
        ScriptableRenderer rendererPass,
        ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera)
        {
            return;
        }
        if (rendererPass != null)
        {
            rendererPass.EnqueuePass(renderPass_);
        }
    }

}
