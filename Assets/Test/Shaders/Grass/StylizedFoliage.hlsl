#ifndef STYLIZED_FOLIAGE_INCLUDED
#define STYLIZED_FOLIAGE_INCLUDED

#include "FoliageLib.hlsl"
#include "../LibraryUrp/SuppressWarnings.hlsl"
// ReSharper disable once CppUnusedIncludeDirective

void GrassVert_float(float3 PositionOS, float3 PositionWS, float3 ObjectScale, float2 UV, out float3 PositionOut,
                     out float3 ColorOut)
{
    PositionOut = PositionOS;
    ColorOut = _ColorTop.rgb;

    // Wind (Ambient).
    UNITY_BRANCH
    if (_WindEnabled)
    {
        const float2 noise_uv = UV * _WindFrequency;
        const float noise01 = GradientNoise(noise_uv, 1.0);
        const float noise = (noise01 * 2.0 - 1.0) * _WindNoise;

        float s = SineWave(PositionOS, noise, _WindSpeed, _WindFrequency);
        // s *= SineWave(positionOS, HALF_PI + noise);

        s = s * UV.y * ObjectScale.y * _WindIntensity;

        PositionOut.xz += s * 0.1;
        PositionOut.y += s * 0.02;
    }

    // Patches.
    UNITY_BRANCH
    if (_PatchesEnabled)
    {
        const float cell_size = 50.0;
        const float2 noise_uv = float2(sin((PositionWS.x + _PatchesOffset.x) / cell_size),
                                       sin((PositionWS.z + _PatchesOffset.y) / cell_size)) * 0.5 + 0.5;
        float noise = GradientNoise(noise_uv, _PatchesScale * 100.0);
        const float patch_t = smoothstep(_PatchesThreshold - _PatchesBluriness * 0.5,
                                         _PatchesThreshold + _PatchesBluriness * 0.5, noise);
        ColorOut = lerp(ColorOut, _PatchesColor.rgb, patch_t);
    }

    // Gusts.
    UNITY_BRANCH
    if (_GustEnabled)
    {
        float s = SineWave(PositionWS, 0.0, _GustSpeed * 1.5, _GustFrequency);
        s = 1.0 - abs(s);
        s = pow(s, 3.0);
        const float angle = atan2(PositionWS.z, PositionWS.x) + _WindDirection * PI - PI * 0.25;
        const float l = length(PositionWS.xz);
        const float2 rotated_point = float2(cos(angle), sin(angle)) * l;
        s *= smoothstep(0.2, 0.9, GradientNoise(rotated_point + _Time.z * _GustSpeed, 0.2));

        PositionOut.xz += s  * UV.y * abs(UV.x - 0.5) * ObjectScale.y * _GustIntensity * 2.0;
        PositionOut.y += s * UV.y * ObjectScale.y * _GustIntensity * 0.5;

        ColorOut += _GustColor.rgb * length(ColorOut) * _GustColor.a * s;
    }
}

void GrassFrag_float(float4 BaseMapColor, float2 UV, float3 ColorTop, out float3 Color, out float Alpha)
{
    const float3 albedo = lerp(_ColorBottom.rgb, ColorTop.rgb, UV.y);
    Color = BaseMapColor.rgb * albedo;
    Alpha = BaseMapColor.a;
}

#endif
