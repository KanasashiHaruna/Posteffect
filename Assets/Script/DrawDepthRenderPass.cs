using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class DrawDepthRenderPass : ScriptableRenderPass
{
    private Material depthTextureMaterial_ = null;

    public DrawDepthRenderPass(
        Material Material)
    {
        depthTextureMaterial_ = Material;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceDate =
            frameData.Get<UniversalResourceData>();
        if (resourceDate.isActiveTargetBackBuffer)
        { return; }

        TextureHandle cameraTexture =
            resourceDate.activeColorTexture;

        TextureDesc depthTextureDesc =
            renderGraph.GetTextureDesc(cameraTexture);

        depthTextureDesc.name = "_DepthTexture";
        depthTextureDesc.depthBufferBits = 0;
        depthTextureDesc.format = UnityEngine.Experimental.Rendering.GraphicsFormat.R16_SFloat;

        TextureHandle depthTexture=renderGraph.CreateTexture(depthTextureDesc);

        RenderGraphUtils.BlitMaterialParameters
            depthTextureBlitDesc =
            new RenderGraphUtils.BlitMaterialParameters(
                cameraTexture,
                depthTexture,
                depthTextureMaterial_,
                0
                );

        renderGraph.AddBlitPass(
            depthTextureBlitDesc,
            "DrawDepthBlit"
            );

        renderGraph.AddCopyPass(
            depthTexture,
            cameraTexture,
            "CopyBlur"
            );
    }
}
