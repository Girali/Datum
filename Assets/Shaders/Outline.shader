Shader "Unlit/Outline"
{
    Properties
    {
        _Power("Power", Range(0.0, 0.2)) = 0.01
        _BaseColor("Base Colour", Color) = (1.0, 0.0, 0.0, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }
        LOD 100

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float _Power;
            float4 _BaseColor;
        CBUFFER_END

        struct VertexInput
        {
            float4 position : POSITION;
            float4 normal : NORMAL;
        };

        struct VertexOutput
        {
            float4 position : SV_POSITION;
        };

        ENDHLSL

        Pass
        {
            Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            VertexOutput vert(VertexInput i)
            {
                half3x3 m = (half3x3)UNITY_MATRIX_M;
                half3 objectScale = half3(
                    length(half3(m[0][0], m[1][0], m[2][0])),
                    length(half3(m[0][1], m[1][1], m[2][1])),
                    length(half3(m[0][2], m[1][2], m[2][2]))
                );

                 VertexOutput o;

                 float camDist = 1;//distance(mul(UNITY_MATRIX_M , i.position), _WorldSpaceCameraPos);
                 i.position.xyz += (normalize(i.normal) * camDist * _Power) / objectScale;
                 o.position = TransformObjectToHClip(i.position);

                 return o;
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                return _BaseColor;
            }

            ENDHLSL
        }
    }
}
