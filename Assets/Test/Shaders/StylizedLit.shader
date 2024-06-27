Shader "Quibli/Stylized Lit"
{
    Properties
    {
        [Gradient] _GradientRamp("Gradient", 2D) = "white" {}

        _SelfShadingSize ("[]Shading Offset", Range(0, 1.0)) = 0.0

        [Space(10)]
        [Toggle(DR_SPECULAR_ON)] _SpecularEnabled("Enable Specular", Int) = 0
        [HDR] _FlatSpecularColor("[DR_SPECULAR_ON]Specular Color", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        _FlatSpecularSize("[DR_SPECULAR_ON]Specular Size", Range(0.0, 1.0)) = 0.1
        _FlatSpecularEdgeSmoothness("[DR_SPECULAR_ON]Specular Edge Smoothness", Range(0.0, 1.0)) = 0

        [Space(10)]
        [Toggle(DR_RIM_ON)] _RimEnabled("Enable Rim", Int) = 0
        [HDR] _FlatRimColor("[DR_RIM_ON]Rim Color", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        _FlatRimLightAlign("[DR_RIM_ON]Light Align", Range(0.0, 1.0)) = 0
        _FlatRimSize("[DR_RIM_ON]Rim Size", Range(0, 1)) = 0.5
        _FlatRimEdgeSmoothness("[DR_RIM_ON]Rim Edge Smoothness", Range(0, 1)) = 0.5

        [Space(10)]
        [Toggle(DR_GRADIENT_ON)] _GradientEnabled("Enable Height Gradient", Int) = 0
        [HideInInspector][MainColor] _BaseColor ("Color", Color) = (1,1,1,1)
        _ColorGradient("[DR_GRADIENT_ON]Gradient Color", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        _GradientCenterX("[DR_GRADIENT_ON]Center X", Float) = 0
        _GradientCenterY("[DR_GRADIENT_ON]Center Y", Float) = 0
        _GradientSize("[DR_GRADIENT_ON]Size", Float) = 10.0
        _GradientAngle("[DR_GRADIENT_ON]Gradient Angle", Range(0, 360)) = 0

        [Space(10)]
        [Toggle(DR_VERTEX_COLORS_ON)] _VertexColorsEnabled("Enable Vertex Colors", Int) = 0

        [Space]
        [Toggle(DR_OUTLINE_ON)] _OutlineEnabled("Enable Outline", Int) = 0
        _OutlineColor("[DR_OUTLINE_ON]Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("[DR_OUTLINE_ON]Width", Float) = 1.0
        _OutlineScale("[DR_OUTLINE_ON]Scale", Float) = 1.0
        _OutlineDepthOffset("[DR_OUTLINE_ON]Depth Offset", Range(0, 1)) = 0.0
        _CameraDistanceImpact("[DR_OUTLINE_ON]Camera Distance Impact", Range(0, 1)) = 0.5

        [MainTexture] _BaseMap("[FOLDOUT(Texture maps){11}]Albedo", 2D) = "black" {}
    	[Toggle(_BASEMAP_PREMULTIPLY)]_BaseMapPremultiply("[_]Mix Into Shading", Int) = 0
        [KeywordEnum(Multiply, Add)]_TextureBlendingMode("[]Blending Mode", Float) = 0
        _TextureImpact("[]Texture Impact", Range(0, 1)) = 1.0

        _DetailMap("Detail Map", 2D) = "black" {}
        _DetailMapColor("[]Detail Color", Color) = (1,1,1,1)
        [KeywordEnum(Multiply, Add, Interpolate)]_DetailMapBlendingMode("[]Blending Mode", Float) = 0
        _DetailMapImpact("[]Detail Impact", Range(0, 1)) = 0.0

        _BumpMap ("Normal Map", 2D) = "bump" {}

        [Space(15)]
        [HideInInspector][HDR]_EmissionColor("Emission Color", Color) = (0,0,0)
        [HideInInspector][NoScaleOffset]_EmissionMap("Emission Map", 2D) = "white" {}

        _LightContribution("[FOLDOUT(Lighting){11}]Light Color Impact", Range(0, 1)) = 1
        [Space][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 0
        [Toggle(_UNITYSHADOW_OCCLUSION)]_UnityShadowOcclusion("[_RECEIVE_SHADOWS_OFF]Shadow Occlusion", Int) = 0
        [Space][Toggle(DR_LIGHT_ATTENUATION)]_OverrideLightAttenuation("Customize Light", Int) = 0
        [MinMax]_LightAttenuation("[DR_LIGHT_ATTENUATION][]Attenuation Remap", Vector) = (0, 1, 0, 0)
        _ShadowColor("[DR_LIGHT_ATTENUATION][]Shadow Color", Color) = (0, 0, 0, 1)
        [Space][Toggle(DR_BAKED_GI)]_OverrideBakedGi("Override Baked GI", Int) = 0
        [Gradient]_BakedGIRamp("[DR_BAKED_GI]Baked Light Lookup", 2D) = "transparent" {}

        [Space]
        [Toggle(DR_ENABLE_LIGHTMAP_DIR)]_OverrideLightmapDir("Override Light Direction", Int) = 0
        _LightmapDirectionPitch("[DR_ENABLE_LIGHTMAP_DIR][]Pitch", Range(0, 360)) = 0
        _LightmapDirectionYaw("[DR_ENABLE_LIGHTMAP_DIR][]Yaw", Range(0, 360)) = 0
        [HideInInspector] _LightmapDirection("Direction", Vector) = (0, 1, 0, 0)

        [HideInInspector]_Cutoff ("Base Alpha cutoff", Range (0, 1)) = .5

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0

        // Editmode props
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0

        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"
        }
        LOD 300

    	HLSLINCLUDE
    	// #define DR_DOTS_INSTANCING_ON // Uncomment to enable DOTS instancing
    	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Version.hlsl"
    	ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForwardOnly"
            }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #pragma shader_feature_local DR_LIGHT_ATTENUATION
            #pragma shader_feature_local DR_BAKED_GI
            #pragma shader_feature_local DR_GRADIENT_ON
            #pragma shader_feature_local DR_SPECULAR_ON
            #pragma shader_feature_local DR_RIM_ON
            #pragma shader_feature_local DR_VERTEX_COLORS_ON
            #pragma shader_feature_local DR_ENABLE_LIGHTMAP_DIR
            #pragma shader_feature_local _TEXTUREBLENDINGMODE_MULTIPLY _TEXTUREBLENDINGMODE_ADD
            #pragma shader_feature_local _UNITYSHADOW_OCCLUSION
            #pragma shader_feature_local _BASEMAP_PREMULTIPLY

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
            // #pragma shader_feature_local_fragment _ _SPECGLOSSMAP _SPECULAR_COLOR
            // #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

            // -------------------------------------
            // Universal Pipeline keywords
            #if VERSION_GREATER_EQUAL(11, 0)
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #else
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #endif
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #if VERSION_GREATER_EQUAL(12, 0)
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING
            #endif
            #if UNITY_VERSION >= 202220
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
            #endif

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #if UNITY_VERSION >= 202220
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            #endif

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #if defined(DR_DOTS_INSTANCING_ON)
            #pragma target 4.5							// Uncomment to enable DOTs instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON	// Uncomment to enable DOTs instancing
            #endif

            #define BUMP_SCALE_NOT_SUPPORTED 1
            #define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR 1
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_GradientRamp);
            SAMPLER(sampler_GradientRamp);

            TEXTURE2D(_BakedGIRamp);
            SAMPLER(sampler_BakedGIRamp);

            // Detail map.
            #pragma shader_feature_local_fragment _DETAILMAPBLENDINGMODE_MULTIPLY _DETAILMAPBLENDINGMODE_ADD _DETAILMAPBLENDINGMODE_INTERPOLATE

            TEXTURE2D(_DetailMap);
            SAMPLER(sampler_DetailMap);

            #pragma vertex LitPassVertex
            #pragma fragment StylizedPassFragment

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "LibraryUrp/LitForwardPass_DR.hlsl"
            #include "LibraryUrp/QuibliVertex.hlsl"
            #include "LibraryUrp/Lighting_DR.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            Tags
            {
                "LightMode" = "SRPDefaultUnlit"
            }

            Cull Front

            HLSLPROGRAM
            #include "LibraryUrp/StylizedInput.hlsl"

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            #pragma multi_compile _ DR_OUTLINE_ON
            #pragma multi_compile_fog

            struct VertexInput
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float3 normal : NORMAL;

                float fogCoord : TEXCOORD1;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 ObjectToClipPos(float4 pos) {
                return mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4(pos.xyz, 1)));
            }

            VertexOutput VertexProgram(VertexInput v) {
                VertexOutput o;

                UNITY_SETUP_INSTANCE_ID(v);

                o = (VertexOutput)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                #if defined(DR_OUTLINE_ON)
                float4 clipPosition = ObjectToClipPos(v.position * _OutlineScale);
                const float3 clipNormal = mul((float3x3)UNITY_MATRIX_VP, mul((float3x3)UNITY_MATRIX_M, v.normal));
                const half outlineWidth = _OutlineWidth;
                const half cameraDistanceImpact = lerp(clipPosition.w, 4.0, _CameraDistanceImpact);
                const float2 aspectRatio = float2(_ScreenParams.x / _ScreenParams.y, 1);
                const float2 offset = normalize(clipNormal.xy) / aspectRatio * outlineWidth * cameraDistanceImpact * 0.005;
                clipPosition.xy += offset;
                const half outlineDepthOffset = _OutlineDepthOffset;

                #if UNITY_REVERSED_Z
                clipPosition.z -= outlineDepthOffset * 0.1;
                #else
                clipPosition.z += outlineDepthOffset * 0.1 * (1.0 - UNITY_NEAR_CLIP_VALUE);
                #endif

                o.position = clipPosition;
                o.normal = clipNormal;

                o.fogCoord = ComputeFogFactor(o.position.z);
                #endif

                return o;
            }

            half4 FragmentProgram(VertexOutput i) : SV_TARGET {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                half4 color = _OutlineColor;
                color.rgb = MixFog(color.rgb, i.fogCoord);
                return color;
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #if defined(DR_DOTS_INSTANCING_ON)
            #pragma target 4.5							// Uncomment to enable DOTs instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON	// Uncomment to enable DOTs instancing
            #endif

            // -------------------------------------
            // Universal Pipeline keywords

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            
            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "GBuffer"
            Tags{"LightMode" = "UniversalGBuffer"}

            ZWrite[_ZWrite]
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM
            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            // #pragma shader_feature _ALPHAPREMULTIPLY_ON
            // #pragma shader_feature_local_fragment _ _SPECGLOSSMAP _SPECULAR_COLOR
            // #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_LAYERS

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
            #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #if defined(DR_DOTS_INSTANCING_ON)
            #pragma target 4.5							// Uncomment to enable DOTs instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON	// Uncomment to enable DOTs instancing
            #endif

            #pragma vertex LitPassVertexSimple
            #pragma fragment LitPassFragmentSimple

            #define BUMP_SCALE_NOT_SUPPORTED 1

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitGBufferPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            // #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            // Universal Pipeline keywords
            #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #if defined(FLAT_KIT_DOTS_INSTANCING_ON)
            #pragma target 4.5							// Uncomment to enable DOTs instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON	// Uncomment to enable DOTs instancing
            #endif

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }

            Cull Off

            HLSLPROGRAM
            #define _SpecColor _BaseColor
            #define _SpecGlossMap _BaseMap
            #define sampler_SpecGlossMap sampler_BaseMap

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaSimple
            #pragma shader_feature EDITOR_VISUALIZATION

            #pragma shader_feature_local_fragment _EMISSION
            // #pragma shader_feature_local_fragment _SPECGLOSSMAP

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitMetaPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Universal2D"
            Tags
            {
                "LightMode" = "Universal2D"
            }
            Tags
            {
                "RenderType" = "Transparent" "Queue" = "Transparent"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

            #include "LibraryUrp/StylizedInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Universal2D.hlsl"
            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "Quibli.QuibliEditor"
}