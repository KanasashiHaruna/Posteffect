using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class GreenPostEffectRenderPass : ScriptableRenderPass
{
    private Material material_ = null;

    public GreenPostEffectRenderPass(
        Material postEffectMaterial
    )
    { 
       material_ = postEffectMaterial;
       
    }

    public override void RecordRenderGraph(
        RenderGraph renderGraph, ContextContainer frameData)
    {
        if (material_==null)
        {
            base.RecordRenderGraph(renderGraph, frameData);
            return;
        }

        UniversalResourceData resourceDate =
            frameData.Get<UniversalResourceData>();
        if(resourceDate.isActiveTargetBackBuffer)
        { return; }

        //-----------
        TextureHandle cameraTexture =
            resourceDate.activeColorTexture;

        TextureDesc tempDesc=
            renderGraph.GetTextureDesc(cameraTexture);
        tempDesc.name = "_GreenTexture";
        tempDesc.depthBufferBits = 0;

        TextureHandle TempTexture=
            renderGraph.CreateTexture(tempDesc);

        
        //-----------

        RenderGraphUtils.BlitMaterialParameters
            blitMaterialParameters =
            new RenderGraphUtils.BlitMaterialParameters(
                cameraTexture, TempTexture, material_, 0);
        renderGraph.AddBlitPass(blitMaterialParameters,
            "BlitGreenPostEffect");
       
        renderGraph.AddCopyPass(TempTexture, cameraTexture, "CopyBlur");
    }
}
