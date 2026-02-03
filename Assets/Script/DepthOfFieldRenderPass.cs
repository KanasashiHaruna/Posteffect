using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class DepthOfFieldRenderPass : ScriptableRenderPass
{
    private Material depthMaterial_ = null;
    private Material blurMaterial_ = null;
    private Material dofMaterial_ = null;

    static readonly int depthTextureId =
        Shader.PropertyToID("_DepthTexture");
    static readonly int blurTextureId =
        Shader.PropertyToID("_BlurTexture");

    class DepthOfFieldPassData
    {
        public TextureHandle sourseTexture;
        public TextureHandle depthTexture;
        public TextureHandle blurTexture;
        public TextureHandle destination;
        public Material material;
    }

    private void DepthOfFieldBlit(RenderGraph renderGraph,TextureHandle cameraTexture,TextureHandle destinationTexture,
        TextureHandle depthTexture, TextureHandle blurTexture)
    {
        using(
            IRasterRenderGraphBuilder builder=
            renderGraph.AddRasterRenderPass(
                "DepthOfFieldBlit",out DepthOfFieldPassData passDara
                )
            )
        {
            passDara.sourseTexture = cameraTexture;
            passDara.depthTexture = depthTexture;
            passDara.blurTexture= blurTexture;
            passDara.destination = destinationTexture;
            passDara.material = dofMaterial_;

            builder.UseTexture(passDara.sourseTexture);
            builder.UseTexture(passDara.depthTexture);
            builder.UseTexture(passDara.blurTexture);

            builder.SetRenderAttachment(
                passDara.destination, 0
                );

            builder.SetRenderFunc((
                DepthOfFieldPassData data,
                RasterGraphContext ctx) =>
            {
                data.material.SetTexture(
                    depthTextureId, data.depthTexture);
                data.material.SetTexture(
                    blurTextureId, data.blurTexture);

                Blitter.BlitTexture(
                    ctx.cmd, data.sourseTexture,
                    new Vector4(1, 1, 0, 0), data.material, 0
                );
            });
        }
    }

    public DepthOfFieldRenderPass(
    Material depthMaterial,
    Material dofMaterial,
    Material blurMaterial
    )
    {
        depthMaterial_ = depthMaterial;
        blurMaterial_ = blurMaterial;
        dofMaterial_ = dofMaterial;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {

        if (depthMaterial_ == null || blurMaterial_ == null || dofMaterial_==null)
        {
            base.RecordRenderGraph(renderGraph, frameData);
            return;
        }

        UniversalResourceData resourceDate =
            frameData.Get<UniversalResourceData>();
        if (resourceDate.isActiveTargetBackBuffer)
        { return; }

        //-----------
        TextureHandle cameraTexture =
            resourceDate.activeColorTexture;

        TextureDesc depthTextureDesc=
            renderGraph.GetTextureDesc(cameraTexture);

        depthTextureDesc.name = "_DepthTexture";
        depthTextureDesc.depthBufferBits = 0;
        depthTextureDesc.format = UnityEngine.Experimental.Rendering.GraphicsFormat.R16_SFloat;

        TextureHandle depthTexture=
            renderGraph.CreateTexture(depthTextureDesc);

        TextureDesc blurTextureDesc =
            renderGraph.GetTextureDesc(cameraTexture);
        blurTextureDesc.name = "_BlurTexture";
        int div = 2;
        blurTextureDesc.width /= div;
        blurTextureDesc.height /= div;

        blurTextureDesc.depthBufferBits = 0;

        TextureHandle blurTexture=
            renderGraph.CreateTexture(blurTextureDesc);

        TextureDesc destinationTextureDesc=
            renderGraph.GetTextureDesc(cameraTexture);
        destinationTextureDesc.name = "_DestinationTexture";

        destinationTextureDesc.depthBufferBits= 0;
        TextureHandle destinationTexture=
            renderGraph.CreateTexture(destinationTextureDesc);

        RenderGraphUtils.BlitMaterialParameters
            depthTextureBlitDesc =
            new RenderGraphUtils.BlitMaterialParameters(
                cameraTexture,
                depthTexture,
                depthMaterial_,
                0
                );

        renderGraph.AddBlitPass( depthTextureBlitDesc,"DrawDepthBlit" );

        RenderGraphUtils.BlitMaterialParameters
            blurTextureBlitDesc =
            new RenderGraphUtils.BlitMaterialParameters(
                cameraTexture,
                blurTexture,
                blurMaterial_,
                0
                );

        renderGraph.AddBlitPass(
            blurTextureBlitDesc, "DrawBlurBlit"
            );

        DepthOfFieldBlit(
            renderGraph,
            cameraTexture,
            destinationTexture,
            depthTexture,
            blurTexture
            );

        renderGraph.AddCopyPass(
            destinationTexture, cameraTexture, "CopyDof");
    }
}

