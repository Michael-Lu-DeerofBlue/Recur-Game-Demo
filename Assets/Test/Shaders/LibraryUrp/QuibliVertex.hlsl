#ifndef QUIBLI_VERTEX_INCLUDED
#define QUIBLI_VERTEX_INCLUDED

half _WindEnabled;
half _WindIntensity;
half _WindFrequency;
half _WindNoise;
half _WindSpeed;
half _WindDirection;

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
// ReSharper disable once CppUnusedIncludeDirective
#include "../Grass/FoliageLib.hlsl"

Varyings LitPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    // Wind (Ambient).
    #if defined(DR_WIND_ENABLED)
    {
        const float2 noise_uv = input.texcoord * _WindFrequency;
        const float noise01 = GradientNoise(noise_uv, 1.0);
        const float noise = (noise01 * 2.0 - 1.0) * _WindNoise;

        const float s = SineWave(input.positionOS.xyz, noise, _WindSpeed, _WindFrequency) * _WindIntensity;

        input.positionOS.xy += s * 0.1 * input.positionOS.y;
    }
    #endif

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

    // normalWS and tangentWS already normalize.
    // this is required to avoid skewing the direction during interpolation
    // also required for per-vertex lighting and SH evaluation
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    #if defined(_FOG_FRAGMENT)
    half fogFactor = 0;
    #else
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    #endif

    const half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);

    // already normalized from normal transform to WS.
    output.normalWS = normalInput.normalWS;
    output.viewDirWS = viewDirWS;
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR) || defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    real sign = input.tangentOS.w * GetOddNegativeScale();
    half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
    #endif
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
    output.tangentWS = tangentWS;
    #endif

    OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
    #ifdef DYNAMICLIGHTMAP_ON
    output.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif

    #if UNITY_VERSION >= 202318
    OUTPUT_SH4(vertexInput.positionWS, output.normalWS.xyz, GetWorldSpaceNormalizeViewDir(vertexInput.positionWS), output.vertexSH);
    #else
    #if UNITY_VERSION >= 202310
    OUTPUT_SH(vertexInput.positionWS, output.normalWS.xyz, GetWorldSpaceNormalizeViewDir(vertexInput.positionWS), output.vertexSH);
    #else
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
    #endif
    #endif

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
    #else
    output.fogFactor = fogFactor;
    #endif

    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    output.positionWS = vertexInput.positionWS;
    #endif

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    output.shadowCoord = GetShadowCoord(vertexInput);
    #endif

    output.positionCS = vertexInput.positionCS;

    #if defined(DR_VERTEX_COLORS_ON)
    output.VertexColor = input.color;
    #endif

    return output;
}

#endif
