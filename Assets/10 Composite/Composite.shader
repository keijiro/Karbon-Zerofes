Shader "Hidden/Karbon/Composite"
{
    Properties
    {
        _BGPlane("BG Plane", 2D) = "black" {}
        _FGPlane1("FG Plane1", 2D) = "black" {}
        _FGPlane2("FG Plane2", 2D) = "black" {}
        _OverlayPlane("Overlay Plane", 2D) = "black" {}
        _BGColor("BG Color", Color) = (1, 1, 1, 1)
        _FGColor("FG Color", Color) = (1, 1, 1, 1)
    }

HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

TEXTURE2D(_BGPlane);
TEXTURE2D(_FGPlane1);
TEXTURE2D(_FGPlane2);
TEXTURE2D(_OverlayPlane);

float4 _BGColor;
float4 _FGColor;

float4 AlphaOver(float4 dst, float4 src)
{
    dst.rgb = lerp(dst.rgb, src.rgb, src.a);
    dst.a = src.a + dst.a * (1 - src.a);
    return dst;
}

float4 Frag(Varyings input) : SV_Target
{
    float2 uv = input.texcoord;

    float4 bg = SAMPLE_TEXTURE2D(_BGPlane, sampler_LinearClamp, uv);
    float4 fg1 = SAMPLE_TEXTURE2D(_FGPlane1, sampler_LinearClamp, uv);
    float4 fg2 = SAMPLE_TEXTURE2D(_FGPlane2, sampler_LinearClamp, uv);
    float4 ov = SAMPLE_TEXTURE2D(_OverlayPlane, sampler_LinearClamp, uv);

    float4 comp = 0;

    comp = AlphaOver(comp, float4(Luminance(bg.rgb) * _BGColor.rgb, bg.a));
    comp = AlphaOver(comp, float4(Luminance(fg1.rgb) * _FGColor.rgb, fg1.a));
    comp = AlphaOver(comp, float4(Luminance(fg2.rgb) * _FGColor.rgb, fg2.a));

    float3 screened = 1 - (1 - comp.rgb) * (1 - ov.rgb);
    comp.rgb = lerp(comp.rgb, screened, ov.a);
    comp.a = 1;

    return comp;
}

ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Name "Composite"
            ZTest Always ZWrite Off Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
