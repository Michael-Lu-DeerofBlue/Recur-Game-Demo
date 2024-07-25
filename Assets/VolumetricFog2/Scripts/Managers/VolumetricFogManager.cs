//#define FOG_VOID_ROTATION

using UnityEngine;
using UnityEngine.Rendering;

namespace VolumetricFogAndMist2 {

    [ExecuteInEditMode]
    [HelpURL("https://kronnect.com/guides/volumetric-fog-urp-adding-volumetric-fog-mist-to-your-scene/#ftoc-heading-1")]
    public class VolumetricFogManager : MonoBehaviour, IVolumetricFogManager {

        public string managerName {
            get {
                return "Volumetric Fog Manager";
            }
        }

        static PointLightManager _pointLightManager;
        static FogVoidManager _fogVoidManager;
        static VolumetricFogManager _instance;

        [Tooltip("Directional light used as the Sun")]
        public Light sun;
        [Tooltip("Directional light used as the Moon")]
        public Light moon;
        [Tooltip("Flip depth texture. Use only as a workaround to a bug in URP if the depth shows inverted in GameView. Alternatively you can enable MSAA or HDR instead of using this option.")]
        public bool flipDepthTexture;
        [Tooltip("Enable this option to choose this manager when others could be loaded from sub-scenes")]
        public bool mainManager;
        [Tooltip("Optionally specify which transparent layers must be included in the depth prepass. Use only to avoid fog clipping with certain transparent objects.")]
        public LayerMask includeTransparent;
        [Tooltip("Cull mode for the transparent depth prepass")]
        public CullMode transparentCullMode = CullMode.Back;
        [Tooltip("Renders fog in two stages: on the back and on the front of transparent objects such as particles")]
        public bool depthPeeling;
        [Tooltip("Optionally specify which semi-transparent (materials using alpha clipping or cut-off) must be included in the depth prepass. Use only to avoid fog clipping with certain transparent objects.")]
        public LayerMask includeSemiTransparent;
        [Tooltip("Optionally determines the alpha cut off for semitransparent objects")]
        [Range(0, 1)]
        public float alphaCutOff;

        [Tooltip("Light scattering effect through fog")]
        [Range(0, 1)]
        public float scattering;

        [Tooltip("Threshold applied to input brightness")]
        public float scatteringThreshold;
        [Tooltip("Brightness multiplier applied to input")]
        public float scatteringIntensity;
        [Tooltip("Absorption or brightness decay inside the fog")]
        [Range(0, 1)]
        public float scatteringAbsorption = 0.35f;
        public Color scatteringTint = Color.white;
        [Tooltip("Uses higher resolution intermediate buffers and edge-aware upsampling filter")]
        public bool scatteringHighQuality;

        [Range(1, 8)]
        public float downscaling = 1;
        [Tooltip("Depth-based detection threshold for the upscaling reconstruction filter")]
        public float downscalingEdgeDepthThreshold = 0.05f;
        [Range(0, 6)]
        public int blurPasses;
        [Range(1, 8)]
        public float blurDownscaling = 1;
        [Range(0.1f, 4)]
        public float blurSpread = 1f;
        [Tooltip("Uses 16-bit RGBA floating point pixel format for rendering & blur fog volumes. If disabled, 8-bit RGBA pixel format will be used which can improve performance on certain devices. Note that if you use bloom or other HDR-based effects, you should enable this HDR option as well.")]
        public bool blurHDR = true;
        [Tooltip("Enable to use an edge-aware blur.")]
        public bool blurEdgePreserve = true;
        [Tooltip("Ignores blur when fog color intensity is below this value.")]
        public float blurEdgeDepthThreshold = 0.008f;
        [Range(0, 0.2f)]
        public float ditherStrength;

        // used to keep shader option in sync with scripting side
#if FOG_VOID_ROTATION
        public static bool allowFogVoidRotation => true;
#else
        public static bool allowFogVoidRotation => false;
#endif

        const string SKW_FLIP_DEPTH_TEXTURE = "VF2_FLIP_DEPTH_TEXTURE";

        public const uint FOG_VOLUMES_RENDERING_LAYER = 1 << 49;

        public static VolumetricFogManager instance {
            get {
                if (_instance == null) {
                    _instance = Tools.CheckMainManager();
                }
                return _instance;
            }
        }

        public static VolumetricFogManager GetManagerIfExists() {
            if (_instance != null && _instance.gameObject == null) _instance = null;
            if (_instance == null) {
                VolumetricFogManager[] managers = Misc.FindObjectsOfType<VolumetricFogManager>(true);
                int count = managers.Length;
                // look for main manager
                for (int k = 0; k < count; k++) {
                    VolumetricFogManager manager = managers[k];
                    if (manager.mainManager) {
                        _instance = manager;
                        return _instance;
                    }
                }
                if (count > 0) {
                    _instance = managers[0];
                }
            }
            return _instance;
        }

        public static PointLightManager pointLightManager {
            get {
                Tools.CheckManager(ref _pointLightManager);
                return _pointLightManager;
            }
        }

        public static FogVoidManager fogVoidManager {
            get {
                Tools.CheckManager(ref _fogVoidManager);
                return _fogVoidManager;
            }
        }

        void OnEnable() {
            // Forces other managers to be found
            _pointLightManager = null;
            _fogVoidManager = null;
            // Ensures no other fog manager exist
            VolumetricFogManager[] managers = Misc.FindObjectsOfType<VolumetricFogManager>(true);
            if (managers.Length > 1) {
                bool isThisTheMainManager = mainManager;
                for (int k = 0; k < managers.Length; k++) {
                    if (!managers[k].mainManager) DestroyImmediate(managers[k].gameObject);
                }
                if (!isThisTheMainManager) return;
            }
            if (_instance == null) _instance = this;

            SetupLights();
            SetupDepthPrePass();
            Tools.CheckManager(ref _pointLightManager);
            Tools.CheckManager(ref _fogVoidManager);
        }

        void OnValidate() {
            downscalingEdgeDepthThreshold = Mathf.Max(0.0001f, downscalingEdgeDepthThreshold);
            blurEdgeDepthThreshold = Mathf.Max(0.0001f, blurEdgeDepthThreshold);
            scatteringThreshold = Mathf.Max(0, scatteringThreshold);
            scatteringIntensity = Mathf.Max(0, scatteringIntensity);
            SetupDepthPrePass();
        }


        void SetupLights() {
            Light[] lights = Misc.FindObjectsOfType<Light>();
            for (int k = 0; k < lights.Length; k++) {
                Light l = lights[k];
                if (l.type == LightType.Directional) {
                    if (sun == null) {
                        sun = l;
                    }
                    return;
                }
            }
        }

        void SetupDepthPrePass() {
            Shader.SetGlobalInt(SKW_FLIP_DEPTH_TEXTURE, flipDepthTexture ? 1 : 0);
            DepthRenderPrePassFeature.DepthRenderPass.SetupLayerMasks(includeTransparent, includeSemiTransparent);
        }

        /// <summary>
        /// Creates a new fog volume
        /// </summary>
        public static GameObject CreateFogVolume(string name) {
            GameObject go = Resources.Load<GameObject>("Prefabs/FogVolume2D");
            go = Instantiate(go);
            go.name = name;
            return go;
        }

        /// <summary>
        /// Creates a new fog void
        /// </summary>
        public static GameObject CreateFogVoid(string name) {
            return new GameObject(name, typeof(FogVoid));
        }


        /// <summary>
        /// Creates a new fog sub-volume
        /// </summary>
        public static GameObject CreateFogSubVolume(string name) {
            GameObject go = Resources.Load<GameObject>("Prefabs/FogSubVolume");
            go = Instantiate(go);
            go.name = name;
            return go;
        }

    }
}
