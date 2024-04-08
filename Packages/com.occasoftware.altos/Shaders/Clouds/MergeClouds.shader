Shader"OccaSoftware/Altos/MergeClouds"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        
        ZWrite Off
        Cull Off
        ZTest Always
        
        Pass
        {
            Name "Merge Clouds"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.occasoftware.altos/ShaderLibrary/Common.hlsl"

            Texture2D _ScreenTexture;
            int _CLOUD_DEPTH_CULL_ON;
            Texture2D _MERGE_PASS_INPUT_TEX;

            SamplerState point_clamp_sampler;

            float3 Fragment(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float3 screenColor = _ScreenTexture.SampleLevel(point_clamp_sampler, input.uv, 0).rgb;
                float4 cloudColor = _MERGE_PASS_INPUT_TEX.SampleLevel(point_clamp_sampler, input.uv, 0);
                float3 outputColor = screenColor.rgb * cloudColor.a + cloudColor.rgb;
                float3 finalColor = outputColor;
                float rawDepth = SampleSceneDepth(input.uv);
                float depth01 = Linear01Depth(rawDepth, _ZBufferParams);
    
                if (depth01 >= 1.0)
                {
                    return outputColor;
                }
                else
                {
                    if (_CLOUD_DEPTH_CULL_ON == 1)
                    {
                        return outputColor;
                    }
                    else
                    {
                        return screenColor;
                    }
                }
            }
            ENDHLSL
        }
    }
}