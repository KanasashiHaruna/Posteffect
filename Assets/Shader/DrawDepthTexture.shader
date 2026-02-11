Shader "Custom/DrawDepthTexture"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma editor_sync_compilation

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            half4 Frag(Varyings IN) : SV_Target
            {
                float rawSceneDepth=SampleSceneDepth(IN.texcoord);
                rawSceneDepth=LinearEyeDepth(rawSceneDepth,_ZBufferParams);

                return rawSceneDepth;
            }
            ENDHLSL
        }
    }
}
