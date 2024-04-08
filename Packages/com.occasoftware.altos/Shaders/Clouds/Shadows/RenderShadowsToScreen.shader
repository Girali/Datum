Shader"OccaSoftware/Altos/RenderShadowsToScreen"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        
        ZWrite Off
        Cull Off
        ZTest Always
        
        Pass
        {
            Name "Render Shadows to Screen"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.occasoftware.altos/ShaderLibrary/Common.hlsl"
            #include "Packages/com.occasoftware.altos/ShaderLibrary/TextureUtils.hlsl"

            Texture2D _ScreenTexture;
            Texture2D _CloudScreenShadows;

            float3 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float3 screenColor = _ScreenTexture.SampleLevel(point_clamp_sampler, input.uv, 0).rgb;
                float3 shadowSample = _CloudScreenShadows.SampleLevel(linear_clamp_sampler, input.uv, 0).rgb;
                return screenColor * shadowSample;
            }
            ENDHLSL
        }
    }
}