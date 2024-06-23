#ifndef DR_LIGHTING_INCLUDED
#define DR_LIGHTING_INCLUDED

// ReSharper disable CppInconsistentNaming

//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

// #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
// #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

// TODO: Add a variant with _ADDITIONAL_LIGHTS
// (https://blog.unity.com/technology/custom-lighting-in-shader-graph-expanding-your-graphs-in-2019).
void MainLight_half(float3 WorldPosition, out half3 Direction, out half3 Color, out half DistanceAttenuation, out half ShadowAttenuation) {
    #ifdef SHADERGRAPH_PREVIEW
    Direction = half3(0.5, 0.5, 0);
    Color = 1;
    DistanceAttenuation = 1;
    ShadowAttenuation = 1;
    #else

    #if SHADOWS_SCREEN
    half4 clipPos = TransformWorldToHClip(WorldPosition);
    half4 shadowCoord = ComputeScreenPos(clipPos);
    #else
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPosition);
    #endif

    Light light = GetMainLight(shadowCoord);
    Direction = light.direction;
    Color = light.color;
    DistanceAttenuation = light.distanceAttenuation;
    ShadowAttenuation = light.shadowAttenuation;

    #endif
}

void NDotL_half(half3 Normal, half3 LightDirection, half ShadingFactor, out half Shading) {
    const half nDotL = dot(Normal, LightDirection) * 0.5 + 0.5;
    Shading = saturate(nDotL * ShadingFactor);
}

#endif
