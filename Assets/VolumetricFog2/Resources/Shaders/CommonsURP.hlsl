#ifndef VOLUMETRIC_FOG_2_COMMONS_URP
#define VOLUMETRIC_FOG_2_COMMONS_URP

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

// ***** Custom options ************

//#define ORTHO_SUPPORT
//#define USE_ALTERNATE_RECONSTRUCT_API

#define MAX_ITERATIONS 500

//#define FOG_BLUE_NOISE

//#define WEBGL_COMPATIBILITY_MODE

//#define FOG_VOID_ROTATION

#define USE_WORLD_SPACE_NOISE

//#define FOG_ROTATION

//#define FOG_BORDER

//#define FOG_MAX_DISTANCE_XZ

//#define FOG_SHADOW_CANCELLATION

//#define V2F_LIGHT_COOKIE_CANCELLATION

//#define FOG_FORWARD_PLUS_IGNORE_CLUSTERING

//#define FOG_FORWARD_PLUS_ADDITIONAL_DIRECTIONAL_LIGHTS

// ***** Common URP code ***********

#if defined(USE_ALTERNATE_RECONSTRUCT_API)
   #define CLAMP_RAY_DEPTH(rayStart, uv, t1) ClampRayDepthAlt(rayStart, uv, t1)
#else
   #define CLAMP_RAY_DEPTH(rayStart, uv, t1) ClampRayDepth(rayStart, uv, t1)
#endif

#define CLAMP_RAY_START(rayStart, uv, t0) ClampRayStart(rayStart, uv, t0)

TEXTURE2D_X_FLOAT(_CustomDepthTexture);
SAMPLER(sampler_CustomDepthTexture);
int VF2_FLIP_DEPTH_TEXTURE;


inline float GetRawDepth(float2 uv) {
    float sceneDepth = SampleSceneDepth(VF2_FLIP_DEPTH_TEXTURE ? float2(uv.x, 1.0 - uv.y) : uv);
    #if VF2_DEPTH_PREPASS
        float customDepth = SAMPLE_TEXTURE2D_X(_CustomDepthTexture, sampler_CustomDepthTexture, uv ).r;
        #if UNITY_REVERSED_Z
            sceneDepth = max(sceneDepth, customDepth);
        #else
            sceneDepth = min(sceneDepth, customDepth);
        #endif
    #endif
    return sceneDepth;
}


void ClampRayDepth(float3 rayStart, float2 uv, inout float t1) {

    #if UNITY_REVERSED_Z
        float depth = GetRawDepth(uv);
    #else
        float depth = GetRawDepth(uv);
        depth = depth * 2.0 - 1.0;
    #endif

    float3 wpos = ComputeWorldSpacePosition(uv, depth, unity_MatrixInvVP);

    // World position reconstruction (old code)
/*
    float depth  = GetRawDepth(uv);
    float4 raw   = mul(UNITY_MATRIX_I_VP, float4(uv * 2.0 - 1.0, depth, 1.0));
    float3 wpos  = raw.xyz / raw.w;
*/

    float z = distance(rayStart, wpos);

    #if defined(ORTHO_SUPPORT)
        #if defined(UNITY_REVERSED_Z)
            depth = 1.0 - depth;
        #endif
        z = lerp(z, lerp(_ProjectionParams.y, _ProjectionParams.z, depth), unity_OrthoParams.w);
    #else

    #endif

    t1 = min(t1, z);
}

void ClampRayStart(float3 rayStart, float2 uv, inout float t0) {

    float customDepth = SAMPLE_TEXTURE2D_X(_CustomDepthTexture, sampler_CustomDepthTexture, uv ).r;
    #if UNITY_REVERSED_Z
        float depth = customDepth;
    #else
        float depth = customDepth;
        depth = depth * 2.0 - 1.0;
    #endif

    float3 wpos = ComputeWorldSpacePosition(uv, depth, unity_MatrixInvVP);

    float z = distance(rayStart, wpos);

    #if defined(ORTHO_SUPPORT)
        #if defined(UNITY_REVERSED_Z)
            depth = 1.0 - depth;
        #endif
        z = lerp(z, lerp(_ProjectionParams.y, _ProjectionParams.z, depth), unity_OrthoParams.w);
    #else

    #endif

    t0 = max(t0, z);
}




// Alternate reconstruct API; URP 7.4 doesn't set UNITY_MATRIX_I_VP correctly in VR, so we need to use this alternate method

inline float GetLinearEyeDepth(float2 uv) {
    float rawDepth = SampleSceneDepth(VF2_FLIP_DEPTH_TEXTURE ? float2(uv.x, 1.0 - uv.y) : uv);
  	float sceneLinearDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
    #if defined(ORTHO_SUPPORT)
        #if UNITY_REVERSED_Z
              rawDepth = 1.0 - rawDepth;
        #endif
        float orthoDepth = lerp(_ProjectionParams.y, _ProjectionParams.z, rawDepth);
        sceneLinearDepth = lerp(sceneLinearDepth, orthoDepth, unity_OrthoParams.w);
    #endif
    #if VF2_DEPTH_PREPASS
        float customRawDepth = SAMPLE_TEXTURE2D_X(_CustomDepthTexture, sampler_CustomDepthTexture, uv).r;
        float customLinearDepth = LinearEyeDepth(customRawDepth, _ZBufferParams);
        #if defined(ORTHO_SUPPORT)
            #if UNITY_REVERSED_Z
                customRawDepth = 1.0 - customRawDepth;
            #endif
            float customOrthoDepth = lerp(_ProjectionParams.y, _ProjectionParams.z, customRawDepth);
            customLinearDepth = lerp(customLinearDepth, customOrthoDepth, unity_OrthoParams.w);
        #endif
        sceneLinearDepth = min(sceneLinearDepth, customLinearDepth);
    #endif
    return sceneLinearDepth;
}


void ClampRayDepthAlt(float3 rayStart, float2 uv, inout float t1) {

    float vz = GetLinearEyeDepth(uv);

    #if defined(ORTHO_SUPPORT)
        if (unity_OrthoParams.w) {
            t1 = min(t1, vz);
            return;
        }
    #endif
    float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
    float2 suv = uv;
    #if UNITY_SINGLE_PASS_STEREO
        // If Single-Pass Stereo mode is active, transform the
        // coordinates to get the correct output UV for the current eye.
        float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
        suv = (suv - scaleOffset.zw) / scaleOffset.xy;
    #endif
    float3 vpos = float3((suv * 2 - 1) / p11_22, -1) * vz;
    float4 wpos = mul(unity_CameraToWorld, float4(vpos, 1));
    float z = distance(rayStart, wpos.xyz);
    t1 = min(t1, z);
}


#endif // VOLUMETRIC_FOG_2_COMMONS_URP

