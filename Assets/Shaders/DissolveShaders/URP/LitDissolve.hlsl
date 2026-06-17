#ifndef LIT_DISSOLVE_INCLUDED
#define LIT_DISSOLVE_INCLUDED

TEXTURE2D(_NoiseTex);
SAMPLER(sampler_NoiseTex);

CBUFFER_START(UnityPerMaterialDissolve)
    half4 _NoiseTex_ST;
    half _DissolveAmount;
    half4 _DissolveColor;
    half _DissolveWidth;
CBUFFER_END

inline float LitDissolveSampleNoise(float2 uv)
{
    float2 noiseUV = uv * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
    return SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;
}

inline void ApplyDissolveClip(float2 uv)
{
    clip(LitDissolveSampleNoise(uv) - _DissolveAmount);
}

inline void ApplyDissolveEmission(float2 uv, inout half3 emission)
{
    float dissolveNoise = LitDissolveSampleNoise(uv);
    emission += _DissolveColor.rgb * step(dissolveNoise, _DissolveAmount + _DissolveWidth);
}

inline void ApplyDissolve(float2 uv, inout half3 emission)
{
    float dissolveNoise = LitDissolveSampleNoise(uv);
    clip(dissolveNoise - _DissolveAmount);
    emission += _DissolveColor.rgb * step(dissolveNoise, _DissolveAmount + _DissolveWidth);
}

#endif
