using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PostEffectRenderFuture : ScriptableRendererFeature
{
    [SerializeField] private Material blurMaterial_;
    [SerializeField] private Material passThroughMaterial_;
    private PostEffectRenderPass1 renderPass_;

    public override void Create()
    {
        renderPass_=new
            PostEffectRenderPass1(blurMaterial_,passThroughMaterial_);

        renderPass_.renderPassEvent =
            RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(
        ScriptableRenderer rendererPass,
        ref RenderingData renderingData)
    {
        
        if(rendererPass != null)
        {
            rendererPass.EnqueuePass(renderPass_);
        }
    }
}
