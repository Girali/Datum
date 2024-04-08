
Shader "Hidden/OccaSoftware/ScreenShadows"
{
    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        Cull Back
        Blend Off
        ZWrite Off
        ZTest Always
        
        Pass
        {
            Name "ScreenShadows"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.occasoftware.altos/ShaderLibrary/CloudShadows.hlsl"
            #include "Packages/com.occasoftware.altos/ShaderLibrary/Common.hlsl"

            static float _INV_INV = 1.0 / 1e20;

            float3 frag(Varyings IN) : SV_Target
            {
                float depth = SampleSceneDepth(IN.uv);
                float depth01 = Linear01Depth(depth, _ZBufferParams);
                
                float cloudShadowAttenuation = 1.0;
                if(depth01 < 1.0)
                {
                    float3 positionWS = ComputeWorldSpacePosition(IN.uv, depth, UNITY_MATRIX_I_VP);
                    cloudShadowAttenuation = GetCloudShadowAttenuation(positionWS);
                }
                
                return cloudShadowAttenuation;
            }
            ENDHLSL
        }
    }
}