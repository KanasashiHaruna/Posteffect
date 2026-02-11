using UnityEngine;
using UnityEngine.Rendering.Universal;

public class s14_BloomRenderFuture : ScriptableRendererFeature
{
    [SerializeField] private Material blurMaterial_;
    [SerializeField] private Material luminanceExtractMaterial_;
    [SerializeField] private Material compositeTextureMaterial_;
    private s14_BloomRenderPass renderPass_;

    public override void Create()
    {
        renderPass_ = new
            s14_BloomRenderPass(luminanceExtractMaterial_, blurMaterial_, compositeTextureMaterial_);

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
