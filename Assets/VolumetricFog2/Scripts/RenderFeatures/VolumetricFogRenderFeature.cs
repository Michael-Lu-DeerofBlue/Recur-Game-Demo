//------------------------------------------------------------------------------------------------------------------
// Volumetric Fog & Mist 2
// Created by Kronnect
//------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_2023_3_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif
using UnityEngine.Rendering.Universal;

namespace VolumetricFogAndMist2 {

    public class VolumetricFogRenderFeature : ScriptableRendererFeature {

        public static class ShaderParams {
            public const string LightBufferName = "_LightBuffer";
            public static int LightBuffer = Shader.PropertyToID(LightBufferName);
            public static int LightBufferSize = Shader.PropertyToID("_VFRTSize");
            public static int MainTex = Shader.PropertyToID("_MainTex");
            public static int BlurRT = Shader.PropertyToID("_BlurTex");
            public static int BlurRT2 = Shader.PropertyToID("_BlurTex2");
            public static int MiscData = Shader.PropertyToID("_MiscData");
            public static int ForcedInvisible = Shader.PropertyToID("_ForcedInvisible");
            public static int DownsampledDepth = Shader.PropertyToID("_DownsampledDepth");
            public static int BlueNoiseTexture = Shader.PropertyToID("_BlueNoise");
            public static int BlurScale = Shader.PropertyToID("_BlurScale");
            public static int Downscaling = Shader.PropertyToID("_Downscaling");
            public static int ScatteringData = Shader.PropertyToID("_ScatteringData");
            public static int ScatteringTint = Shader.PropertyToID("_ScatteringTint");

            public static int BlurredTex = Shader.PropertyToID("_BlurredTex");

            public const string SKW_DITHER = "DITHER";
            public const string SKW_EDGE_PRESERVE = "EDGE_PRESERVE";
            public const string SKW_EDGE_PRESERVE_UPSCALING = "EDGE_PRESERVE_UPSCALING";
            public const string SKW_SCATTERING_HQ = "SCATTERING_HQ";
            public const string SKW_DEPTH_PEELING = "VF2_DEPTH_PEELING";
            public const string SKW_DEPTH_PREPASS = "VF2_DEPTH_PREPASS";
        }

        public static int GetScaledSize(int size, float factor) {
            size = (int)(size / factor);
            size /= 2;
            if (size < 1)
                size = 1;
            return size * 2;
        }

        class VolumetricFogRenderPass : ScriptableRenderPass {

            const string m_ProfilerTag = "Volumetric Fog Buffer Rendering";

            static FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.transparent, -1);
            static readonly List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
            RTHandle m_LightBuffer;
            VolumetricFogRenderFeature settings;

            public VolumetricFogRenderPass() {
                shaderTagIdList.Clear();
                shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                RenderTargetIdentifier lightBuffer = new RenderTargetIdentifier(ShaderParams.LightBuffer, 0, CubemapFace.Unknown, -1);
                m_LightBuffer = RTHandles.Alloc(lightBuffer, name: ShaderParams.LightBufferName);
            }

            public void CleanUp() {
                RTHandles.Release(m_LightBuffer);
            }

            public void Setup(VolumetricFogRenderFeature settings, RenderPassEvent renderPassEvent) {
                this.settings = settings;
                this.renderPassEvent = renderPassEvent;
            }

#if UNITY_2023_3_OR_NEWER
            [Obsolete]
#endif
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                RenderTextureDescriptor lightBufferDesc = cameraTextureDescriptor;
                VolumetricFogManager manager = VolumetricFogManager.GetManagerIfExists();
                if (manager != null) {
                    if (manager.downscaling > 1f) {
                        int size = GetScaledSize(cameraTextureDescriptor.width, manager.downscaling);
                        lightBufferDesc.width = size;
                        lightBufferDesc.height = size;
                    }
                    lightBufferDesc.colorFormat = manager.blurHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
                    cmd.SetGlobalVector(ShaderParams.LightBufferSize, new Vector4(lightBufferDesc.width, lightBufferDesc.height, manager.downscaling > 1f ? 1f : 0, 0));
                }
                lightBufferDesc.depthBufferBits = 0;
                lightBufferDesc.msaaSamples = 1;
                lightBufferDesc.useMipMap = false;

                cmd.GetTemporaryRT(ShaderParams.LightBuffer, lightBufferDesc, FilterMode.Bilinear);
                ConfigureTarget(m_LightBuffer);
                ConfigureClear(ClearFlag.Color, new Color(0, 0, 0, 0));
                ConfigureInput(ScriptableRenderPassInput.Depth);
            }

#if UNITY_2023_3_OR_NEWER
            [Obsolete]
#endif
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

                VolumetricFogManager manager = VolumetricFogManager.GetManagerIfExists();

                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                cmd.SetGlobalInt(ShaderParams.ForcedInvisible, 0);
                context.ExecuteCommandBuffer(cmd);

                bool usesDepthPeeling = manager.includeTransparent != 0 && manager.depthPeeling;
                if (manager == null || (manager.downscaling <= 1f && manager.blurPasses < 1 && manager.scattering <= 0 && !usesDepthPeeling)) {
                    CommandBufferPool.Release(cmd);
                    return;
                }

                foreach (VolumetricFog vg in VolumetricFog.volumetricFogs) {
                    if (vg != null) {
                        vg.meshRenderer.renderingLayerMask = VolumetricFogManager.FOG_VOLUMES_RENDERING_LAYER;
                    }
                }

                if (usesDepthPeeling) {
                    cmd.Clear();
                    if (renderPassEvent < RenderPassEvent.AfterRenderingTransparents) {
                        cmd.DisableShaderKeyword(ShaderParams.SKW_DEPTH_PREPASS);
                        cmd.EnableShaderKeyword(ShaderParams.SKW_DEPTH_PEELING);
                    } else {
                        cmd.DisableShaderKeyword(ShaderParams.SKW_DEPTH_PEELING);
                        cmd.EnableShaderKeyword(ShaderParams.SKW_DEPTH_PREPASS);
                    }
                    context.ExecuteCommandBuffer(cmd);
                }
                var sortFlags = SortingCriteria.CommonTransparent;
                var drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, sortFlags);
                var filterSettings = filteringSettings;
                filterSettings.layerMask = settings.fogLayerMask;
                filterSettings.renderingLayerMask = VolumetricFogManager.FOG_VOLUMES_RENDERING_LAYER;

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filterSettings);

                CommandBufferPool.Release(cmd);

            }

#if UNITY_2023_3_OR_NEWER

            class PassData {
                public RendererListHandle rendererListHandle;
                public UniversalCameraData cameraData;
                public RenderPassEvent renderPassEvent;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {

                using (var builder = renderGraph.AddUnsafePass<PassData>(m_ProfilerTag, out var passData)) {
                    builder.AllowPassCulling(false);

                    UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                    UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
                    UniversalLightData lightData = frameData.Get<UniversalLightData>();
                    UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                    passData.cameraData = cameraData;
                    passData.renderPassEvent = renderPassEvent;

                    builder.UseTexture(resourceData.activeDepthTexture, AccessFlags.Read);
                    ConfigureInput(ScriptableRenderPassInput.Depth);

                    SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
                    var drawingSettings = CreateDrawingSettings(shaderTagIdList, renderingData, cameraData, lightData, sortingCriteria);
                    var filterSettings = filteringSettings;
                    filterSettings.layerMask = settings.fogLayerMask;
                    filterSettings.renderingLayerMask = VolumetricFogManager.FOG_VOLUMES_RENDERING_LAYER;
                    RendererListParams listParams = new RendererListParams(renderingData.cullResults, drawingSettings, filterSettings);
                    passData.rendererListHandle = renderGraph.CreateRendererList(listParams);
                    builder.UseRendererList(passData.rendererListHandle);

                    builder.SetRenderFunc((PassData passData, UnsafeGraphContext context) => {

                        CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

                        bool usesDepthPeeling = false;
                        RenderTextureDescriptor lightBufferDesc = passData.cameraData.cameraTargetDescriptor;
                        VolumetricFogManager manager = VolumetricFogManager.GetManagerIfExists();
                        if (manager != null) {
                            if (manager.downscaling > 1f) {
                                int size = GetScaledSize(lightBufferDesc.width, manager.downscaling);
                                lightBufferDesc.width = size;
                                lightBufferDesc.height = size;
                            }
                            lightBufferDesc.colorFormat = manager.blurHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
                            cmd.SetGlobalVector(ShaderParams.LightBufferSize, new Vector4(lightBufferDesc.width, lightBufferDesc.height, manager.downscaling > 1f ? 1f : 0, 0));

                            usesDepthPeeling = manager.includeTransparent != 0 && manager.depthPeeling;
                        }
                        lightBufferDesc.depthBufferBits = 0;
                        lightBufferDesc.msaaSamples = 1;
                        lightBufferDesc.useMipMap = false;

                        cmd.GetTemporaryRT(ShaderParams.LightBuffer, lightBufferDesc, FilterMode.Bilinear);
                        RenderTargetIdentifier rti = new RenderTargetIdentifier(ShaderParams.LightBuffer, 0, CubemapFace.Unknown, -1);
                        cmd.SetRenderTarget(rti);
                        cmd.ClearRenderTarget(false, true, new Color(0, 0, 0, 0));

                        cmd.SetGlobalInt(ShaderParams.ForcedInvisible, 0);
                        if (manager == null || (manager.downscaling <= 1f && manager.blurPasses < 1 && manager.scattering <= 0 && !usesDepthPeeling)) {
                            return;
                        }

                        foreach (VolumetricFog vg in VolumetricFog.volumetricFogs) {
                            if (vg != null) {
                                vg.meshRenderer.renderingLayerMask = VolumetricFogManager.FOG_VOLUMES_RENDERING_LAYER;
                            }
                        }

                        if (usesDepthPeeling) {
                            if (passData.renderPassEvent < RenderPassEvent.AfterRenderingTransparents) {
                                cmd.DisableShaderKeyword(ShaderParams.SKW_DEPTH_PREPASS);
                                cmd.EnableShaderKeyword(ShaderParams.SKW_DEPTH_PEELING);
                            } else {
                                cmd.DisableShaderKeyword(ShaderParams.SKW_DEPTH_PEELING);
                                cmd.EnableShaderKeyword(ShaderParams.SKW_DEPTH_PREPASS);
                            }
                        }

                        context.cmd.DrawRendererList(passData.rendererListHandle);
                    });
                }
            }
#endif

        }


        class BlurRenderPass : ScriptableRenderPass {

            enum Pass {
                BlurHorizontal = 0,
                BlurVertical = 1,
                BlurVerticalAndBlend = 2,
                UpscalingBlend = 3,
                DownscaleDepth = 4,
                BlurVerticalFinal = 5,
                Resample = 6,
                ResampleAndCombine = 7,
                ScatteringPrefilter = 8,
                ScatteringBlend = 9,
                Blend = 10
            }

            class PassData {
#if UNITY_2022_3_OR_NEWER
                public RTHandle source;
#else
                public RenderTargetIdentifier source;
#endif
#if UNITY_2023_3_OR_NEWER
                public TextureHandle colorTexture;
                public UniversalCameraData cameraData;
#endif
                public RenderPassEvent renderPassEvent;
            }


            const string m_ProfilerTag = "Volumetric Fog Render Feature";
            ScriptableRenderer renderer;
            static Material mat;
            static RenderTextureDescriptor sourceDesc;
            static VolumetricFogManager manager;
            static readonly PassData passData = new PassData();

            public void Setup(Shader shader, ScriptableRenderer renderer, RenderPassEvent renderPassEvent) {
                this.renderPassEvent = renderPassEvent;
                this.renderer = renderer;
                manager = VolumetricFogManager.GetManagerIfExists();
                if (mat == null) {
                    mat = CoreUtils.CreateEngineMaterial(shader);
                    Texture2D noiseTex = Resources.Load<Texture2D>("Textures/blueNoiseVF128");
                    mat.SetTexture(ShaderParams.BlueNoiseTexture, noiseTex);
                }
            }

#if UNITY_2023_3_OR_NEWER
            [Obsolete]
#endif
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                sourceDesc = cameraTextureDescriptor;
                ConfigureInput(ScriptableRenderPassInput.Depth);
            }

#if UNITY_2023_3_OR_NEWER
            [Obsolete]
#endif
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

#if UNITY_2022_1_OR_NEWER
                passData.source = renderer.cameraColorTargetHandle;
#else
                passData.source = renderer.cameraColorTarget;
#endif
                passData.renderPassEvent = renderPassEvent;
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                ExecutePass(passData, cmd);
                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);

            }

#if UNITY_2023_3_OR_NEWER

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {

                using (var builder = renderGraph.AddUnsafePass<PassData>(m_ProfilerTag, out var passData)) {
                    builder.AllowPassCulling(false);

                    UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                    UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
                    UniversalLightData lightData = frameData.Get<UniversalLightData>();
                    UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                    passData.cameraData = cameraData;
                    passData.colorTexture = resourceData.activeColorTexture;
                    builder.UseTexture(resourceData.activeColorTexture, AccessFlags.ReadWrite);
                    builder.UseTexture(resourceData.activeDepthTexture, AccessFlags.Read);

                    ConfigureInput(ScriptableRenderPassInput.Depth);
                    passData.renderPassEvent = renderPassEvent;

                    sourceDesc = cameraData.cameraTargetDescriptor;

                    builder.SetRenderFunc((PassData passData, UnsafeGraphContext context) => {
                        CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                        passData.source = passData.colorTexture;
                        ExecutePass(passData, cmd);
                    });
                }
            }
#endif

            static void ExecutePass(PassData passData, CommandBuffer cmd) {

                bool usesDepthPeeling = manager.includeTransparent != 0 && manager.depthPeeling;
                if (manager == null || (manager.downscaling <= 1f && manager.blurPasses < 1 && manager.scattering <= 0 && !usesDepthPeeling)) {
                    Cleanup();
                    return;
                }

                mat.SetVector(ShaderParams.MiscData, new Vector4(manager.ditherStrength * 0.1f, 0, manager.blurEdgeDepthThreshold, manager.downscalingEdgeDepthThreshold * 0.001f));
                if (manager.ditherStrength > 0) {
                    mat.EnableKeyword(ShaderParams.SKW_DITHER);
                } else {
                    mat.DisableKeyword(ShaderParams.SKW_DITHER);
                }
                mat.DisableKeyword(ShaderParams.SKW_EDGE_PRESERVE);
                mat.DisableKeyword(ShaderParams.SKW_EDGE_PRESERVE_UPSCALING);
                if (manager.blurPasses > 0 && manager.blurEdgePreserve) {
                    mat.EnableKeyword(manager.downscaling > 1f ? ShaderParams.SKW_EDGE_PRESERVE_UPSCALING : ShaderParams.SKW_EDGE_PRESERVE);
                }

#if UNITY_2022_3_OR_NEWER
                RTHandle source = passData.source;
#else
                RenderTargetIdentifier source = passData.source;
#endif

                cmd.SetGlobalInt(ShaderParams.ForcedInvisible, 1);

                RenderTextureDescriptor rtBlurDesc = sourceDesc;
                rtBlurDesc.width = GetScaledSize(sourceDesc.width, manager.downscaling);
                rtBlurDesc.height = GetScaledSize(sourceDesc.height, manager.downscaling);
                rtBlurDesc.useMipMap = false;
                rtBlurDesc.colorFormat = manager.blurHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
                rtBlurDesc.msaaSamples = 1;
                rtBlurDesc.depthBufferBits = 0;

                bool usingDownscaling = manager.downscaling > 1f;
                bool isFrontDepthPeeling = usesDepthPeeling && passData.renderPassEvent >= RenderPassEvent.AfterRenderingTransparents;
                if (usingDownscaling && !isFrontDepthPeeling) {
                    RenderTextureDescriptor rtDownscaledDepth = rtBlurDesc;
                    rtDownscaledDepth.colorFormat = RenderTextureFormat.RFloat;
                    cmd.GetTemporaryRT(ShaderParams.DownsampledDepth, rtDownscaledDepth, FilterMode.Bilinear);
                    FullScreenBlit(cmd, source, ShaderParams.DownsampledDepth, mat, (int)Pass.DownscaleDepth);
                }

                if (usesDepthPeeling) {
                    mat.EnableKeyword(ShaderParams.SKW_DEPTH_PEELING);
                } else {
                    mat.DisableKeyword(ShaderParams.SKW_DEPTH_PEELING);
                }

                if (manager.blurPasses < 1) {
                    // no blur but downscaling
                    FullScreenBlit(cmd, ShaderParams.LightBuffer, source, mat, usingDownscaling ? (int)Pass.UpscalingBlend : (int)Pass.Blend);
                } else {
                    // blur (with or without downscaling)
                    rtBlurDesc.width = GetScaledSize(sourceDesc.width, manager.blurDownscaling);
                    rtBlurDesc.height = GetScaledSize(sourceDesc.height, manager.blurDownscaling);
                    cmd.GetTemporaryRT(ShaderParams.BlurRT, rtBlurDesc, FilterMode.Bilinear);
                    cmd.GetTemporaryRT(ShaderParams.BlurRT2, rtBlurDesc, FilterMode.Bilinear);
                    cmd.SetGlobalFloat(ShaderParams.BlurScale, manager.blurSpread * manager.blurDownscaling);
                    FullScreenBlit(cmd, ShaderParams.LightBuffer, ShaderParams.BlurRT, mat, (int)Pass.BlurHorizontal);
                    cmd.SetGlobalFloat(ShaderParams.BlurScale, manager.blurSpread);
                    for (int k = 0; k < manager.blurPasses - 1; k++) {
                        FullScreenBlit(cmd, ShaderParams.BlurRT, ShaderParams.BlurRT2, mat, (int)Pass.BlurVertical);
                        FullScreenBlit(cmd, ShaderParams.BlurRT2, ShaderParams.BlurRT, mat, (int)Pass.BlurHorizontal);
                    }
                    if (usingDownscaling) {
                        FullScreenBlit(cmd, ShaderParams.BlurRT, ShaderParams.BlurRT2, mat, (int)Pass.BlurVerticalFinal);
                        FullScreenBlit(cmd, ShaderParams.BlurRT2, source, mat, (int)Pass.UpscalingBlend);
                    } else {
                        FullScreenBlit(cmd, ShaderParams.BlurRT, source, mat, (int)Pass.BlurVerticalAndBlend);
                    }

                    cmd.ReleaseTemporaryRT(ShaderParams.BlurRT2);
                    cmd.ReleaseTemporaryRT(ShaderParams.BlurRT);
                }

                if (manager.scattering > 0 && (!usesDepthPeeling || isFrontDepthPeeling)) {
                    ComputeScattering(cmd, source, mat);
                }

                cmd.ReleaseTemporaryRT(ShaderParams.LightBuffer);
                if (usingDownscaling) {
                    cmd.ReleaseTemporaryRT(ShaderParams.DownsampledDepth);
                }
            }


            struct ScatteringMipData {
                public int rtDown, rtUp, width, height;
            }
            static ScatteringMipData[] rt;
            const int PYRAMID_MAX_LEVELS = 5;


#if UNITY_2022_1_OR_NEWER
            static void ComputeScattering(CommandBuffer cmd, RTHandle source, Material mat) {
#else
            static void ComputeScattering(CommandBuffer cmd, RenderTargetIdentifier source, Material mat) {
#endif

                mat.SetVector(ShaderParams.ScatteringData, new Vector4(manager.scatteringThreshold, manager.scatteringIntensity, 1f - manager.scatteringAbsorption, manager.scattering));
                mat.SetColor(ShaderParams.ScatteringTint, manager.scatteringTint);
                float downscaling = manager.downscaling;

                // Initialize buffers descriptors
                if (rt == null || rt.Length != PYRAMID_MAX_LEVELS + 1) {
                    rt = new ScatteringMipData[PYRAMID_MAX_LEVELS + 1];
                    for (int k = 0; k < rt.Length; k++) {
                        rt[k].rtDown = Shader.PropertyToID("_VFogDownMip" + k);
                        rt[k].rtUp = Shader.PropertyToID("_VFogUpMip" + k);
                    }
                }

                int width = GetScaledSize(sourceDesc.width, downscaling);
                int height = GetScaledSize(sourceDesc.height, downscaling);
                if (downscaling > 1 && manager.scatteringHighQuality) {
                    mat.EnableKeyword(ShaderParams.SKW_SCATTERING_HQ);
                } else {
                    mat.DisableKeyword(ShaderParams.SKW_SCATTERING_HQ);
                }
                if (!manager.scatteringHighQuality) {
                    width /= 2;
                    height /= 2;
                }
                int mipCount = manager.scatteringHighQuality ? 5 : 4;
                RenderTextureDescriptor scatterDesc = sourceDesc;
                scatterDesc.colorFormat = RenderTextureFormat.ARGBHalf;
                scatterDesc.msaaSamples = 1;
                scatterDesc.depthBufferBits = 0;
                for (int k = 0; k <= mipCount; k++) {
                    if (width < 2) width = 2;
                    if (height < 2) height = 2;
                    scatterDesc.width = rt[k].width = width;
                    scatterDesc.height = rt[k].height = height;
                    cmd.GetTemporaryRT(rt[k].rtDown, scatterDesc, FilterMode.Bilinear);
                    cmd.GetTemporaryRT(rt[k].rtUp, scatterDesc, FilterMode.Bilinear);
                    width /= 2;
                    height /= 2;
                }

                RenderTargetIdentifier sourceMip = rt[0].rtDown;

                FullScreenBlit(cmd, source, sourceMip, mat, (int)Pass.ScatteringPrefilter);

                // Blitting down...
                cmd.SetGlobalFloat(ShaderParams.BlurScale, 1f);
                for (int k = 1; k <= mipCount; k++) {
                    FullScreenBlit(cmd, sourceMip, rt[k].rtDown, mat, (int)Pass.Resample);
                    sourceMip = rt[k].rtDown;
                }

                // Blitting up...
                cmd.SetGlobalFloat(ShaderParams.BlurScale, 1.5f);
                for (int k = mipCount; k > 0; k--) {
                    cmd.SetGlobalTexture(ShaderParams.BlurredTex, rt[k - 1].rtDown);
                    FullScreenBlit(cmd, sourceMip, rt[k - 1].rtUp, mat, (int)Pass.ResampleAndCombine);
                    sourceMip = rt[k - 1].rtUp;
                }

                FullScreenBlit(cmd, sourceMip, source, mat, (int)Pass.ScatteringBlend);
            }

            static void FullScreenBlit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int passIndex) {
                destination = new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1);
                cmd.SetRenderTarget(destination);
                cmd.SetGlobalTexture(ShaderParams.MainTex, source);
                cmd.DrawMesh(Tools.fullscreenMesh, Matrix4x4.identity, material, 0, passIndex);
            }

            static public void Cleanup() {
                Shader.SetGlobalInt(ShaderParams.ForcedInvisible, 0);
            }

        }

        [SerializeField, HideInInspector]
        Shader blurShader;
        VolumetricFogRenderPass fogRenderPass, fogRenderBackTranspPass;
        BlurRenderPass blurRenderPass, blurRenderBackTranspPass;
        public static bool installed;
        public static bool isRenderingBeforeTransparents;

        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;

        [Tooltip("Specify which fog volumes will be rendered by this feature.")]
        public LayerMask fogLayerMask = -1;

        [Tooltip("Specify which cameras can execute this render feature. If you have several cameras in your scene, make sure only the correct cameras use this feature in order to optimize performance.")]
        public LayerMask cameraLayerMask = -1;

        [Tooltip("Ignores reflection probes from executing this render feature")]
        public bool ignoreReflectionProbes = true;

        void OnDisable() {
            installed = false;
            isRenderingBeforeTransparents = false;
            BlurRenderPass.Cleanup();
        }

        private void OnDestroy() {
            if (fogRenderPass != null) {
                fogRenderPass.CleanUp();
            }
            if (fogRenderBackTranspPass != null) {
                fogRenderBackTranspPass.CleanUp();
            }
        }

        public override void Create() {
            name = "Volumetric Fog 2";
            fogRenderPass = new VolumetricFogRenderPass();
            blurRenderPass = new BlurRenderPass();
            fogRenderBackTranspPass = new VolumetricFogRenderPass();
            blurRenderBackTranspPass = new BlurRenderPass();
            blurShader = Shader.Find("Hidden/VolumetricFog2/Blur");
            if (blurShader == null) {
                Debug.LogWarning("Could not load Volumetric Fog composition shader.");
            }
        }

        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {

            installed = true;

            if (VolumetricFog.volumetricFogs.Count == 0) return;

            VolumetricFogManager manager = VolumetricFogManager.GetManagerIfExists();
            bool usesDepthPeeling = manager.includeTransparent != 0 && manager.depthPeeling;
            if (manager == null || (manager.downscaling <= 1f && manager.blurPasses < 1 && manager.scattering <= 0 && !usesDepthPeeling)) {
                Shader.SetGlobalInt(ShaderParams.ForcedInvisible, 0);
                return;
            }

            Camera cam = renderingData.cameraData.camera;

            CameraType camType = cam.cameraType;
            if (camType == CameraType.Preview) return;
            if (ignoreReflectionProbes && camType == CameraType.Reflection) return;

            if ((fogLayerMask & cam.cullingMask) == 0) return;

            if ((cameraLayerMask & (1 << cam.gameObject.layer)) == 0) return;

            if (cam.targetTexture != null && cam.targetTexture.format == RenderTextureFormat.Depth) return; // ignore occlusion cams!

            if (usesDepthPeeling) {
                fogRenderBackTranspPass.Setup(this, RenderPassEvent.BeforeRenderingTransparents);
                blurRenderBackTranspPass.Setup(blurShader, renderer, RenderPassEvent.BeforeRenderingTransparents);
                renderer.EnqueuePass(fogRenderBackTranspPass);
                renderer.EnqueuePass(blurRenderBackTranspPass);
                fogRenderPass.Setup(this, RenderPassEvent.AfterRenderingTransparents);
                blurRenderPass.Setup(blurShader, renderer, RenderPassEvent.AfterRenderingTransparents);
                isRenderingBeforeTransparents = false;
            } else {
                fogRenderPass.Setup(this, renderPassEvent);
                blurRenderPass.Setup(blurShader, renderer, renderPassEvent);
                isRenderingBeforeTransparents = renderPassEvent < RenderPassEvent.AfterRenderingTransparents;
            }
            renderer.EnqueuePass(fogRenderPass);
            renderer.EnqueuePass(blurRenderPass);
        }
    }
}
