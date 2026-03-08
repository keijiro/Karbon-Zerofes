Shader "Hidden/Karbon/OverlayFX"
{
HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise3D.hlsl"

float3 _FxParam;
float _FxSeed;

half4 ApplyOverlay(float2 uv, bool mask)
{
    return mask ? 1 : SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
}

bool SlitsMask(float2 uv)
{
    float n = 0;
    float t = _Time.y * 1.8;

    n += SimplexNoise(float2(uv.x * 6, t));
    n += SimplexNoise(float2(uv.x * 20, t));

    return abs(n) < _FxParam.x;
}

bool BlotsMask(float2 uv)
{
    float2 asp = float2(960.0 / 256, 1);
    float n = abs(SimplexNoise(float3(uv * asp * 1.1, _Time.y * 0.25)));

    return n > 1 - _FxParam.y;
}

bool WiperMask(float2 uv)
{
    uint seed = (uint)floor(_FxSeed);
    float time = frac(_FxParam.z);

    float y1 = smoothstep(Hash(seed + 0) / 2, Hash(seed + 1) / 2 + 0.5, time);
    float y2 = smoothstep(Hash(seed + 2) / 2, Hash(seed + 3) / 2 + 0.5, time);
    float y3 = smoothstep(Hash(seed + 4) / 2, Hash(seed + 5) / 2 + 0.5, time);

    uint h = JenkinsHash(seed + 6);
    if (h & 1) uv = 1 - uv;
    if (h & 2) uv = uv.yx;

    float thresh = lerp(lerp(y1, y2, saturate(uv.x * 2)), y3, saturate(uv.x * 2 - 1));
    return (uv.y < thresh) ^ (_FxParam.z > 1);
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
