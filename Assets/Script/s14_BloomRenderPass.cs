using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class s14_BloomRenderPass : ScriptableRenderPass
{
    private Material blurMaterial_ = null;
    private Material luminanceExtractMaterial_ = null;
    private Material compositeTextureMaterial_ = null;

    static readonly int luminanceBlurTextureId = Shader.PropertyToID("_OtherTexture");

    class CompositePassDate
    {
        public TextureHandle sourceTexture;
        public TextureHandle otherTexture;
        public TextureHandle destination;
        public Material material;
    }

    public s14_BloomRenderPass(Material luminanceExtractMaterial,
                               Material blurMaterial,
                               Material compositeTextureMaterial
                               )
    {
        blurMaterial_ = blurMaterial;
        compositeTextureMaterial_ = compositeTextureMaterial;
        luminanceExtractMaterial_ = luminanceExtractMaterial;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (blurMaterial_ == null || compositeTextureMaterial_ == null || luminanceExtractMaterial_ == null)
        {
            base.RecordRenderGraph(renderGraph, frameData);
            return;
        }

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        if (resourceData.isActiveTargetBackBuffer)
        {
            base.RecordRenderGraph(renderGraph, frameData);
            return;
        }

        TextureHandle cameraTexture = resourceData.activeColorTexture;
        TextureDesc originalTextureDesc = renderGraph.GetTextureDesc(cameraTexture);
        originalTextureDesc.name = "_OriginalTexture";
        originalTextureDesc.depthBufferBits = 0;
        TextureHandle origTempTexture = renderGraph.CreateTexture(originalTextureDesc);

        //å≥ÇÃÇ‚Ç¬
        TextureDesc luminanceTextureDesc = originalTextureDesc;
        luminanceTextureDesc.name = "_SmallTempTexture";


        //èkè¨-------------------------
        int div = 2;
        luminanceTextureDesc.width /= div;
        luminanceTextureDesc.height /= div;
        luminanceTextureDesc.format = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;

        ////ìÒâÒñ⁄
        //TextureDesc luminanceTextureDesc02 = luminanceTextureDesc;
        //luminanceTextureDesc02.name = "_SmallTempTexture02";
        //luminanceTextureDesc02.width /= div;
        //luminanceTextureDesc02.height /= div;
        //luminanceTextureDesc02.format = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;

        ////éOâÒñ⁄
        //TextureDesc luminanceTextureDesc03 = luminanceTextureDesc02;
        //luminanceTextureDesc03.name = "_SmallTempTexture03";
        //luminanceTextureDesc03.width /= div;
        //luminanceTextureDesc03.height /= div;
        //luminanceTextureDesc03.format = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;


        TextureHandle luminanceTexture = renderGraph.CreateTexture(luminanceTextureDesc);
        TextureHandle[] luminanceBlurTexture =  new TextureHandle[3];

        for (int i = 0; i < luminanceBlurTexture.Length; i++)
        {
            luminanceBlurTexture[i] = renderGraph.CreateTexture(luminanceTextureDesc);

            luminanceTextureDesc.width /= div;
            luminanceTextureDesc.height /= div;
 
        }

        RenderGraphUtils.BlitMaterialParameters luminanceExtractBlitMaterialParameters =
            new RenderGraphUtils.BlitMaterialParameters(cameraTexture, luminanceTexture, luminanceExtractMaterial_, 0);
        renderGraph.AddBlitPass(luminanceExtractBlitMaterialParameters, "LuminanceExtractBlit");

        //---
        RenderGraphUtils.BlitMaterialParameters brightnessBlitMaterialParameters =
            new RenderGraphUtils.BlitMaterialParameters(luminanceTexture, luminanceBlurTexture[0], blurMaterial_, 0);
        renderGraph.AddBlitPass(brightnessBlitMaterialParameters, "BrightnessBlit");
        brightnessBlitMaterialParameters =
            new RenderGraphUtils.BlitMaterialParameters(luminanceBlurTexture[0], luminanceBlurTexture[1], blurMaterial_, 0);
        renderGraph.AddBlitPass(brightnessBlitMaterialParameters, "BrightnessBlit");
        brightnessBlitMaterialParameters =
            new RenderGraphUtils.BlitMaterialParameters(luminanceBlurTexture[1], luminanceBlurTexture[2], blurMaterial_, 0);
        renderGraph.AddBlitPass(brightnessBlitMaterialParameters, "BrightnessBlit");
        //------
        TextureHandle textureHandle0 = renderGraph.CreateTexture(originalTextureDesc);
        TextureHandle textureHandle1 = renderGraph.CreateTexture(originalTextureDesc);

        ComposeBlit(renderGraph, cameraTexture, luminanceBlurTexture[0], textureHandle0);
        ComposeBlit(renderGraph, textureHandle0, luminanceBlurTexture[1], textureHandle1);
        ComposeBlit(renderGraph, textureHandle1, luminanceBlurTexture[2], textureHandle0);

        renderGraph.AddCopyPass(textureHandle0, cameraTexture, "CopyBloom");
    }

    private void ComposeBlit(RenderGraph renderGraph, TextureHandle sourceTexture, TextureHandle otherTexture, TextureHandle destination)
    {
        using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass(
                    "BloomComposite", out CompositePassDate passDate
                    )
                )
        {
            passDate.sourceTexture = sourceTexture;
            passDate.otherTexture = otherTexture;
            passDate.destination = destination;
            passDate.material = compositeTextureMaterial_;

            builder.UseTexture(passDate.sourceTexture);
            builder.UseTexture(passDate.otherTexture);
            builder.SetRenderAttachment(passDate.destination, 0);

            builder.SetRenderFunc(
                (
                  CompositePassDate date,
                  RasterGraphContext ctx
                  ) =>
                {
                    date.material.SetTexture(
                        luminanceBlurTextureId,
                        date.otherTexture
                    );

                    Blitter.BlitTexture(
                        ctx.cmd,
                        date.sourceTexture,
                        new Vector4(1, 1, 0, 0),
                        date.material,
                        0
                    );
                });
        }
    }
}
