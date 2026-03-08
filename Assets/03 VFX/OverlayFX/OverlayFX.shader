Shader "Hidden/Karbon/OverlayFX"
{
HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise3D.hlsl"

half3 _FxParam;
uint _FxSeed;

half4 ApplyOverlay(float2 uv, bool mask)
{
    return mask ? 1 : SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
}

bool SlitsMask(float2 uv)
{
    half n = 0;
    float t = _Time.y * 1.8;

    n += SimplexNoise(float2(uv.x * 6, t));
    n += SimplexNoise(float2(uv.x * 20, t));

    return abs(n) < _FxParam.x;
}

bool BlotsMask(float2 uv)
{
    float2 asp = float2(960.0 / 256, 1);
    half n = abs(SimplexNoise(float3(uv * asp * 1.1, _Time.y * 0.25)));

    return n > 1 - _FxParam.y;
}

half3 Hash3(uint seed)
{
    return half3(Hash(seed), Hash(seed + 1), Hash(seed + 2));
}

bool WiperMask(float2 uv)
{
    uint sw = JenkinsHash(_FxSeed);
    if (sw & 1) uv = 1 - uv;
    if (sw & 2) uv = uv.yx;

    half t = frac(_FxParam.z);
    half3 t_start = Hash3(_FxSeed + 1) / 2;
    half3 t_end   = Hash3(_FxSeed + 4) / 2 + 0.5;

    half3 v3 = smoothstep(t_start, t_end, t);

    half edge = lerp(v3.x, v3.y, saturate(uv.x * 2));
    edge = lerp(edge, v3.z, saturate(uv.x * 2 - 1));

    return (uv.y <= edge) ^ (_FxParam.z >= 1);
}

half4 Frag(Varyings input) : SV_Target
{
    float2 uv = input.texcoord;
    bool mask = false;

    #if defined(OVERLAYFX_SLITS)
    mask = mask || SlitsMask(uv);
    #endif

    #if defined(OVERLAYFX_BLOTS)
    mask = mask || BlotsMask(uv);
    #endif

    #if defined(OVERLAYFX_WIPER)
    mask = mask || WiperMask(uv);
    #endif

    return ApplyOverlay(uv, mask);
}

ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Name "OverlayFX"
            ZTest Always ZWrite Off Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_local_fragment __ OVERLAYFX_SLITS
            #pragma multi_compile_local_fragment __ OVERLAYFX_BLOTS
            #pragma multi_compile_local_fragment __ OVERLAYFX_WIPER
            ENDHLSL
        }
    }
}
