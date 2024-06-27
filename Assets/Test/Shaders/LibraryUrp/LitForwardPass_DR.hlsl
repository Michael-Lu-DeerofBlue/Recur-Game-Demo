#ifndef LIT_FORWARD_PASS_DR
#define LIT_FORWARD_PASS_DR

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Version.hlsl"

// Check is needed because in Unity 2021 they use two different URP versions - 14 on desktop and 12 on mobile.
#if UNITY_VERSION >= 202210
#if defined(LOD_FADE_CROSSFADE)
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
#endif
#endif
#include "Lighting_DR.hlsl"

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float2 texcoord     : TEXCOORD0;
    float2 staticLightmapUV    : TEXCOORD1;
    float2 dynamicLightmapUV    : TEXCOORD2;

#if defined(DR_VERTEX_COLORS_ON)
    float4 color        : COLOR;
#endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv                       : TEXCOORD0;

    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    float3 positionWS               : TEXCOORD1;
    #endif

    float3 normalWS                 : TEXCOORD2;
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
    float4 tangentWS                : TEXCOORD3;    // xyz: tangent, w: sign
    #endif
    float3 viewDirWS                : TEXCOORD4;

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
    half4 fogFactorAndVertexLight  : TEXCOORD5; // x: fogFactor, yzw: vertex light
    #else
    half  fogFactor                 : TEXCOORD5;
    #endif

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    float4 shadowCoord              : TEXCOORD6;
    #endif

    DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 7);

    #ifdef DYNAMICLIGHTMAP_ON
    float2  dynamicLightmapUV : TEXCOORD8; // Dynamic lightmap UVs
    #endif

    float4 positionCS               : SV_POSITION;

    // ---
    #if defined(DR_VERTEX_COLORS_ON)
    float4 VertexColor              : COLOR;
    #endif
    // ---

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

/// ---------------------------------------------------------------------------

void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
{
    inputData = (InputData)0;

    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    inputData.positionWS = input.positionWS;
    #endif

    half3 viewDirWS = SafeNormalize(input.viewDirWS);
    #if defined(_NORMALMAP) || defined(_DETAIL)
    float sgn = input.tangentWS.w;      // should be either +1 or -1
    float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
    inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz));
    #else
    inputData.normalWS = input.normalWS;
    #endif

    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    inputData.viewDirectionWS = viewDirWS;

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    inputData.shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
    #else
    inputData.shadowCoord = float4(0, 0, 0, 0);
    #endif

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
    inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactorAndVertexLight.x);
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    #else
    inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactor);
    inputData.vertexLighting = half3(0, 0, 0);
    #endif

    // Note: This needs to not interfere with _ADDITIONAL_LIGHTS_VERTEX
    #if defined(DR_VERTEX_COLORS_ON)
    inputData.vertexLighting = input.VertexColor.rgb;
    #endif
    
    #if defined(DYNAMICLIGHTMAP_ON)
    inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV, input.vertexSH, inputData.normalWS);
    #else
    inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, inputData.normalWS);
    #endif

    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);

    #if defined(DEBUG_DISPLAY)
    #if defined(DYNAMICLIGHTMAP_ON)
    inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
    #endif
    #if defined(LIGHTMAP_ON)
    inputData.staticLightmapUV = input.staticLightmapUV;
    #else
    inputData.vertexSH = input.vertexSH;
    #endif
    #endif
}

half4 StylizedPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    SurfaceData surfaceData;
    InitializeSimpleLitSurfaceData(input.uv, surfaceData);

    #if defined(LOD_FADE_CROSSFADE) && !VERSION_LOWER(13, 0)
    LODFadeCrossFade(input.positionCS);
    #endif

    InputData inputData;
    InitializeInputData(input, surfaceData.normalTS, inputData);
    #if UNITY_VERSION >= 202330
    SETUP_DEBUG_TEXTURE_DATA(inputData, input.uv);
    #elif UNITY_VERSION >= 202210
    SETUP_DEBUG_TEXTURE_DATA(inputData, input.uv, _BaseMap);
    #endif

    #ifdef _DBUFFER
    ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
    #endif

    half4 color = UniversalFragment_DSTRM(inputData, surfaceData, input.uv);

    color.rgb = MixFog(color.rgb, inputData.fogCoord);

    #if UNITY_VERSION >= 202220
    color.a = OutputAlpha(color.a, IsSurfaceTypeTransparent(_Surface));
    #else
    color.a = OutputAlpha(color.a, _Surface);
    #endif

    return color;
}

#endif // LIT_FORWARD_PASS_DR
