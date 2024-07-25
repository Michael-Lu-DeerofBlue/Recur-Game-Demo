Shader "VolumetricFog2/VolumetricFog2DURP"
{
	Properties
	{
		[HideInInspector] _MainTex("Noise Texture", 2D) = "white" {}
		[HideInInspector] _DetailTex("Detail Texture", 3D) = "white" {}
		[HideInInspector] _NoiseScale("Noise Scale", Float) = 0.025
		[HideInInspector] _NoiseFinalMultiplier("Noise Scale", Float) = 1.0
		[HideInInspector] _NoiseStrength("Noise Strength", Float) = 1.0
		[HideInInspector] _Color("Color", Color) = (1,1,1)
		[HideInInspector] _Density("Density", Float) = 1.0
		[HideInInspector] _DeepObscurance("Deep Obscurance", Range(0, 2)) = 0.7
		[HideInInspector] _LightColor("Light Color", Color) = (1,1,1)
		[HideInInspector] _LightDiffusionPower("Sun Diffusion Power", Range(1, 64)) = 32
		[HideInInspector] _LightDiffusionIntensity("Sun Diffusion Intensity", Range(0, 1)) = 0.4
		[HideInInspector] _SunDir("Sun Direction", Vector) = (1,0,0)
		[HideInInspector] _ShadowData("Shadow Data", Vector) = (0.5, 0, 62500)
		[HideInInspector] _WindDirection("Wind Direction", Vector) = (1, 0, 0)
		[HideInInspector] _DetailWindDirection("Detail Wind Direction", Vector) = (1, 0, 0)
		[HideInInspector] _RayMarchSettings("Raymarch Settings", Vector) = (2, 0.01, 1.0, 0.1)
		[HideInInspector] _BoundsCenter("Bounds Center", Vector) = (0,0,0)
		[HideInInspector] _BoundsExtents("Bounds Size", Vector) = (0,0,0)
		[HideInInspector] _BoundsBorder("Bounds Border", Vector) = (0,1,0)
		[HideInInspector] _BoundsData("Bounds Data", Vector) = (0,0,1)
		[HideInInspector] _DetailData("Detail Data", Vector) = (0.5, 4, -0.5, 0)
		[HideInInspector] _DetailColor("Detail Color", Color) = (0.5,0.5,0.5,0)
		[HideInInspector] _DetailOffset("Detail Offset", Float) = -0.5
		[HideInInspector] _DistanceData("Distance Data", Vector) = (0, 5, 1, 1)
		[HideInInspector] _DepthGradientTex("Depth Gradient Texture", 2D) = "white" {}
		[HideInInspector] _HeightGradientTex("Height Gradient Texture", 2D) = "white" {}
		[HideInInspector] _SpecularThreshold("Specular Threshold", Float) = 0.5
		[HideInInspector] _SpecularIntensity("Specular Intensity", Float) = 0
		[HideInInspector] _SpecularColor("Specular Color", Color) = (0.5,0.5,0.5,0)
		[HideInInspector] _FogOfWarCenterAdjusted("FoW Center Adjusted", Vector) = (0,0,0)
		[HideInInspector] _FogOfWarSize("FoW Size", Vector) = (0,0,0)
		[HideInInspector] _FogOfWarCenter("FoW Center", Vector) = (0,0,0)
		[HideInInspector] _FogOfWar("FoW Texture", 2D) = "white" {}
		[HideInInspector] _BlueNoise("_Blue Noise Texture", 2D) = "white" {}
		[HideInInspector] _MaxDistanceData("Max Lengh Data", Vector) = (100000, 0.00001, 0)
		[HideInInspector] _NativeLightsMultiplier("Native Lights Multiplier", Float) = 1
		[HideInInspector] _APVIntensityMultiplier("APV Intensity Multiplier", Float) = 1
	}
		SubShader
		{
			Name "Volumetric Fog"
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent+100" "DisableBatching" = "True" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }
			Blend One OneMinusSrcAlpha
			ZTest Always
			Cull Front
			ZWrite Off

			Pass
			{
				Tags { "LightMode" = "UniversalForward" }
				HLSLPROGRAM
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag

				#if UNITY_VERSION < 202100
					#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
					#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
				#else
					#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
				#endif

                #pragma multi_compile _ _ADDITIONAL_LIGHTS
				#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile _ VF2_DEPTH_PREPASS VF2_DEPTH_PEELING
				#pragma multi_compile_local_fragment _ VF2_POINT_LIGHTS VF2_NATIVE_LIGHTS
				#pragma multi_compile_local_fragment _ VF2_RECEIVE_SHADOWS
				#pragma multi_compile_local_fragment VF2_SHAPE_BOX VF2_SHAPE_SPHERE
				#pragma multi_compile_local_fragment _ VF2_DETAIL_NOISE VF2_CONSTANT_DENSITY
				#pragma shader_feature_local_fragment VF2_DISTANCE
				#pragma shader_feature_local_fragment VF2_VOIDS
				#pragma shader_feature_local_fragment VF2_FOW
				#pragma shader_feature_local_fragment VF2_SURFACE
				#pragma shader_feature_local_fragment VF2_DEPTH_GRADIENT
				#pragma shader_feature_local_fragment VF2_HEIGHT_GRADIENT
				#pragma shader_feature_local_fragment VF2_LIGHT_COOKIE
				#pragma shader_feature_local_fragment VF2_DIFFUSION_SIMPLE VF2_DIFFUSION_SMOOTH VF2_DIFFUSION_STRONG
				#define UNITY_FOVEATED_RENDERING_INCLUDED
				#define _SURFACE_TYPE_TRANSPARENT

				#if UNITY_VERSION >= 202200
					#pragma multi_compile _ _FORWARD_PLUS
				#endif

				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
				#undef SAMPLE_TEXTURE2D
				#define SAMPLE_TEXTURE2D(textureName, samplerName, coord2) SAMPLE_TEXTURE2D_LOD(textureName, samplerName, coord2, 0)
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

				#include "CommonsURP.hlsl"

				#if UNITY_VERSION >= 202310
		            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
					#pragma shader_feature_local_fragment VF2_APV
				#endif

				#include "Primitives.cginc"
				#include "ShadowsURP.cginc"
				#include "Raymarch2D.cginc"
				#include "PointLights.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 pos     : SV_POSITION;
                    float3 wpos    : TEXCOORD0;
					float4 scrPos  : TEXCOORD1;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
				};

				int _ForcedInvisible;

				v2f vert(appdata v)
				{
					v2f o;

					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

					o.pos = TransformObjectToHClip(v.vertex.xyz);
				    o.wpos = TransformObjectToWorld(v.vertex.xyz);
					o.scrPos = ComputeScreenPos(o.pos);

					#if defined(UNITY_REVERSED_Z)
						o.pos.z = o.pos.w * UNITY_NEAR_CLIP_VALUE * 0.99995; //  0.99995 avoids precision issues on some Android devices causing unexpected clipping of light mesh
					#else
						o.pos.z = o.pos.w - 0.000005;
					#endif

					if (_ForcedInvisible == 1) {
						o.pos.xy = -10000;
                    }

					return o;
				}

				half4 frag(v2f i) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID(i);
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

					float3 rayStart = GetRayStart(i.wpos);
					float3 ray = i.wpos - rayStart;

                   	float t1 = length(ray);

					#if defined(FOG_ROTATION)
						float3 rayStartNonRotated = rayStart;
						float3 rayDirNonRotated = ray / t1;
						rayStart = RotateInv(rayStart);
						ray = mul((float3x3)_InvRotMatrix, ray);
						float3 rayDir = ray / t1;
					#else
						float3 rayDir = ray / t1;
						float3 rayStartNonRotated = rayStart;
						float3 rayDirNonRotated = rayDir;
					#endif

					#if VF2_SHAPE_SPHERE
						float t0;
						SphereIntersection(rayStart, rayDir, t0, t1);
					#else
						float t0 = BoxIntersection(rayStart, rayDir);
					#endif

					float2 uv = i.scrPos.xy / i.scrPos.w;

					CLAMP_RAY_DEPTH(rayStartNonRotated, uv, t1); // clamp to geometry

					#if VF2_DEPTH_PEELING
						CLAMP_RAY_START(rayStartNonRotated, uv, t0); // clamp to start of transparent objects
					#endif

					#if defined(FOG_MAX_DISTANCE_XZ)
						float slope = 1.0001 - abs(rayDir.y);
						FOG_MAX_LENGTH /= slope;
					#endif

					SetJitter(uv);

					t1 = min(t1, FOG_MAX_LENGTH); // max distance

                  	if (t0>=t1) return 0;

					float jiterring = jitter * JITTERING;
					t0 += jiterring;
					t1 += jiterring;

					half4 fogColor = GetFogColor(rayStart, rayDir, uv, t0, t1);

					// dither
					fogColor.rgb = max(0, fogColor.rgb - jitter * DITHERING);

					// alpha
					fogColor *= _LightColor.a;

					#if VF2_POINT_LIGHTS
						AddPointLights(rayStartNonRotated, rayDirNonRotated, fogColor, t0, t1 - t0);
					#endif

					#if defined(FOG_MAX_DISTANCE_XZ)
						float fallOffFactor = FOG_MAX_LENGTH * FOG_MAX_LENGTH_FALLOFF + 1.0;
						half maxDistanceFallOff = (FOG_MAX_LENGTH - t0) / fallOffFactor;
					#else
						half maxDistanceFallOff = (FOG_MAX_LENGTH - t0) / FOG_MAX_LENGTH_FALLOFF_PRECOMPUTED;
					#endif
					fogColor *= saturate(maxDistanceFallOff * maxDistanceFallOff * maxDistanceFallOff * maxDistanceFallOff);

					return fogColor;
				}
				ENDHLSL
			}

		}
}
