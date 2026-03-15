Shader "Karbon/Soft Quad"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1, 1, 1, 1)
        _EdgeFade("Edge Fade", Range(0, 0.5)) = 0.05
        _FadePower("Fade Power", Range(0.1, 8)) = 1
    }

HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);
float4 _BaseMap_ST;
float4 _BaseColor;
float _EdgeFade;
float _FadePower;

void Vert(float4 positionOS : POSITION,
          float2 texCoord : TEXCOORD0,
          out float4 positionCS : SV_Position,
          out float2 uv : TEXCOORD0,
          out float2 fadeCoord : TEXCOORD1)
{
    positionCS = TransformObjectToHClip(positionOS.xyz);
    uv = TRANSFORM_TEX(texCoord, _BaseMap);
    fadeCoord = texCoord;
}

half EdgeMask(float2 uv)
{
    float fade = max(_EdgeFade, 1e-5);
    float2 dist = min(uv, 1 - uv);
    float mask = saturate(min(dist.x, dist.y) / fade);
    return pow(mask, _FadePower);
}

half4 Frag(float2 uv : TEXCOORD0,
           float2 fadeCoord : TEXCOORD1) : SV_Target
{
    half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv) * _BaseColor;
    color.rgb *= EdgeMask(fadeCoord);
    return color;
}

ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" "RenderType"="Opaque" }
        Pass
        {
            Cull Off
            ZWrite On
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
