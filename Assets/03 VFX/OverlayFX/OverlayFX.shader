Shader "Hidden/Karbon/OverlayFX"
{
HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise3D.hlsl"

float _FxParam;
uint _FxSeed;

half4 ApplyOverlay(float2 uv, bool mask)
{
    return mask ? 1 : SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
}

half4 FragSlits(Varyings input) : SV_Target
{
    float2 uv = input.texcoord;

    float n = 0;
    float t = _Time.y * 1.8;

    n += SimplexNoise(float2(uv.x * 6, t));
    n += SimplexNoise(float2(uv.x * 20, t));

    bool mask = abs(n) < _FxParam;

    return ApplyOverlay(uv, mask);
}

half4 FragBlots(Varyings input) : SV_Target
{
    float2 uv = input.texcoord;

    float2 asp = float2(960.0 / 256, 1);
    float n = abs(SimplexNoise(float3(uv * asp * 1.1, _Time.y * 0.25)));

    bool mask = n > 1 - _FxParam;

    return ApplyOverlay(uv, mask);
}

half4 FragWiper(Varyings input) : SV_Target
{
    float2 uv = input.texcoord;

    uint seed = _FxSeed;
    float time = frac(_FxParam);

    float y1 = smoothstep(Hash(seed + 0) / 2, Hash(seed + 1) / 2 + 0.5, time);
    float y2 = smoothstep(Hash(seed + 2) / 2, Hash(seed + 3) / 2 + 0.5, time);
    float y3 = smoothstep(Hash(seed + 4) / 2, Hash(seed + 5) / 2 + 0.5, time);

    uint h = JenkinsHash(seed + 6);
    if (h & 1) uv = 1 - uv;
    if (h & 2) uv = uv.yx;

    float thresh = lerp(lerp(y1, y2, saturate(uv.x * 2)), y3, saturate(uv.x * 2 - 1));
    bool mask = (uv.y < thresh) ^ (_FxParam > 1);

    return ApplyOverlay(uv, mask);
}

ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Name "OverlayFX Slits"
            ZTest Always ZWrite Off Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragSlits
            ENDHLSL
        }
        Pass
        {
            Name "OverlayFX Blots"
            ZTest Always ZWrite Off Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragBlots
            ENDHLSL
        }
        Pass
        {
            Name "OverlayFX Wiper"
            ZTest Always ZWrite Off Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragWiper
            ENDHLSL
        }
    }
}
