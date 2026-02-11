using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ToneMappingRenderFuture : ScriptableRendererFeature
{
    [SerializeField] private Material postEffectMaterial_;
    private ToneMappingRenderPass renderPass_;

    public override void Create()
    {
        renderPass_ = new
            ToneMappingRenderPass(postEffectMaterial_);

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