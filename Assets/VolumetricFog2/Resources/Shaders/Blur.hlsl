#ifndef VOLUMETRIC_FOG_2_BLUR
#define VOLUMETRIC_FOG_2_BLUR

#include "CommonsURP.hlsl"

TEXTURE2D_X(_MainTex);
TEXTURE2D_X(_LightBuffer);
TEXTURE2D_X(_DownsampledDepth);
TEXTURE2D(_BlueNoise);
float4  _BlueNoise_TexelSize;
float4  _MainTex_TexelSize;
float4  _MainTex_ST;
float4  _DownsampledDepth_TexelSize;
float4  _MiscData;
float   _BlurScale;
half4   _ScatteringData;
half3   _ScatteringTint;

#define DITHER_STRENGTH _MiscData.x
#define BRIGHTNESS _MiscData.y
#define BLUR_EDGE_THRESHOLD _MiscData.z
#define DOWNSCALING_EDGE_THRESHOLD _MiscData.w
#define BLUR_SCALE _BlurScale
#define SCATTERING _ScatteringData.w
#define SCATTERING_THRESHOLD _ScatteringData.x
#define SCATTERING_INTENSITY _ScatteringData.y
#define SCATTERING_ABSORPTION _ScatteringData.z
TEXTURE2D_X(_BlurredTex);

// Optimization for SSPR
#define uvN uv1
#define uvE uv2
#define uvW uv3
#define uvS uv4


#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED) || defined(SINGLE_PASS_STEREO)
    #define VERTEX_CROSS_UV_DATA

    #define VERTEX_OUTPUT_CROSS_UV(o)
    #define FRAG_SETUP_CROSS_UV(i) float3 uvInc = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0); uvInc.xy *= BLUR_SCALE; float2 uvN = i.uv + uvInc.zy; float2 uvE = i.uv + uvInc.xz; float2 uvW = i.uv - uvInc.xz; float2 uvS = i.uv - uvInc.zy;

    #define VERTEX_OUTPUT_GAUSSIAN_UV(o)
    #if defined(BLUR_HORIZ)
        #define FRAG_SETUP_GAUSSIAN_UV(i) float2 inc = float2(_MainTex_TexelSize.x * 1.3846153846 * BLUR_SCALE, 0); float2 uv1 = i.uv - inc; float2 uv2 = i.uv + inc; float2 inc2 = float2(_MainTex_TexelSize.x * 3.2307692308 * BLUR_SCALE, 0); float2 uv3 = i.uv - inc2; float2 uv4 = i.uv + inc2;
    #else
        #define FRAG_SETUP_GAUSSIAN_UV(i) float2 inc = float2(0, _MainTex_TexelSize.y * 1.3846153846 * BLUR_SCALE); float2 uv1 = i.uv - inc; float2 uv2 = i.uv + inc; float2 inc2 = float2(0, _MainTex_TexelSize.y * 3.2307692308 * BLUR_SCALE); float2 uv3 = i.uv - inc2; float2 uv4 = i.uv + inc2;
    #endif

#else
    #define VERTEX_CROSS_UV_DATA float2 uvN : TEXCOORD1; float2 uvW: TEXCOORD2; float2 uvE: TEXCOORD3; float2 uvS: TEXCOORD4;

    #define VERTEX_OUTPUT_CROSS_UV(o) float3 uvInc = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0); uvInc.xy *= BLUR_SCALE; o.uvN = o.uv + uvInc.zy; o.uvE = o.uv + uvInc.xz; o.uvW = o.uv - uvInc.xz; o.uvS = o.uv - uvInc.zy;
    #define FRAG_SETUP_CROSS_UV(i) float2 uv1 = i.uv1; float2 uv2 = i.uv2; float2 uv3 = i.uv3; float2 uv4 = i.uv4;

    #if defined(BLUR_HORIZ)
        #define VERTEX_OUTPUT_GAUSSIAN_UV(o) float2 inc = float2(_MainTex_TexelSize.x * 1.3846153846 * BLUR_SCALE, 0); o.uv1 = o.uv - inc; o.uv2 = o.uv + inc; float2 inc2 = float2(_MainTex_TexelSize.x * 3.2307692308 * BLUR_SCALE, 0); o.uv3 = o.uv - inc2; o.uv4 = o.uv + inc2;
    #else
        #define VERTEX_OUTPUT_GAUSSIAN_UV(o) float2 inc = float2(0, _MainTex_TexelSize.y * 1.3846153846 * BLUR_SCALE); o.uv1 = o.uv - inc; o.uv2 = o.uv + inc; float2 inc2 = float2(0, _MainTex_TexelSize.y * 3.2307692308 * BLUR_SCALE); o.uv3 = o.uv - inc2; o.uv4 = o.uv + inc2;
    #endif
    #define FRAG_SETUP_GAUSSIAN_UV(i) float2 uv1 = i.uv1; float2 uv2 = i.uv2; float2 uv3 = i.uv3; float2 uv4 = i.uv4;

#endif

struct AttributesSimple
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

	struct VaryingsCross {
	    float4 positionCS : SV_POSITION;
	    float2 uv: TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_OUTPUT_STEREO
	};


	VaryingsCross VertBlur(AttributesSimple v) {
    	VaryingsCross o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_TRANSFER_INSTANCE_ID(v, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	    o.positionCS = v.positionOS;
		o.positionCS.y *= _ProjectionParams.x;
    	o.uv = v.uv;
        VERTEX_OUTPUT_GAUSSIAN_UV(o)

    	return o;
	}

    inline void ApplyDither(inout half4 pixel, float2 uv) {
        // screen space dithering
        //const float3 magic = float3( 0.06711056, 0.00583715, 52.9829189 );
        //float jitter = frac( magic.z * frac( dot( uv * _MainTex_TexelSize.zw, magic.xy ) ) );
	    //pixel = max(0, pixel - jitter * DITHER_STRENGTH);

        // blue noise based dithering
        float2 noiseUV = uv * _MainTex_TexelSize.zw * _BlueNoise_TexelSize.xy;
        half jitter = SAMPLE_TEXTURE2D(_BlueNoise, sampler_PointRepeat, noiseUV).r;
        pixel = max(0, pixel - jitter * DITHER_STRENGTH);
    }

#if EDGE_PRESERVE_UPSCALING

    inline float GetDownsampledLinearEyeDepth(float2 uv) {
        float rawDepth = SAMPLE_TEXTURE2D_X(_DownsampledDepth, sampler_LinearClamp, uv).r;
	    float sceneLinearDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
        #if defined(ORTHO_SUPPORT)
            #if UNITY_REVERSED_Z
                  rawDepth = 1.0 - rawDepth;
            #endif
            float orthoDepth = lerp(_ProjectionParams.y, _ProjectionParams.z, rawDepth);
            sceneLinearDepth = lerp(sceneLinearDepth, orthoDepth, unity_OrthoParams.w);
        #endif
        return sceneLinearDepth;
    }

#else

    #define GetDownsampledLinearEyeDepth(uv) GetLinearEyeDepth(uv)

#endif


	half4 FragBlur (VaryingsCross i): SV_Target {
    	UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        FRAG_SETUP_GAUSSIAN_UV(i)

        half4 pixel0 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
        half4 pixel1 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1);
        half4 pixel2 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2);
	    half4 pixel3 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3);
        half4 pixel4 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4);

        float w0 = 0.2270270270;
        half4 wn = half4(0.3162162162.xx, 0.0702702703.xx);

        #if VF2_DEPTH_PEELING // avoids artifacts on particles rect borders
            w0   *= pixel0.a;
            wn.x *= pixel1.a;
            wn.y *= pixel2.a;
            wn.z *= pixel3.a;
            wn.w *= pixel4.a;
        #endif
        #if EDGE_PRESERVE_UPSCALING || EDGE_PRESERVE
            float depthFull = GetDownsampledLinearEyeDepth(i.uv);
            float4 depths;
            depths.x = GetDownsampledLinearEyeDepth(uv1);
            depths.y = GetDownsampledLinearEyeDepth(uv2);
            depths.z = GetDownsampledLinearEyeDepth(uv3);
            depths.w = GetDownsampledLinearEyeDepth(uv4);
            half4 depthDiffs = 1.0 + abs(depths - depthFull) * BLUR_EDGE_THRESHOLD;
            wn *= saturate(1.0 / depthDiffs);
            half w = w0 + dot(wn, 1.0);
            half4 pixel = pixel0 * w0 + pixel1 * wn.x + pixel2 * wn.y + pixel3 * wn.z + pixel4 * wn.w;
            pixel /= w + 0.00001;
        #else
            half4 pixel = pixel0 * w0 + pixel1 * wn.x + pixel2 * wn.y + pixel3 * wn.z + pixel4 * wn.w;
        #endif

        #if DITHER
            ApplyDither(pixel, i.uv);
        #endif

  		return pixel;
	}

	struct VaryingsSimple {
	    float4 positionCS : SV_POSITION;
	    float2 uv: TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
	};


	VaryingsSimple VertSimple(AttributesSimple v) {
    	VaryingsSimple o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_TRANSFER_INSTANCE_ID(v, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	    o.positionCS = v.positionOS;
		o.positionCS.y *= _ProjectionParams.x;
    	o.uv = v.uv;

    	return o;
	}

	half4 FragUpscalingBlend (VaryingsSimple i): SV_Target {
    	UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        float depthFull = GetRawDepth(i.uv);

        const float t = 0.5;
        float2 uv00 = UnityStereoTransformScreenSpaceTex(i.uv + _DownsampledDepth_TexelSize.xy * float2(-t, -t));
        float2 uv10 = UnityStereoTransformScreenSpaceTex(i.uv + _DownsampledDepth_TexelSize.xy * float2(t, -t));
        float2 uv01 = UnityStereoTransformScreenSpaceTex(i.uv + _DownsampledDepth_TexelSize.xy * float2(-t, t));
        float2 uv11 = UnityStereoTransformScreenSpaceTex(i.uv + _DownsampledDepth_TexelSize.xy * float2(t, t));
        float4 depths;
        depths.x = SAMPLE_TEXTURE2D_X(_DownsampledDepth, sampler_LinearClamp, uv00).r;
        depths.y = SAMPLE_TEXTURE2D_X(_DownsampledDepth, sampler_LinearClamp, uv10).r;
        depths.z = SAMPLE_TEXTURE2D_X(_DownsampledDepth, sampler_LinearClamp, uv01).r;
        depths.w = SAMPLE_TEXTURE2D_X(_DownsampledDepth, sampler_LinearClamp, uv11).r;

        float4 diffs = abs(depthFull.xxxx - depths);

        float2 minUV = UnityStereoTransformScreenSpaceTex(i.uv);
        if (any(diffs > DOWNSCALING_EDGE_THRESHOLD)) {
            // Check 10 vs 00
            float minDiff  = lerp(diffs.x, diffs.y, diffs.y < diffs.x);
            minUV    = lerp(uv00, uv10, diffs.y < diffs.x);
            // Check against 01
            minUV    = lerp(minUV, uv01, diffs.z < minDiff);
            minDiff  = lerp(minDiff, diffs.z, diffs.z < minDiff);
            // Check against 11
            minUV    = lerp(minUV, uv11, diffs.w < minDiff);
        } 

        #if VF2_DEPTH_PEELING // avoids artifacts on particles rect borders
            half4 pixel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, minUV);
        #else
            half4 pixel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, minUV);
        #endif
        
        #if DITHER
            ApplyDither(pixel, i.uv);
        #endif

   		return pixel;
	}

    half4 FragBlend (VaryingsSimple i): SV_Target {
    	UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        half4 pixel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, UnityStereoTransformScreenSpaceTex(i.uv));

        #if DITHER
            ApplyDither(pixel, i.uv);
        #endif

   		return pixel;
	}	

	
	float4 FragOnlyDepth (VaryingsSimple i): SV_Target {
    	UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        return GetRawDepth(i.uv);
	}


   	VaryingsCross VertCross(AttributesSimple v) {
    	VaryingsCross o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_TRANSFER_INSTANCE_ID(v, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = v.positionOS;
		o.positionCS.y *= _ProjectionParams.x;
        o.uv = v.uv;
        VERTEX_OUTPUT_CROSS_UV(o)

		return o;
	}

	half Brightness(half3 c) {
		return max(c.r, max(c.g, c.b));
	}

  	half4 FragScatteringPrefilter(VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
		half4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);

        #if UNITY_COLORSPACE_GAMMA
            color.rgb = FastSRGBToLinear(color.rgb);
        #endif

        half3 extraLight = max(0, color.rgb - SCATTERING_THRESHOLD);
        color.rgb += extraLight * SCATTERING_INTENSITY;

        color.a = Brightness(color.rgb);

        return color;
    }
    
  	half4 FragResample(VaryingsCross i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        FRAG_SETUP_CROSS_UV(i)

		half4 c1 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1);
		half4 c2 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2);
		half4 c3 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3);
		half4 c4 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4);
			    
	    half w1 = 1.0 / (c1.a + 1.0);
	    half w2 = 1.0 / (c2.a + 1.0);
	    half w3 = 1.0 / (c3.a + 1.0);
	    half w4 = 1.0 / (c4.a + 1.0);
	    
	    half dd  = 1.0 / (w1 + w2 + w3 + w4);
	    half4 v = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
        #if defined(COMBINE_SCATTERING)
            half4 o = SAMPLE_TEXTURE2D_X(_BlurredTex, sampler_LinearClamp, i.uv);
            v = (o + v * (1.0 + SCATTERING)) / (1.0 + SCATTERING * 0.6735);
            v *= SCATTERING_ABSORPTION;
        #endif
        return v;
	}


  	half4 FragScatteringBlend(VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

		half4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);

        #if UNITY_COLORSPACE_GAMMA
            color.rgb = FastLinearToSRGB(color.rgb);
        #endif

        float2 minUV = i.uv;
        #if SCATTERING_HQ
            float depthFull = GetRawDepth(i.uv);
            const float t = 0.5;
            float2 uv00 = UnityStereoTransformScreenSpaceTex(i.uv + _DownsampledDepth_TexelSize.xy * float2(-t, -t));
            float2 uv10 = UnityStereoTransformScreenSpaceTex(i.uv + _DownsampledDepth_TexelSize.xy * float2(t, -t));
            float2 uv01 = UnityStereoTransformScreenSpaceTex(i.uv + _DownsampledDepth_TexelSize.xy * float2(-t, t));
            float2 uv11 = UnityStereoTransformScreenSpaceTex(i.uv + _DownsampledDepth_TexelSize.xy * float2(t, t));
            float4 depths;
            depths.x = SAMPLE_TEXTURE2D_X(_DownsampledDepth, sampler_LinearClamp, uv00).r;
            depths.y = SAMPLE_TEXTURE2D_X(_DownsampledDepth, sampler_LinearClamp, uv10).r;
            depths.z = SAMPLE_TEXTURE2D_X(_DownsampledDepth, sampler_LinearClamp, uv01).r;
            depths.w = SAMPLE_TEXTURE2D_X(_DownsampledDepth, sampler_LinearClamp, uv11).r;
            float4 diffs = abs(depthFull.xxxx - depths);

            if (any(diffs > DOWNSCALING_EDGE_THRESHOLD)) {
                // Check 10 vs 00
                float minDiff  = lerp(diffs.x, diffs.y, diffs.y < diffs.x);
                minUV    = lerp(uv00, uv10, diffs.y < diffs.x);
                // Check against 01
                minUV    = lerp(minUV, uv01, diffs.z < minDiff);
                minDiff  = lerp(minDiff, diffs.z, diffs.z < minDiff);
                // Check against 11
                minUV    = lerp(minUV, uv11, diffs.w < minDiff);
            }
        #endif
        
        half4 fog = SAMPLE_TEXTURE2D_X(_LightBuffer, sampler_LinearClamp, minUV);
        color.a = saturate( fog.a * fog.a * SCATTERING * 4.0);
        color.rgb *= _ScatteringTint;

        return color;
    }

#endif // VOLUMETRIC_FOG_2_BLUR