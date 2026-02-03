using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DepthOfFieldRenderFuture : ScriptableRendererFeature
{
    [SerializeField] private Material depthTextureMaterial_;
    [SerializeField] private Material depthOfFieldTextureMaterial;
    [SerializeField] private Material blurTextureMaterial;
    private DepthOfFieldRenderPass renderPass_;

    public override void Create()
    {
        renderPass_ = new
            DepthOfFieldRenderPass(depthTextureMaterial_, depthOfFieldTextureMaterial, blurTextureMaterial);

        renderPass_.renderPassEvent =
            RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(
        ScriptableRenderer rendererPass,
        ref RenderingData renderingData)
    {

        if (rendererPass != null)
        {
            rendererPass.EnqueuePass(renderPass_);
        }
    }
}
