using UnityEngine;
using UnityEngine.Rendering;

namespace Kamgam.SettingsGenerator
{
    public partial class QualityPreset
    {
        /// <summary>
        /// Budget for how many ray casts can be performed per frame for approximate collision
        //  testing.
        /// </summary>
        public int particleRaycastBudget;

        /// <summary>
        /// Use a two-pass shader for the vegetation in the terrain engine.
        /// </summary>
        public bool softVegetation;

        /// <summary>
        /// The VSync Count.
        /// </summary>
        public int vSyncCount;

        /// <summary>
        /// Choose the level of Multi-Sample Anti-aliasing (MSAA) that the GPU performs.
        /// </summary>
        public int antiAliasing;

        /// <summary>
        /// Async texture upload provides timesliced async texture upload on the render thread
        /// with tight control over memory and timeslicing. There are no allocations except
        /// for the ones which driver has to do. To read data and upload texture data a ringbuffer
        /// whose size can be controlled is re-used. Use asyncUploadTimeSlice to set the
        /// time-slice in milliseconds for asynchronous texture uploads per frame. Minimum
        /// value is 1 and maximum is 33.
        /// </summary>
        public int asyncUploadTimeSlice;

        /// <summary>
        /// Asynchronous texture and mesh data upload provides timesliced async texture and
        /// mesh data upload on the render thread with tight control over memory and timeslicing.
        /// There are no allocations except for the ones which driver has to do. To read
        /// data and upload texture and mesh data, Unity re-uses a ringbuffer whose size
        /// can be controlled. Use asyncUploadBufferSize to set the buffer size for asynchronous
        /// texture and mesh data uploads. The size is in megabytes. The minimum value is
        /// 2 and the maximum value is 512. The buffer resizes automatically to fit the largest
        /// texture currently loading. To avoid re-sizing of the buffer, which can incur
        /// performance cost, set the value approximately to the size of biggest texture
        /// used in the Scene.
        /// </summary>
        public int asyncUploadBufferSize;

        /// <summary>
        /// This flag controls if the async upload pipeline's ring buffer remains allocated
        /// when there are no active loading operations. Set this to true, to make the ring
        /// buffer allocation persist after all upload operations have completed. If you
        /// have issues with excessive memory usage, you can set this to false. This means
        /// you reduce the runtime memory footprint, but memory fragmentation can occur.
        /// The default value is true.
        /// </summary>
        public bool asyncUploadPersistentBuffer;

        /// <summary>
        /// Enables realtime reflection probes.
        /// </summary>
        public bool realtimeReflectionProbes;

        /// <summary>
        /// If enabled, billboards will face towards camera position rather than camera orientation.
        /// </summary>
        public bool billboardsFaceCameraPosition;

        /// <summary>
        /// In resolution scaling mode, this factor is used to multiply with the target Fixed
        //  DPI specified to get the actual Fixed DPI to use for this quality setting.
        /// </summary>
        public float resolutionScalingFixedDPIFactor;

        /// <summary>
        /// Should soft blending be used for particles?
        /// </summary>
        public bool softParticles;

        /// <summary>
        /// The RenderPipelineAsset for this quality level
        /// </summary>
        public RenderPipelineAsset renderPipeline;

        /// <summary>
        /// The maximum number of bones per vertex that are taken into account during skinning,
        /// for all meshes in the project.
        /// </summary>
        public SkinWeights skinWeights;

        /// <summary>
        /// Enable automatic streaming of texture mipmap levels based on their distance from
        /// all active cameras.
        /// </summary>
        public bool streamingMipmapsActive;

        /// <summary>
        /// The total amount of memory to be used by streaming and non-streaming textures.
        /// </summary>
        public float streamingMipmapsMemoryBudget;

        /// <summary>
        /// The number of renderer instances that are processed each frame when calculating 
        /// which texture mipmap levels should be streamed.
        /// </summary>
        public int streamingMipmapsRenderersPerFrame;

        /// <summary>
        /// The maximum number of mipmap levels to discard for each texture.
        /// </summary>
        public int streamingMipmapsMaxLevelReduction;

        /// <summary>
        /// Process all enabled Cameras for texture streaming (rather than just those with
        /// StreamingController components).
        /// </summary>
        public bool streamingMipmapsAddAllCameras;

        /// <summary>
        /// The maximum number of active texture file IO requests from the texture streaming
        /// system.
        /// </summary>
        public int streamingMipmapsMaxFileIORequests;

        /// <summary>
        /// Maximum number of frames queued up by graphics driver.
        /// </summary>
        public int maxQueuedFrames;

        /// <summary>
        /// Desired color space (Read Only).
        /// </summary>
        public ColorSpace desiredColorSpace;

        /// <summary>
        /// Active color space (Read Only).
        /// </summary>
        public ColorSpace activeColorSpace;

        /// <summary>
        /// A texture size limit applied to most textures.
        /// </summary>
#if UNITY_2022_2_OR_NEWER
        public int globalTextureMipmapLimit;
#else
        public int masterTextureLimit;
#endif

        /// <summary>
        /// The maximum number of pixel lights that should affect any object.
        /// </summary>
        public int pixelLightCount;

        /// <summary>
        /// A maximum LOD level. All LOD groups.
        /// </summary>
        public int maximumLODLevel;

        /// <summary>
        /// Directional light shadow projection.
        /// </summary>
        public ShadowProjection shadowProjection;

        /// <summary>
        /// Number of cascades to use for directional light shadows.
        /// </summary>
        public int shadowCascades;

        /// <summary>
        /// Shadow drawing distance.
        /// </summary>
        public float shadowDistance;

        /// <summary>
        /// Realtime Shadows type to be used.
        /// </summary>
        public UnityEngine.ShadowQuality shadows;

        /// <summary>
        /// The rendering mode of Shadowmask.
        /// </summary>
        public ShadowmaskMode shadowmaskMode;

        /// <summary>
        /// Offset shadow frustum near plane.
        /// </summary>
        public float shadowNearPlaneOffset;

        /// <summary>
        /// The normalized cascade distribution for a 2 cascade setup. The value defines
        /// the position of the cascade with respect to Zero.
        /// </summary>
        public float shadowCascade2Split;

        /// <summary>
        /// The normalized cascade start position for a 4 cascade setup. Each member of the
        /// vector defines the normalized position of the coresponding cascade with respect
        /// to Zero.
        /// </summary>
        public Vector3 shadowCascade4Split;

        /// <summary>
        /// Global multiplier for the LOD's switching distance.
        /// </summary>
        public float lodBias;

        /// <summary>
        /// Global anisotropic filtering mode.
        /// </summary>
        public AnisotropicFiltering anisotropicFiltering;

        /// <summary>
        /// The default resolution of the shadow maps.
        /// </summary>
        public UnityEngine.ShadowResolution shadowResolution;


        public static QualityPreset CreateFromCurrentLevel()
        {
            var preset = new QualityPreset();

            preset.particleRaycastBudget                = QualitySettings.particleRaycastBudget;
            preset.softVegetation                       = QualitySettings.softVegetation;
            preset.vSyncCount                           = QualitySettings.vSyncCount;
            preset.antiAliasing                         = QualitySettings.antiAliasing;
            preset.asyncUploadTimeSlice                 = QualitySettings.asyncUploadTimeSlice;
            preset.asyncUploadBufferSize                = QualitySettings.asyncUploadBufferSize;
            preset.asyncUploadPersistentBuffer          = QualitySettings.asyncUploadPersistentBuffer;
            preset.realtimeReflectionProbes             = QualitySettings.realtimeReflectionProbes;
            preset.billboardsFaceCameraPosition         = QualitySettings.billboardsFaceCameraPosition;
            preset.resolutionScalingFixedDPIFactor      = QualitySettings.resolutionScalingFixedDPIFactor;
            preset.softParticles                        = QualitySettings.softParticles;
            preset.skinWeights                          = QualitySettings.skinWeights;
            preset.streamingMipmapsActive               = QualitySettings.streamingMipmapsActive;
            preset.streamingMipmapsMemoryBudget         = QualitySettings.streamingMipmapsMemoryBudget;
            preset.streamingMipmapsRenderersPerFrame    = QualitySettings.streamingMipmapsRenderersPerFrame;
            preset.streamingMipmapsMaxLevelReduction    = QualitySettings.streamingMipmapsMaxLevelReduction;
            preset.streamingMipmapsAddAllCameras        = QualitySettings.streamingMipmapsAddAllCameras;
            preset.streamingMipmapsMaxFileIORequests    = QualitySettings.streamingMipmapsMaxFileIORequests;
            preset.maxQueuedFrames                      = QualitySettings.maxQueuedFrames;
            preset.desiredColorSpace                    = QualitySettings.desiredColorSpace;
            preset.activeColorSpace                     = QualitySettings.activeColorSpace;
#if UNITY_2022_2_OR_NEWER
            preset.globalTextureMipmapLimit             = QualitySettings.globalTextureMipmapLimit;
#else
            preset.masterTextureLimit                   = QualitySettings.masterTextureLimit;
#endif
            preset.pixelLightCount                      = QualitySettings.pixelLightCount;
            preset.maximumLODLevel                      = QualitySettings.maximumLODLevel;
            preset.shadowProjection                     = QualitySettings.shadowProjection;
            preset.shadowCascades                       = QualitySettings.shadowCascades;
            preset.shadowDistance                       = QualitySettings.shadowDistance;
            preset.shadows                              = QualitySettings.shadows;
            preset.shadowmaskMode                       = QualitySettings.shadowmaskMode;
            preset.shadowNearPlaneOffset                = QualitySettings.shadowNearPlaneOffset;
            preset.shadowCascade2Split                  = QualitySettings.shadowCascade2Split;
            preset.shadowCascade4Split                  = QualitySettings.shadowCascade4Split;
            preset.lodBias                              = QualitySettings.lodBias;
            preset.anisotropicFiltering                 = QualitySettings.anisotropicFiltering;
            preset.shadowResolution                     = QualitySettings.shadowResolution;

            preset.renderPipeline                       = QualitySettings.renderPipeline == null ? null : ScriptableObject.Instantiate(QualitySettings.renderPipeline);

            return preset;
        }

        public void ApplyToCurrentLevel()
        {
            QualitySettings.particleRaycastBudget                  = particleRaycastBudget;
            QualitySettings.softVegetation                         = softVegetation;
            QualitySettings.vSyncCount                             = vSyncCount;
            QualitySettings.antiAliasing                           = antiAliasing;
            QualitySettings.asyncUploadTimeSlice                   = asyncUploadTimeSlice;
            QualitySettings.asyncUploadBufferSize                  = asyncUploadBufferSize;
            QualitySettings.asyncUploadPersistentBuffer            = asyncUploadPersistentBuffer;
            QualitySettings.realtimeReflectionProbes               = realtimeReflectionProbes;
            QualitySettings.billboardsFaceCameraPosition           = billboardsFaceCameraPosition;
            QualitySettings.resolutionScalingFixedDPIFactor        = resolutionScalingFixedDPIFactor;
            QualitySettings.softParticles                          = softParticles;
            QualitySettings.skinWeights                            = skinWeights;
            QualitySettings.streamingMipmapsActive                 = streamingMipmapsActive;
            QualitySettings.streamingMipmapsMemoryBudget           = streamingMipmapsMemoryBudget;
            QualitySettings.streamingMipmapsRenderersPerFrame      = streamingMipmapsRenderersPerFrame;
            QualitySettings.streamingMipmapsMaxLevelReduction      = streamingMipmapsMaxLevelReduction;
            QualitySettings.streamingMipmapsAddAllCameras          = streamingMipmapsAddAllCameras;
            QualitySettings.streamingMipmapsMaxFileIORequests      = streamingMipmapsMaxFileIORequests;
            QualitySettings.maxQueuedFrames                        = maxQueuedFrames;
            // QualitySettings.desiredColorSpace                      = desiredColorSpace; // Read Only
            // QualitySettings.activeColorSpace                       = activeColorSpace;  // Read Only
#if UNITY_2022_2_OR_NEWER
            QualitySettings.globalTextureMipmapLimit               = globalTextureMipmapLimit;
#else
            QualitySettings.masterTextureLimit                     = masterTextureLimit;
#endif
            QualitySettings.pixelLightCount                        = pixelLightCount;
            QualitySettings.maximumLODLevel                        = maximumLODLevel;
            QualitySettings.shadowProjection                       = shadowProjection;
            QualitySettings.shadowCascades                         = shadowCascades;
            QualitySettings.shadowDistance                         = shadowDistance;
            QualitySettings.shadows                                = shadows;
            QualitySettings.shadowmaskMode                         = shadowmaskMode;
            QualitySettings.shadowNearPlaneOffset                  = shadowNearPlaneOffset;
            QualitySettings.shadowCascade2Split                    = shadowCascade2Split;
            QualitySettings.shadowCascade4Split                    = shadowCascade4Split;
            QualitySettings.lodBias                                = lodBias;
            QualitySettings.anisotropicFiltering                   = anisotropicFiltering;
            QualitySettings.shadowResolution                       = shadowResolution;

            // copy backup values back into the render pipeline asset.
            if (QualitySettings.renderPipeline != null && renderPipeline != null)
            {
#if KAMGAM_RENDER_PIPELINE_URP
                applyToCurrentLevelURP();
#endif

#if KAMGAM_RENDER_PIPELINE_HDRP
                applyToCurrentLevelHDRP();
#endif
            }
        }
    }
}
