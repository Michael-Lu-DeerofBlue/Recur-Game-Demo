using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumetricFogAndMist2 {

    public delegate void OnSettingsChanged();

    public enum VolumetricFogNoiseSize {
        [InspectorName("8")]
        _8 = 8,
        [InspectorName("16")]
        _16 = 16,
        [InspectorName("32")]
        _32 = 32,
        [InspectorName("64")]
        _64 = 64,
        [InspectorName("128")]
        _128 = 128,
        [InspectorName("256")]
        _256 = 256
    }

    public enum DiffusionModel {
        Simple,
        Smooth,
        Strong
    }

    [CreateAssetMenu(menuName = "Volumetric Fog \x8B& Mist/Fog Profile", fileName = "VolumetricFogProfile", order = 1001)]
    public class VolumetricFogProfile : ScriptableObject {

        [Header("Rendering")]
        [Range(1, 16)] public int raymarchQuality = 6;
        [Tooltip("Determines the minimum step size. Increase to improve performance / decrease to improve accuracy. When increasing this value, you can also increase 'Jittering' amount to improve quality.")]
        public float raymarchMinStep = 0.1f;
        public float jittering = 0.5f;
        [Range(0, 2)] public float dithering = 1f;
        [Tooltip("The render queue for this renderer. By default, all transparent objects use a render queue of 3000. Use a lower value to render before all transparent objects.")]
        public int renderQueue = 3100;
        [Tooltip("Optional sorting layer Id (number) for this renderer. By default 0. Usually used to control the order with other transparent renderers, like Sprite Renderer.")]
        public int sortingLayerID;
        [Tooltip("Optional sorting order for this renderer. Used to control the order with other transparent renderers, like Sprite Renderer.")]
        public int sortingOrder;

        [Header("Density")]
        [Tooltip("Do not use any noise at all")]
        public bool constantDensity;
        public Texture2D noiseTexture;
        [Tooltip("Size of the final noise used by the shader. Reduce to improve performance on old devices.")]
        public VolumetricFogNoiseSize noiseTextureOptimizedSize = VolumetricFogNoiseSize._256;
        [Range(0, 3)] public float noiseStrength = 1f;
        public float noiseScale = 15f;
        public float noiseFinalMultiplier = 1f;

        public bool useDetailNoise;
        public Texture3D detailTexture;
        public float detailScale = 0.35f;
        [Range(0, 1f)] public float detailStrength = 0.5f;
        public float detailOffset = -0.5f;

        public float density = 1f;

        [Header("Geometry")]
        public VolumetricFogShape shape = VolumetricFogShape.Box;
        [Range(0, 1)]
        public float scaleNoiseWithHeight;
        [Range(0, 1f)] public float border = 0.05f;
        [Tooltip("Ignores volume height and use a custom height defined by this profile")]
        public bool customHeight;
        public float height;
        public float verticalOffset;
        [Tooltip("When enabled, makes fog appear at certain distance from a camera")]
        public float distance;
        [Range(0, 1)] public float distanceFallOff = 0.93f;
        [Tooltip("Maximum distance from camera")]
        public float maxDistance = 10000;
        [Range(0, 1)]
        public float maxDistanceFallOff;

        [Tooltip("Fits the fog altitude to the terrain heightmap")]
        public bool terrainFit;
        public VolumetricFog.HeightmapCaptureResolution terrainFitResolution = VolumetricFog.HeightmapCaptureResolution._128;
        [Tooltip("Which objects will be included in the heightmap capture. By default all objects are included but you may want to restrict this to just the terrain.")]
        public LayerMask terrainLayerMask = -1;
        [Tooltip("The height of fog above terrain surface.")]
        public float terrainFogHeight = 25f;
        public float terrainFogMinAltitude;
        public float terrainFogMaxAltitude = 150f;

        [Header("Colors")]
        [ColorUsage(showAlpha: false)]
        public Color albedo = new Color32(227, 227, 227, 255);
        public bool enableDepthGradient;
        [GradientUsage(hdr: true, ColorSpace.Linear)] public Gradient depthGradient;
        public float depthGradientMaxDistance = 1000f;
        public bool enableHeightGradient;
        [GradientUsage(hdr: true, ColorSpace.Linear)] public Gradient heightGradient;
        public float brightness = 1f;
        [Range(0, 2)] public float deepObscurance = 1f;
        public Color specularColor = new Color(1, 1, 0.8f, 1);
        [Range(0, 1f)] public float specularThreshold = 0.637f;
        [Range(0, 1f)] public float specularIntensity = 0.428f;

        [Header("Animation")]
        public float turbulence = 0.73f;
        public Vector3 windDirection = new Vector3(0.02f, 0, 0);
        public bool useCustomDetailNoiseWindDirection;
        public Vector3 detailNoiseWindDirection = new Vector3(0.02f, 0, 0);

        [Header("Directional Light")]
        [Tooltip("Enable to synchronize fog light intensity and color with the Sun and the Moon (must be assigned into Volumetric Fog Manager)")]
        public bool dayNightCycle = true;
        [Tooltip("When day/night cycle option is disabled, customize the direction of the Sun light.")]
        public Vector3 sunDirection = Vector3.up;
        [Tooltip("Custom sun color when day/night cycle is disabled")]
        public Color sunColor = new Color(0, 0.9568f, 0.8392f);
        [Tooltip("Custom sun intensity when day/night cycle is disabled")]
        public float sunIntensity = 1f;
        [Tooltip("Ambient light influence")]
        public float ambientLightMultiplier;
        public DiffusionModel lightDiffusionModel = DiffusionModel.Simple;
        [Range(0, 256)] public float lightDiffusionPower = 32;
        public float lightDiffusionIntensity = 0.4f;
        public bool receiveShadows;
        [Range(0, 1)] public float shadowIntensity = 0.5f;
        [Tooltip("Removes shadowed fog")]
        [Range(0, 1)] public float shadowCancellation;
        public float shadowMaxDistance = 250f;
        [Tooltip("Uses the directional light cookie")]
        public bool cookie;

        [Header("Distant Fog")]
        [Tooltip("Enables exponential distant fog. Use this option to cover horizon/sky/far distances with optimal performance")]
        public bool distantFog;
        public float distantFogStartDistance = 1000f;
        public float distantFogDistanceDensity = 0.5f;
        public float distantFogMaxHeight = 4000;
        public float distantFogHeightDensity = 0.5f;
        public Color distantFogColor = new Color(0.358f, 0.358f, 0.358f);
        public float distantFogDiffusionIntensity = 0.4f;
        public int distantFogRenderQueue = 2999;
        public float distantFogBaseAltitude;

        public event OnSettingsChanged onSettingsChanged;

        [NonSerialized]
        public Texture2D depthGradientTex;

        [NonSerialized]
        public Texture2D heightGradientTex;

        Color[] depthGradientColors;
        Color[] heightGradientColors;


        private void OnEnable() {
            if (noiseTexture == null) {
                noiseTexture = Resources.Load<Texture2D>("Textures/NoiseTex256");
            }
            if (detailTexture == null) {
                detailTexture = Resources.Load<Texture3D>("Textures/NoiseTex3D");
            }
            ValidateSettings();
        }

        private void OnValidate() {
            ValidateSettings();
            if (onSettingsChanged != null) {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () => {
                    try {
                        onSettingsChanged();
                        UnityEditor.EditorApplication.delayCall += () => UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    } catch { }
                };
#else
                onSettingsChanged();
#endif
            }
        }

        public void ValidateSettings() {
            distance = Mathf.Max(0, distance);
            density = Mathf.Max(0, density);
            noiseScale = Mathf.Max(0.1f, noiseScale);
            noiseFinalMultiplier = Mathf.Max(0, noiseFinalMultiplier);
            detailScale = Mathf.Max(0.01f, detailScale);
            raymarchMinStep = Mathf.Max(0.1f, raymarchMinStep);
            jittering = Mathf.Max(0, jittering);
            terrainFogHeight = Mathf.Max(0, terrainFogHeight);
            height = Mathf.Max(0, height);
            if (depthGradient == null) {
                depthGradient = new Gradient();
                depthGradient.colorKeys = new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0),
                    new GradientColorKey(Color.white, 1)
                };
            }
            maxDistance = Mathf.Max(0.0001f, maxDistance);
            depthGradientMaxDistance = Mathf.Max(0, depthGradientMaxDistance);
            ambientLightMultiplier = Mathf.Max(0, ambientLightMultiplier);
            sunIntensity = Mathf.Max(0, sunIntensity);
            shadowMaxDistance = Mathf.Max(0, shadowMaxDistance);
            lightDiffusionIntensity = MathF.Max(0, lightDiffusionIntensity);
            distantFogStartDistance = Mathf.Max(0, distantFogStartDistance);
            distantFogDistanceDensity = Mathf.Max(0, distantFogDistanceDensity);
            distantFogMaxHeight = Mathf.Max(0, distantFogMaxHeight);
            distantFogHeightDensity = Mathf.Max(0, distantFogHeightDensity);
            distantFogDiffusionIntensity = Mathf.Max(0, distantFogDiffusionIntensity);

            if (enableDepthGradient) {
                const int DEPTH_GRADIENT_TEX_SIZE = 32;
                bool requiresUpdate = false;
                if (depthGradientTex == null) {
                    depthGradientTex = new Texture2D(DEPTH_GRADIENT_TEX_SIZE, 1, TextureFormat.RGBA32, mipChain: false, linear: true);
                    depthGradientTex.wrapMode = TextureWrapMode.Clamp;
                    requiresUpdate = true;
                }
                if (depthGradientColors == null || depthGradientColors.Length != DEPTH_GRADIENT_TEX_SIZE) {
                    depthGradientColors = new Color[DEPTH_GRADIENT_TEX_SIZE];
                    requiresUpdate = true;
                }
                for (int k = 0; k < DEPTH_GRADIENT_TEX_SIZE; k++) {
                    float t = (float)k / DEPTH_GRADIENT_TEX_SIZE;
                    Color color = depthGradient.Evaluate(t);
                    if (color != depthGradientColors[k]) {
                        depthGradientColors[k] = color;
                        requiresUpdate = true;
                    }
                }
                if (requiresUpdate) {
                    depthGradientTex.SetPixels(depthGradientColors);
                    depthGradientTex.Apply();
                }
            }


            if (enableHeightGradient) {
                const int HEIGHT_GRADIENT_TEX_SIZE = 32;
                bool requiresUpdate = false;
                if (heightGradientTex == null) {
                    heightGradientTex = new Texture2D(HEIGHT_GRADIENT_TEX_SIZE, 1, TextureFormat.RGBA32, mipChain: false, linear: true);
                    heightGradientTex.wrapMode = TextureWrapMode.Clamp;
                    requiresUpdate = true;
                }
                if (heightGradientColors == null || heightGradientColors.Length != HEIGHT_GRADIENT_TEX_SIZE) {
                    heightGradientColors = new Color[HEIGHT_GRADIENT_TEX_SIZE];
                    requiresUpdate = true;
                }
                for (int k = 0; k < HEIGHT_GRADIENT_TEX_SIZE; k++) {
                    float t = (float)k / HEIGHT_GRADIENT_TEX_SIZE;
                    Color color = heightGradient.Evaluate(t);
                    if (color != heightGradientColors[k]) {
                        heightGradientColors[k] = color;
                        requiresUpdate = true;
                    }
                }
                if (requiresUpdate) {
                    heightGradientTex.SetPixels(heightGradientColors);
                    heightGradientTex.Apply();
                }
            }

        }

        public void Lerp(VolumetricFogProfile p1, VolumetricFogProfile p2, float t) {
            float t0 = 1f - t;

            raymarchQuality = (int)(p1.raymarchQuality * t0 + p2.raymarchQuality * t);
            raymarchMinStep = p1.raymarchMinStep * t0 + p2.raymarchMinStep * t;
            jittering = p1.jittering * t0 + p2.jittering * t;
            dithering = p1.dithering * t0 + p2.dithering * t;
            renderQueue = t < 0.5f ? p1.renderQueue : p2.renderQueue;
            sortingLayerID = t < 0.5f ? p1.sortingLayerID : p2.sortingLayerID;
            sortingOrder = t < 0.5f ? p1.sortingOrder : p2.sortingOrder;
            noiseStrength = p1.noiseStrength * t0 + p2.noiseStrength * t;
            noiseScale = p1.noiseScale * t0 + p2.noiseScale * t;
            noiseFinalMultiplier = p1.noiseFinalMultiplier * t0 + p2.noiseFinalMultiplier * t;
            noiseTexture = t < 0.5f ? p1.noiseTexture : p2.noiseTexture;
            useDetailNoise = t < 0.5f ? p1.useDetailNoise : p2.useDetailNoise;
            detailTexture = t < 0.5f ? p1.detailTexture : p2.detailTexture;
            detailScale = p1.detailScale * t0 + p2.detailScale * t;
            detailStrength = p1.detailStrength * t0 + p2.detailStrength * t;
            detailOffset = p1.detailOffset * t0 + p2.detailOffset * t;
            density = p1.density * t0 + p2.density * t;
            shape = t < 0.5f ? p1.shape : p2.shape;
            border = p1.border * t0 + p2.border * t;
            verticalOffset = p1.verticalOffset * t0 + p2.verticalOffset * t;
            distance = p1.distance * t0 + p2.distance * t;
            distanceFallOff = p1.distanceFallOff * t0 + p2.distanceFallOff * t;
            albedo = p1.albedo * t0 + p2.albedo * t;
            constantDensity = t < 0.5f ? p1.constantDensity : p2.constantDensity;
            enableDepthGradient = p1.enableDepthGradient || p2.enableDepthGradient;
            LerpGradient(depthGradient, p1.depthGradient, p2.depthGradient, t);
            depthGradientMaxDistance = p1.depthGradientMaxDistance * t0 + p2.depthGradientMaxDistance * t;
            enableHeightGradient = p1.enableHeightGradient || p2.enableHeightGradient;
            LerpGradient(heightGradient, p1.heightGradient, p2.heightGradient, t);
            ambientLightMultiplier = p1.ambientLightMultiplier * t0 + p2.ambientLightMultiplier * t;
            brightness = p1.brightness * t0 + p2.brightness * t;
            deepObscurance = p1.deepObscurance * t0 + p2.deepObscurance * t;
            specularColor = p1.specularColor * t0 + p2.specularColor * t;
            specularThreshold = p1.specularThreshold * t0 + p2.specularThreshold * t;
            specularIntensity = p1.specularIntensity * t0 + p2.specularIntensity * t;
            turbulence = p1.turbulence * t0 + p2.turbulence * t;
            windDirection = p1.windDirection * t0 + p2.windDirection * t;
            useCustomDetailNoiseWindDirection = t < 0.5f ? p1.useCustomDetailNoiseWindDirection : p2.useCustomDetailNoiseWindDirection;
            detailNoiseWindDirection = p1.detailNoiseWindDirection * t0 + p2.detailNoiseWindDirection * t;
            lightDiffusionModel = t < 0.5f ? p1.lightDiffusionModel : p2.lightDiffusionModel;
            lightDiffusionPower = p1.lightDiffusionPower * t0 + p2.lightDiffusionPower * t;
            lightDiffusionIntensity = p1.lightDiffusionIntensity * t0 + p2.lightDiffusionIntensity * t;
            receiveShadows = t < 0.5f ? p1.receiveShadows : p2.receiveShadows;
            shadowIntensity = p1.shadowIntensity * t0 + p2.shadowIntensity * t;
            shadowCancellation = t < 0.5f ? p1.shadowCancellation : p2.shadowCancellation;
            shadowMaxDistance = p1.shadowMaxDistance * t0 + p2.shadowMaxDistance * t;
            terrainFit = t < 0.5f ? p1.terrainFit : p2.terrainFit;
            terrainFitResolution = t < 0.5 ? p1.terrainFitResolution : p2.terrainFitResolution;
            terrainFogHeight = p1.terrainFogHeight * t0 + p2.terrainFogHeight * t;
            terrainFogMinAltitude = p1.terrainFogMinAltitude * t0 + p2.terrainFogMinAltitude * t;
            terrainFogMaxAltitude = p1.terrainFogMaxAltitude * t0 + p2.terrainFogMaxAltitude * t;
            terrainLayerMask = t < 0.5f ? p1.terrainLayerMask : p2.terrainLayerMask;
            dayNightCycle = t < 0.5f ? p1.dayNightCycle : p2.dayNightCycle;
            sunDirection = Vector3.Slerp(p1.sunDirection, p2.sunDirection, t);
            sunColor = p1.sunColor * t0 + p2.sunColor * t;
            sunIntensity = p1.sunIntensity * t0 + p2.sunIntensity * t;
            ambientLightMultiplier = p1.ambientLightMultiplier * t0 + p2.ambientLightMultiplier * t;
            cookie = t < 0.5f ? p1.cookie : p2.cookie;
            distantFog = t < 0.5f ? p1.distantFog : p2.distantFog;
            distantFogStartDistance = p1.distantFogStartDistance * t0 + p2.distantFogStartDistance * t;
            distantFogDistanceDensity = p1.distantFogDistanceDensity * t0 + p2.distantFogDistanceDensity * t;
            distantFogMaxHeight = p1.distantFogMaxHeight * t0 + p2.distantFogMaxHeight * t;
            distantFogHeightDensity = p1.distantFogHeightDensity * t0 + p2.distantFogHeightDensity * t;
            distantFogColor = p1.distantFogColor * t0 + p2.distantFogColor * t;
            distantFogDiffusionIntensity = p1.distantFogDiffusionIntensity * t0 + p2.distantFogDiffusionIntensity * t;
            distantFogBaseAltitude = p1.distantFogBaseAltitude * t0 + p2.distantFogBaseAltitude * t;

            ValidateSettings();
        }

        readonly static List<float> colorKeysTimes = new List<float>();
        readonly static List<float> alphaKeysTimes = new List<float>();

        void LerpGradient(Gradient g, Gradient a, Gradient b, float t) {

            if (g == null || a == null || b == null) return;

            if (a.colorKeys.Length + b.colorKeys.Length > 8 || a.alphaKeys.Length + b.alphaKeys.Length > 8) {
                Debug.LogError("Gradients total key count exceeding 8, can not lerp");
                return;
            }

            colorKeysTimes.Clear();
            if (a.colorKeys != null) {
                for (int i = 0; i < a.colorKeys.Length; i++) {
                    float k = a.colorKeys[i].time;
                    if (!colorKeysTimes.Contains(k))
                        colorKeysTimes.Add(k);
                }
            }

            if (b.colorKeys != null) {
                for (int i = 0; i < b.colorKeys.Length; i++) {
                    float k = b.colorKeys[i].time;
                    if (!colorKeysTimes.Contains(k))
                        colorKeysTimes.Add(k);
                }
            }

            alphaKeysTimes.Clear();
            if (a.alphaKeys != null) {
                for (int i = 0; i < a.alphaKeys.Length; i++) {
                    float k = a.alphaKeys[i].time;
                    if (!alphaKeysTimes.Contains(k))
                        alphaKeysTimes.Add(k);
                }
            }

            if (b.alphaKeys != null) {
                for (int i = 0; i < b.alphaKeys.Length; i++) {
                    float k = b.alphaKeys[i].time;
                    if (!alphaKeysTimes.Contains(k))
                        alphaKeysTimes.Add(k);
                }
            }

            int colorKeysTimesCount = colorKeysTimes.Count;
            GradientColorKey[] colorKeys = g.colorKeys;
            if (colorKeys == null || colorKeys.Length != colorKeysTimesCount) {
                colorKeys = new GradientColorKey[colorKeysTimesCount];
            }
            for (int i = 0; i < colorKeysTimesCount; i++) {
                float key = colorKeysTimes[i];
                var color = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
                colorKeys[i] = new GradientColorKey(color, key);
            }

            int alphaKeysTimesCount = alphaKeysTimes.Count;
            GradientAlphaKey[] alphaKeys = g.alphaKeys;
            if (alphaKeys == null || alphaKeys.Length != alphaKeysTimesCount) {
                alphaKeys = new GradientAlphaKey[alphaKeysTimesCount];
            }
            for (int i = 0; i < alphaKeysTimesCount; i++) {
                float key = alphaKeysTimes[i];
                var color = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
                alphaKeys[i] = new GradientAlphaKey(color.a, key);
            }

            g.SetKeys(colorKeys, alphaKeys);
        }

    }
}

