Shader "Custom/14_01TextureComposite"
{
    Properties
    {
        _OtherTexture("OtherTexture",2D)="black"{}
    }

    SubShader
    {
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma editor_sync_compilation

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D(_OtherTexture);
            SAMPLER(sampler_OtherTexture);

            half4 Frag(Varyings IN) : SV_Target
            {
                half4 blitColor=SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearClamp,IN.texcoord);

                half4 otherColor=SAMPLE_TEXTURE2D(_OtherTexture,sampler_LinearClamp,IN.texcoord);
                half4 output=saturate(blitColor+otherColor);
                return output;
            }
            
            ENDHLSL
        }
    }
}
