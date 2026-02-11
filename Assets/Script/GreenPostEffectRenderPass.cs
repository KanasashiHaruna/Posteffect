using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GreenPostEffectRenderFuture : ScriptableRendererFeature
{
    [SerializeField] private Material postEffectMaterial_;
    private GreenPostEffectRenderPass greenPass_;

    public override void Create()
    {
        greenPass_ = new
            GreenPostEffectRenderPass(postEffectMaterial_);

        greenPass_.renderPassEvent =
            RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(
        ScriptableRenderer rendererPass,
        ref RenderingData renderingData)
    {
        
        if(rendererPass != null)
        {
            rendererPass.EnqueuePass(greenPass_);
        }
    }
}
