#ifndef VOLUMETRIC_FOG_2_APV
#define VOLUMETRIC_FOG_2_APV

#if UNITY_VERSION >= 202310 && defined(VF2_APV)

    #if PROBE_VOLUMES_L1 || PROBE_VOLUMES_L2
    	#include "Packages/com.unity.render-pipelines.core/Runtime/Lighting/ProbeVolume/ProbeVolume.hlsl"

        void EvaluateAdaptiveProbeVolumeNoNoise(in float3 posWS, out float3 bakeDiffuseLighting) {
            APVResources apvRes = FillAPVResources();
            float3 uvw;
            if (TryToGetPoolUVW(apvRes, posWS, 0, 0, uvw)) {
                bakeDiffuseLighting = SAMPLE_TEXTURE3D_LOD(apvRes.L0_L1Rx, s_linear_clamp_sampler, uvw, 0).rgb;
            } else {
                bakeDiffuseLighting = EvaluateAmbientProbe(0);
            }
        }

        half _APVIntensityMultiplier;

        half3 GetAPVColor(float3 wpos) {
	        float3 gi;
	        EvaluateAdaptiveProbeVolumeNoNoise(wpos, gi);
            return gi * _APVIntensityMultiplier;
        }
    #else

        half3 GetAPVColor(float3 wpos) { return 0; }

    #endif // PROBE_VOLUMES_L1 || PROBE_VOLUMES_L2

#endif // UNITY_VERSION >= 202310 && defined(VF2_APV)

#endif // VOLUMETRIC_FOG_2_APV