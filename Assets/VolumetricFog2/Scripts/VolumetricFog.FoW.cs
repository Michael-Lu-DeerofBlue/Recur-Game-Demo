//------------------------------------------------------------------------------------------------------------------
// Volumetric Fog & Mist 2
// Created by Kronnect
//------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;


namespace VolumetricFogAndMist2 {


    public enum MaskTextureBrushMode {
        AddFog = 0,
        RemoveFog = 1,
        ColorFog = 2
    }


    public enum FoWUpdateMethod {
        MainThread,
        BackgroundThread
    }

    public partial class VolumetricFog : MonoBehaviour {

        public bool enableFogOfWar;
        public Vector3 fogOfWarCenter;
        public bool fogOfWarIsLocal;
        public Vector3 fogOfWarSize = new Vector3(1024, 0, 1024);
        public bool fogOfWarShowCoverage;
        [Range(32, 2048)] public int fogOfWarTextureSize = 256;
        [Tooltip("Delay before the fog alpha is restored. A value of 0 keeps the fog cleared forever.")]
        [Range(0, 100)] public float fogOfWarRestoreDelay;
        [Range(0, 25)] public float fogOfWarRestoreDuration = 2f;
        [Range(0, 1)] public float fogOfWarSmoothness = 1f;
        public bool fogOfWarBlur;

        const int MAX_SIMULTANEOUS_TRANSITIONS = 64000;
        bool canDestroyFOWTexture;
        float now;

        #region In-Editor fog of war painter

        public bool maskEditorEnabled;
        public MaskTextureBrushMode maskBrushMode = MaskTextureBrushMode.RemoveFog;
        public Color maskBrushColor = Color.white;
        [Range(1, 128)] public int maskBrushWidth = 20;
        [Range(0, 1)] public float maskBrushFuzziness = 0.5f;
        [Range(0, 1)] public float maskBrushOpacity = 0.15f;

        #endregion

        [SerializeField]
        Texture2D _fogOfWarTexture;

        public Vector3 anchoredFogOfWarCenter => fogOfWarIsLocal ? transform.position + fogOfWarCenter : fogOfWarCenter;


        public Texture2D fogOfWarTexture {
            get { return _fogOfWarTexture; }
            set {
                if (_fogOfWarTexture != value) {
                    if (value != null) {
                        if (value.width != value.height) {
                            Debug.LogError("Fog of war texture must be square.");
                        } else {
                            _fogOfWarTexture = value;
                            canDestroyFOWTexture = false;
                            ReloadFogOfWarTexture();
                        }
                    } else {
                        if (canDestroyFOWTexture && _fogOfWarTexture != null) {
                            DestroyImmediate(_fogOfWarTexture);
                        }
                        _fogOfWarTexture = null;
                        canDestroyFOWTexture = false;
                    }
                    if (fogMat != null) {
                        fogMat.SetTexture(ShaderParams.FogOfWarTexture, _fogOfWarTexture);
                    }
                }
            }
        }


        Color32[] fogOfWarColorBuffer;

        struct FogOfWarTransition {
            public bool enabled;
            public int x, y;
            public float startTime, startDelay;
            public float duration;
            public int initialAlpha;
            public int targetAlpha;

            public int restoreToAlpha;
            public float restoreDelay;
            public float restoreDuration;
        }

        FogOfWarTransition[] fowTransitionList;
        int lastTransitionPos;
        Dictionary<int, int> fowTransitionIndices;
        bool requiresTextureUpload;
        Material fowBlur;
        RenderTexture fowBlur1, fowBlur2;


        void FogOfWarInit() {
            if (fowTransitionList == null || fowTransitionList.Length != MAX_SIMULTANEOUS_TRANSITIONS) {
                fowTransitionList = new FogOfWarTransition[MAX_SIMULTANEOUS_TRANSITIONS];
            }
            if (fowTransitionIndices == null) {
                fowTransitionIndices = new Dictionary<int, int>(MAX_SIMULTANEOUS_TRANSITIONS);
            } else {
                fowTransitionIndices.Clear();
            }
            lastTransitionPos = -1;

            if (_fogOfWarTexture == null) {
                FogOfWarUpdateTexture();
            } else if (enableFogOfWar && (fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)) {
                ReloadFogOfWarTexture();
            }
        }

        void FogOfWarDestroy() {
            if (canDestroyFOWTexture) {
                DestroyImmediate(_fogOfWarTexture);
            }
            if (fowBlur1 != null) {
                fowBlur1.Release();
            }
            if (fowBlur2 != null) {
                fowBlur2.Release();
            }
        }

        /// <summary>
        /// Reloads the current contents of the fog of war texture
        /// </summary>
        public void ReloadFogOfWarTexture() {
            if (_fogOfWarTexture == null || profile == null) return;
            fogOfWarTextureSize = _fogOfWarTexture.width;
            EnsureTextureIsReadable(_fogOfWarTexture);
            fogOfWarColorBuffer = _fogOfWarTexture.GetPixels32();
            lastTransitionPos = -1;
            fowTransitionIndices.Clear();
            if (!enableFogOfWar) {
                enableFogOfWar = true;
                UpdateMaterialPropertiesNow();
            }
        }


        void EnsureTextureIsReadable(Texture2D tex) {
#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(tex);
            if (string.IsNullOrEmpty(path))
                return;
            UnityEditor.TextureImporter imp = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
            if (imp != null && !imp.isReadable) {
                imp.isReadable = true;
                imp.SaveAndReimport();
            }
#endif
        }

        void FogOfWarUpdateTexture() {
            if (!enableFogOfWar || !Application.isPlaying)
                return;
            int size = GetScaledSize(fogOfWarTextureSize, 1.0f);
            if (_fogOfWarTexture == null || _fogOfWarTexture.width != size || _fogOfWarTexture.height != size) {
                _fogOfWarTexture = new Texture2D(size, size, TextureFormat.RGBA32, false, true);
                _fogOfWarTexture.hideFlags = HideFlags.DontSave;
                _fogOfWarTexture.filterMode = FilterMode.Bilinear;
                _fogOfWarTexture.wrapMode = TextureWrapMode.Clamp;
                canDestroyFOWTexture = true;
                ResetFogOfWar();
            }
        }

        int GetScaledSize(int size, float factor) {
            size = (int)(size / factor);
            size /= 4;
            if (size < 1)
                size = 1;
            return size * 4;
        }



        /// <summary>
        /// Updates fog of war transitions and uploads texture changes to GPU if required
        /// </summary>
        public void UpdateFogOfWar(bool forceUpload = false) {
            if (!enableFogOfWar || _fogOfWarTexture == null)
                return;

            if (forceUpload) {
                requiresTextureUpload = true;
            }

            int tw = _fogOfWarTexture.width;
            now = Time.time;
            for (int k = 0; k <= lastTransitionPos; k++) {
                FogOfWarTransition fw = fowTransitionList[k];
                if (!fw.enabled)
                    continue;
                float elapsed = now - fw.startTime - fw.startDelay;
                if (elapsed > 0) {
                    float t = fw.duration <= 0 ? 1 : elapsed / fw.duration;
                    if (t < 0) t = 0; else if (t > 1f) t = 1f;
                    int alpha = (int)(fw.initialAlpha + (fw.targetAlpha - fw.initialAlpha) * t);
                    int colorPos = fw.y * tw + fw.x;
                    fogOfWarColorBuffer[colorPos].a = (byte)alpha;
                    requiresTextureUpload = true;
                    if (t >= 1f) {
                        fowTransitionList[k].enabled = false;
                        // Add refill slot if needed
                        if (fw.targetAlpha != fw.restoreToAlpha && fw.restoreDelay > 0) {
                            AddFogOfWarTransitionSlot(fw.x, fw.y, (byte)fw.targetAlpha, (byte)fw.restoreToAlpha, fw.restoreDelay, fw.restoreDuration, (byte)fw.restoreToAlpha, 0, 0);
                        }
                    }
                }
            }
            if (requiresTextureUpload) {
                requiresTextureUpload = false;
                _fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
                _fogOfWarTexture.Apply();

                // Smooth texture
                if (fogOfWarBlur) {
                    SetFowBlurTexture();
                }

#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    UnityEditor.EditorUtility.SetDirty(_fogOfWarTexture);
                }
#endif


            }

            if (fogOfWarIsLocal) {
                UpdateFogOfWarMaterialBoundsProperties();
            }
        }

        void SetFowBlurTexture() {
            if (fowBlur == null) {
                fowBlur = new Material(Shader.Find("Hidden/VolumetricFog2/FoWBlur"));
                fowBlur.hideFlags = HideFlags.DontSave;
            }
            if (fowBlur == null)
                return;

            if (fowBlur1 == null || fowBlur1.width != _fogOfWarTexture.width || fowBlur2 == null || fowBlur2.width != _fogOfWarTexture.width) {
                CreateFoWBlurRTs();
            }
            fowBlur1.DiscardContents();
            Graphics.Blit(_fogOfWarTexture, fowBlur1, fowBlur, 0);
            fowBlur2.DiscardContents();
            Graphics.Blit(fowBlur1, fowBlur2, fowBlur, 1);
            fogMat.SetTexture(ShaderParams.FogOfWarTexture, fowBlur2);
        }


        void CreateFoWBlurRTs() {
            if (fowBlur1 != null) {
                fowBlur1.Release();
            }
            if (fowBlur2 != null) {
                fowBlur2.Release();
            }
            RenderTextureDescriptor desc = new RenderTextureDescriptor(_fogOfWarTexture.width, _fogOfWarTexture.height, RenderTextureFormat.ARGB32, 0);
            fowBlur1 = new RenderTexture(desc);
            fowBlur2 = new RenderTexture(desc);
        }

        /// <summary>
        /// Instantly changes the alpha value of the fog of war at world position. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="worldPosition">in world space coordinates.</param>
        /// <param name="radius">radius of application in world units.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        public void SetFogOfWarAlpha(Vector3 worldPosition, float radius, float fogNewAlpha) {
            SetFogOfWarAlpha(worldPosition, radius, fogNewAlpha, 1f);
        }


        /// <summary>
        /// Changes the alpha value of the fog of war at world position creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="worldPosition">in world space coordinates.</param>
        /// <param name="radius">radius of application in world units.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        public void SetFogOfWarAlpha(Vector3 worldPosition, float radius, float fogNewAlpha, float duration = 0) {
            SetFogOfWarAlpha(worldPosition, radius, fogNewAlpha, true, duration, fogOfWarSmoothness, fogOfWarRestoreDelay, fogOfWarRestoreDuration);
        }


        /// <summary>
        /// Changes the alpha value of the fog of war at world position creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="worldPosition">in world space coordinates.</param>
        /// <param name="radius">radius of application in world units.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        /// <param name="smoothness">border smoothness (0 = sharp borders, 1 = smooth transition)</param>
        public void SetFogOfWarAlpha(Vector3 worldPosition, float radius, float fogNewAlpha, float duration = 0, float smoothness = 0) {
            SetFogOfWarAlpha(worldPosition, radius, fogNewAlpha, true, duration, smoothness, fogOfWarRestoreDelay, fogOfWarRestoreDuration);
        }

        /// <summary>
        /// Changes the alpha value of the fog of war at world position creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="worldPosition">in world space coordinates.</param>
        /// <param name="radius">radius of application in world units.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="blendAlpha">if new alpha is combined with preexisting alpha value or replaced.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        /// <param name="smoothness">border smoothness.</param>
        /// <param name="restoreDelay">delay before the fog alpha is restored. Pass 0 to keep change forever.</param>
        /// <param name="restoreDuration">restore duration in seconds.</param>
        /// <param name="restoreToAlphaValue">final alpha value when fog restores.</param>
        public void SetFogOfWarAlpha(Vector3 worldPosition, float radius, float fogNewAlpha, bool blendAlpha = false, float duration = 0, float smoothness = 0, float restoreDelay = 0, float restoreDuration = 2, float restoreToAlphaValue = 1f, FoWUpdateMethod updateMethod = FoWUpdateMethod.BackgroundThread) {

            if (_fogOfWarTexture == null || fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)
                return;

            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            now = Time.time;

            if (updateMethod == FoWUpdateMethod.BackgroundThread) {
                System.Threading.Tasks.Task.Run(() => internal_SetFogOfWarAlpha(tw, th, worldPosition, radius, fogNewAlpha, blendAlpha, duration, smoothness, restoreDelay, restoreDuration, restoreToAlphaValue));
                return;
            }
            internal_SetFogOfWarAlpha(tw, th, worldPosition, radius, fogNewAlpha, blendAlpha, duration, smoothness, restoreDelay, restoreDuration, restoreToAlphaValue);
        }

        void internal_SetFogOfWarAlpha(int tw, int th, Vector3 worldPosition, float radius, float fogNewAlpha, bool blendAlpha = false, float duration = 0, float smoothness = 0, float restoreDelay = 0, float restoreDuration = 2, float restoreToAlphaValue = 1f) {


            Vector3 fogOfWarCenter = anchoredFogOfWarCenter;

            float tx = (worldPosition.x - fogOfWarCenter.x) / fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (worldPosition.z - fogOfWarCenter.z) / fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            float sm = 0.0001f + smoothness;
            byte newAlpha8 = (byte)(fogNewAlpha * 255);
            float tr = radius / fogOfWarSize.z;
            int delta = (int)(th * tr);
            int deltaSqr = delta * delta;
            byte restoreAlpha = (byte)(255 * restoreToAlphaValue);

            int minR = Mathf.Max(0, pz - delta);
            int maxR = Mathf.Min(th - 1, pz + delta);
            int minC = Mathf.Max(0, px - delta);
            int maxC = Mathf.Min(tw - 1, px + delta);

            for (int r = minR; r <= maxR; r++) {
                int rOffset = r * tw;
                int rDistSqr = (pz - r) * (pz - r);
                for (int c = minC; c <= maxC; c++) {
                    int cDistSqr = (px - c) * (px - c);
                    int distanceSqr = rDistSqr + cDistSqr;
                    if (distanceSqr <= deltaSqr) {
                        int colorBufferPos = rOffset + c;
                        Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                        if (!blendAlpha) {
                            colorBuffer.a = 255;
                        }
                        distanceSqr = deltaSqr - distanceSqr;
                        float t = (float)distanceSqr / (deltaSqr * sm);
                        t = 1f - t;
                        if (t < 0) {
                            t = 0;
                        } else if (t > 1f) {
                            t = 1f;
                        }
                        byte targetAlpha = (byte)(newAlpha8 + (colorBuffer.a - newAlpha8) * t);
                        if (targetAlpha < 255 && (colorBuffer.a != targetAlpha || restoreDelay > 0)) {
                            if (duration > 0) {
                                AddFogOfWarTransitionSlot(c, r, colorBuffer.a, targetAlpha, 0, duration, restoreAlpha, restoreDelay, restoreDuration);
                            } else {
                                colorBuffer.a = targetAlpha;
                                fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                                requiresTextureUpload = true;
                                if (restoreDelay > 0) {
                                    AddFogOfWarTransitionSlot(c, r, targetAlpha, restoreAlpha, restoreDelay, restoreDuration, targetAlpha, 0, 0);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Changes the alpha value of the fog of war within bounds creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="bounds">in world space coordinates.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        public void SetFogOfWarAlpha(Bounds bounds, float fogNewAlpha, float duration = 0) {
            SetFogOfWarAlpha(bounds, fogNewAlpha, false, duration, fogOfWarSmoothness, fogOfWarRestoreDelay, fogOfWarRestoreDuration);
        }

        /// <summary>
        /// Changes the alpha value of the fog of war within bounds creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="bounds">in world space coordinates.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        /// <param name="smoothness">border smoothness.</param>
        public void SetFogOfWarAlpha(Bounds bounds, float fogNewAlpha, float duration, float smoothness) {
            SetFogOfWarAlpha(bounds, fogNewAlpha, false, duration, smoothness, fogOfWarRestoreDelay, fogOfWarRestoreDuration);
        }


        /// <summary>
        /// Changes the alpha value of the fog of war within bounds creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="bounds">in world space coordinates.</param>
        /// <param name="fogNewAlpha">target alpha value (0-1).</param>
        /// <param name="blendAlpha">if new alpha is combined with preexisting alpha value or replaced.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        /// <param name="smoothness">border smoothness.</param>
        /// <param name="fuzzyness">randomization of border noise.</param>
        /// <param name="restoreDelay">delay before the fog alpha is restored. Pass 0 to keep change forever.</param>
        /// <param name="restoreDuration">restore duration in seconds.</param>
        /// <param name="restoreToAlpha">alpha value (0-1) for the fog when restore is completed.</param>
        public void SetFogOfWarAlpha(Bounds bounds, float fogNewAlpha, bool blendAlpha, float duration = 0, float smoothness = 0, float restoreDelay = 0, float restoreDuration = 2, float restoreToAlpha = 1f, FoWUpdateMethod updateMethod = FoWUpdateMethod.BackgroundThread) {
            if (_fogOfWarTexture == null || fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)
                return;

            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            now = Time.time;

            if (updateMethod == FoWUpdateMethod.BackgroundThread) {
                System.Threading.Tasks.Task.Run(() => internal_SetFogOfWarAlpha(tw, th, bounds, fogNewAlpha, blendAlpha, duration, smoothness, restoreDelay, restoreDuration, restoreToAlpha));
                return;
            }

            internal_SetFogOfWarAlpha(tw, th, bounds, fogNewAlpha, blendAlpha, duration, smoothness, restoreDelay, restoreDuration, restoreToAlpha);
        }

        void internal_SetFogOfWarAlpha(int tw, int th, Bounds bounds, float fogNewAlpha, bool blendAlpha, float duration = 0, float smoothness = 0, float restoreDelay = 0, float restoreDuration = 2, float restoreToAlpha = 1f) {

            Vector3 fogOfWarCenter = anchoredFogOfWarCenter;
            Vector3 worldPosition = bounds.center;

            float tx = (worldPosition.x - fogOfWarCenter.x) / fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (worldPosition.z - fogOfWarCenter.z) / fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            byte newAlpha8 = (byte)(fogNewAlpha * 255);
            float trz = bounds.extents.z / fogOfWarSize.z;
            float trx = bounds.extents.x / fogOfWarSize.x;
            float aspect1 = trx > trz ? 1f : trz / trx;
            float aspect2 = trx > trz ? trx / trz : 1f;
            int deltaz = (int)(th * trz);
            int deltazSqr = deltaz * deltaz;
            int deltax = (int)(tw * trx);
            int deltaxSqr = deltax * deltax;
            float sm = 0.0001f + smoothness;
            byte restoreAlpha = (byte)(restoreToAlpha * 255);

            int minR = Mathf.Max(0, pz - deltaz);
            int maxR = Mathf.Min(th - 1, pz + deltaz);
            int minC = Mathf.Max(0, px - deltax);
            int maxC = Mathf.Min(tw - 1, px + deltax);

            for (int r = minR; r <= maxR; r++) {
                int rOffset = r * tw;
                int distancezSqr = (pz - r) * (pz - r);
                distancezSqr = deltazSqr - distancezSqr;
                float t1 = (float)distancezSqr * aspect1 / (deltazSqr * sm);

                for (int c = minC; c <= maxC; c++) {
                    int distancexSqr = (px - c) * (px - c);
                    int colorBufferPos = rOffset + c;
                    Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                    if (!blendAlpha) colorBuffer.a = 255;

                    distancexSqr = deltaxSqr - distancexSqr;
                    float t2 = (float)distancexSqr * aspect2 / (deltaxSqr * sm);
                    float t = t1 < t2 ? t1 : t2;
                    t = 1f - t;
                    if (t < 0) t = 0; else if (t > 1f) t = 1f;
                    byte targetAlpha = (byte)(newAlpha8 + (colorBuffer.a - newAlpha8) * t);
                    if (targetAlpha < 255 && (colorBuffer.a != targetAlpha || restoreDelay > 0)) {
                        if (duration > 0) {
                            AddFogOfWarTransitionSlot(c, r, colorBuffer.a, targetAlpha, 0, duration, restoreAlpha, restoreDelay, restoreDuration);
                        } else {
                            colorBuffer.a = targetAlpha;
                            fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                            requiresTextureUpload = true;
                            if (restoreDelay > 0) {
                                AddFogOfWarTransitionSlot(c, r, targetAlpha, restoreAlpha, restoreDelay, restoreDuration, restoreAlpha, 0, 0);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Changes the alpha value of the fog of war within collider creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="collider">collider used to define the shape of the area where fog of war alpha will be set. Collider must be convex.</param>
        /// <param name="fogNewAlpha">target alpha value (0-1).</param>
        /// <param name="blendAlpha">if new alpha is combined with preexisting alpha value or replaced.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        /// <param name="smoothness">border smoothness.</param>
        /// <param name="fuzzyness">randomization of border noise.</param>
        /// <param name="restoreDelay">delay before the fog alpha is restored. Pass 0 to keep change forever.</param>
        /// <param name="restoreDuration">restore duration in seconds.</param>
        /// <param name="restoreToAlpha">alpha value (0-1) for the fog when restore is completed.</param>
        public void SetFogOfWarAlpha(Collider collider, float fogNewAlpha, bool blendAlpha = false, float duration = 0, float smoothness = 0, float restoreDelay = 0, float restoreDuration = 2, float restoreToAlpha = 1f) {
            if (_fogOfWarTexture == null || fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)
                return;

            Vector3 fogOfWarCenter = anchoredFogOfWarCenter;

            Bounds bounds = collider.bounds;
            Vector3 worldPosition = bounds.center;
            float tx = (worldPosition.x - fogOfWarCenter.x) / fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (worldPosition.z - fogOfWarCenter.z) / fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            now = Time.time;
            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            byte newAlpha8 = (byte)(fogNewAlpha * 255);
            float trz = bounds.extents.z / fogOfWarSize.z;
            float trx = bounds.extents.x / fogOfWarSize.x;
            float aspect1 = trx > trz ? 1f : trz / trx;
            float aspect2 = trx > trz ? trx / trz : 1f;
            int deltaz = (int)(th * trz);
            int deltazSqr = deltaz * deltaz;
            int deltax = (int)(tw * trx);
            int deltaxSqr = deltax * deltax;
            float sm = 0.0001f + smoothness;
            byte restoreAlpha = (byte)(restoreToAlpha * 255);


            Vector3 wpos = bounds.min;
            wpos.y = bounds.center.y;
            for (int rr = 0; rr <= deltaz * 2; rr++) {
                int r = pz - deltaz + rr;
                if (r > 0 && r < th - 1) {
                    int distancezSqr = (pz - r) * (pz - r);
                    distancezSqr = deltazSqr - distancezSqr;
                    float t1 = (float)distancezSqr * aspect1 / (deltazSqr * sm);
                    wpos.z = bounds.min.z + bounds.size.z * rr / (deltaz * 2f);
                    for (int cc = 0; cc <= deltax * 2; cc++) {
                        int c = px - deltax + cc;
                        if (c > 0 && c < tw - 1) {
                            wpos.x = bounds.min.x + bounds.size.x * cc / (deltax * 2f);
                            Vector3 colliderPos = collider.ClosestPoint(wpos);
                            if (colliderPos != wpos) continue; // point is outside collider

                            int distancexSqr = (px - c) * (px - c);
                            int colorBufferPos = r * tw + c;
                            Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                            if (!blendAlpha) colorBuffer.a = 255;
                            distancexSqr = deltaxSqr - distancexSqr;
                            float t2 = (float)distancexSqr * aspect2 / (deltaxSqr * sm);
                            float t = t1 < t2 ? t1 : t2;
                            t = 1f - t;
                            if (t < 0) t = 0; else if (t > 1f) t = 1f;
                            byte targetAlpha = (byte)(newAlpha8 + (colorBuffer.a - newAlpha8) * t);
                            if (targetAlpha < 255 && (colorBuffer.a != targetAlpha || restoreDelay > 0)) {
                                if (duration > 0) {
                                    AddFogOfWarTransitionSlot(c, r, colorBuffer.a, targetAlpha, 0, duration, restoreAlpha, restoreDelay, restoreDuration);
                                } else {
                                    colorBuffer.a = targetAlpha;
                                    fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                                    requiresTextureUpload = true;
                                    if (restoreDelay > 0) {
                                        AddFogOfWarTransitionSlot(c, r, targetAlpha, restoreAlpha, restoreDelay, restoreDuration, restoreAlpha, 0, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Changes the alpha value of the fog of war within bounds creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="go">gameobject used to define the shape of the area where fog of war alpha will be set. The gameobject must have a mesh associated.</param>
        /// <param name="fogNewAlpha">target alpha value (0-1).</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        /// <param name="fuzzyness">randomization of border noise.</param>
        /// <param name="restoreDelay">delay before the fog alpha is restored. Pass 0 to keep change forever.</param>
        /// <param name="restoreDuration">restore duration in seconds.</param>
        /// <param name="restoreToAlpha">alpha value (0-1) for the fog when restore is completed.</param>
        public void SetFogOfWarAlpha(GameObject go, float fogNewAlpha, float duration = 0, float restoreDelay = 0, float restoreDuration = 2, float restoreToAlpha = 1f, FoWUpdateMethod updateMethod = FoWUpdateMethod.BackgroundThread) {
            if (_fogOfWarTexture == null || fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)
                return;

            if (go == null) return;

            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            now = Time.time;

            MeshRenderer meshRenderer = go.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer == null) {
                Debug.LogError("No MeshRenderer found on this object.");
                return;
            }

            Bounds bounds = meshRenderer.bounds;

            MeshFilter mf = meshRenderer.GetComponent<MeshFilter>();
            if (mf == null) {
                Debug.LogError("No MeshFilter found on this object.");
                return;
            }
            Mesh mesh = mf.sharedMesh;
            if (mesh == null) {
                Debug.LogError("No Mesh found on this object.");
                return;
            }
            if (mesh.GetTopology(0) != MeshTopology.Triangles) {
                Debug.LogError("Only triangle topology is supported by this tool.");
                return;
            }

            // Get triangle info
            int[] indices = mesh.triangles;
            Vector3[] vertices = mesh.vertices;

            int verticesLength = vertices.Length;
            Transform t = meshRenderer.transform;
            for (int k = 0; k < verticesLength; k++) {
                vertices[k] = t.TransformPoint(vertices[k]);
            }

            if (updateMethod == FoWUpdateMethod.BackgroundThread) {
                System.Threading.Tasks.Task.Run(() => internal_SetFogOfWarAlpha(tw, th, bounds, indices, vertices, fogNewAlpha, duration, restoreDelay, restoreDuration, restoreToAlpha));
                return;
            }

            internal_SetFogOfWarAlpha(tw, th, bounds, indices, vertices, fogNewAlpha, duration, restoreDelay, restoreDuration, restoreToAlpha);
        }


        void internal_SetFogOfWarAlpha(int tw, int th, Bounds bounds, int[] indices, Vector3[] vertices, float fogNewAlpha, float duration = 0, float restoreDelay = 0, float restoreDuration = 2, float restoreToAlpha = 1f) {

            Vector3 fogOfWarCenter = anchoredFogOfWarCenter;
            Vector3 worldPosition = bounds.center;
            float tx = (worldPosition.x - fogOfWarCenter.x) / fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (worldPosition.z - fogOfWarCenter.z) / fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            byte newAlpha8 = (byte)(fogNewAlpha * 255);
            byte restoreAlpha = (byte)(restoreToAlpha * 255);

            int indicesLength = indices.Length;

            Vector2[] triangles = new Vector2[indicesLength];
            for (int k = 0; k < indicesLength; k += 3) {
                triangles[k].x = vertices[indices[k]].x;
                triangles[k].y = vertices[indices[k]].z;
                triangles[k + 1].x = vertices[indices[k + 1]].x;
                triangles[k + 1].y = vertices[indices[k + 1]].z;
                triangles[k + 2].x = vertices[indices[k + 2]].x;
                triangles[k + 2].y = vertices[indices[k + 2]].z;
            }
            int index = 0;

            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            float trz = bounds.extents.z / fogOfWarSize.z;
            float trx = bounds.extents.x / fogOfWarSize.x;
            int deltaz = (int)(th * trz);
            int deltax = (int)(tw * trx);
            int r0 = pz - deltaz;
            if (r0 < 1) r0 = 1; else if (r0 >= th) r0 = th - 1;
            int r1 = pz + deltaz;
            if (r1 < 1) r1 = 1; else if (r1 >= th) r1 = th - 1;
            int c0 = px - deltax;
            if (c0 < 1) c0 = 1; else if (c0 >= tw) c0 = tw - 1;
            int c1 = px + deltax;
            if (c1 < 1) c1 = 1; else if (c1 >= tw) c1 = tw - 1;

            Vector2 v0 = triangles[index];
            Vector2 v1 = triangles[index + 1];
            Vector2 v2 = triangles[index + 2];

            for (int r = r0; r <= r1; r++) {
                int rr = r * tw;
                float wz = (((r + 0.5f) / th) - 0.5f) * fogOfWarSize.z + fogOfWarCenter.z;
                for (int c = c0; c <= c1; c++) {
                    float wx = (((c + 0.5f) / tw) - 0.5f) * fogOfWarSize.x + fogOfWarCenter.x;
                    // Check if any triangle contains this position
                    for (int i = 0; i < indicesLength; i += 3) {
                        if (PointInTriangle(wx, wz, v0.x, v0.y, v1.x, v1.y, v2.x, v2.y)) {
                            int colorBufferPos = rr + c;
                            Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                            if (colorBuffer.a != newAlpha8 || restoreDelay > 0) {
                                if (duration > 0) {
                                    AddFogOfWarTransitionSlot(c, r, colorBuffer.a, newAlpha8, 0, duration, restoreAlpha, restoreDelay, restoreDuration);
                                } else {
                                    colorBuffer.a = newAlpha8;
                                    fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                                    requiresTextureUpload = true;
                                    if (restoreDelay > 0) {
                                        AddFogOfWarTransitionSlot(c, r, newAlpha8, restoreAlpha, restoreDelay, restoreDuration, restoreAlpha, 0, 0);
                                    }
                                }
                            }
                            break;
                        } else {
                            index += 3;
                            index %= indicesLength;
                            v0 = triangles[index];
                            v1 = triangles[index + 1];
                            v2 = triangles[index + 2];
                        }
                    }
                }
            }
        }


        float Sign(float p1x, float p1z, float p2x, float p2z, float p3x, float p3z) {
            return (p1x - p3x) * (p2z - p3z) - (p2x - p3x) * (p1z - p3z);
        }

        bool PointInTriangle(float x, float z, float v1x, float v1z, float v2x, float v2z, float v3x, float v3z) {
            float d1 = Sign(x, z, v1x, v1z, v2x, v2z);
            float d2 = Sign(x, z, v2x, v2z, v3x, v3z);
            float d3 = Sign(x, z, v3x, v3z, v1x, v1z);

            bool has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }


        /// <summary>
        /// Restores fog of war to full opacity
        /// </summary>
        /// <param name="worldPosition">World position.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="alpha">Alpha value (1f = fully opaque)</param>
        public void ResetFogOfWarAlpha(Vector3 worldPosition, float radius, float alpha = 1f) {
            if (_fogOfWarTexture == null || fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)
                return;

            Vector3 fogOfWarCenter = anchoredFogOfWarCenter;

            float tx = (worldPosition.x - fogOfWarCenter.x) / fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (worldPosition.z - fogOfWarCenter.z) / fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            float tr = radius / fogOfWarSize.z;
            int delta = (int)(th * tr);
            int deltaSqr = delta * delta;
            byte fogAlpha = (byte)(alpha * 255);
            for (int r = pz - delta; r <= pz + delta; r++) {
                if (r > 0 && r < th - 1) {
                    for (int c = px - delta; c <= px + delta; c++) {
                        if (c > 0 && c < tw - 1) {
                            int distanceSqr = (pz - r) * (pz - r) + (px - c) * (px - c);
                            if (distanceSqr <= deltaSqr) {
                                int colorBufferPos = r * tw + c;
                                Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                                colorBuffer.a = fogAlpha;
                                fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                                requiresTextureUpload = true;
                            }
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Restores fog of war to full opacity
        /// </summary>
        public void ResetFogOfWarAlpha(Bounds bounds, float alpha = 1f) {
            ResetFogOfWarAlpha(bounds.center, bounds.extents.x, bounds.extents.z, alpha);
        }

        /// <summary>
        /// Restores fog of war to full opacity
        /// </summary>
        public void ResetFogOfWarAlpha(Vector3 position, Vector3 size, float alpha = 1f) {
            ResetFogOfWarAlpha(position, size.x * 0.5f, size.z * 0.5f, alpha);
        }

        /// <summary>
        /// Restores fog of war to full opacity
        /// </summary>
        /// <param name="position">Position in world space.</param>
        /// <param name="extentsX">Half of the length of the rectangle in X-Axis.</param>
        /// <param name="extentsZ">Half of the length of the rectangle in Z-Axis.</param>
        /// <param name="alpha">Alpha value (1f = fully opaque)</param>
        public void ResetFogOfWarAlpha(Vector3 position, float extentsX, float extentsZ, float alpha = 1f) {
            if (_fogOfWarTexture == null || fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)
                return;

            Vector3 fogOfWarCenter = anchoredFogOfWarCenter;

            float tx = (position.x - fogOfWarCenter.x) / fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (position.z - fogOfWarCenter.z) / fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            float trz = extentsZ / fogOfWarSize.z;
            float trx = extentsX / fogOfWarSize.x;
            int deltaz = (int)(th * trz);
            int deltax = (int)(tw * trx);
            byte fogAlpha = (byte)(alpha * 255);
            for (int r = pz - deltaz; r <= pz + deltaz; r++) {
                if (r > 0 && r < th - 1) {
                    for (int c = px - deltax; c <= px + deltax; c++) {
                        if (c > 0 && c < tw - 1) {
                            int colorBufferPos = r * tw + c;
                            Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                            colorBuffer.a = fogAlpha;
                            fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                            requiresTextureUpload = true;
                        }
                    }
                }
            }
        }


        public void ResetFogOfWar(float alpha = 1f) {
            if (_fogOfWarTexture == null)
                return;
            int h = _fogOfWarTexture.height;
            int w = _fogOfWarTexture.width;
            int newLength = h * w;
            if (fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length != newLength) {
                fogOfWarColorBuffer = new Color32[newLength];
            }
            Color32 opaque = new Color32(255, 255, 255, (byte)(alpha * 255));
            for (int k = 0; k < newLength; k++) {
                fogOfWarColorBuffer[k] = opaque;
            }
            _fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
            _fogOfWarTexture.Apply();
            lastTransitionPos = -1;
            fowTransitionIndices.Clear();
        }

        /// <summary>
        /// Gets or set fog of war state as a Color32 buffer. The alpha channel stores the transparency of the fog at that position (0 = no fog, 1 = opaque).
        /// </summary>
        public Color32[] fogOfWarTextureData {
            get {
                return fogOfWarColorBuffer;
            }
            set {
                enableFogOfWar = true;
                fogOfWarColorBuffer = value;
                if (value == null || _fogOfWarTexture == null)
                    return;
                if (value.Length != _fogOfWarTexture.width * _fogOfWarTexture.height)
                    return;
                _fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
                _fogOfWarTexture.Apply();
            }
        }

        void AddFogOfWarTransitionSlot(int x, int y, byte initialAlpha, byte targetAlpha, float delay, float duration, byte restoreToAlpha, float restoreDelay, float restoreDuration) {

            // Check if this slot exists
            int index;
            int key = y * 64000 + x;

            if (!fowTransitionIndices.TryGetValue(key, out index)) {
                index = -1;
                for (int k = 0; k <= lastTransitionPos; k++) {
                    if (!fowTransitionList[k].enabled) {
                        index = k;
                        fowTransitionIndices[key] = index;
                        break;
                    }
                }
            }
            if (index >= 0) {
                if (fowTransitionList[index].enabled) {
                    if (fowTransitionList[index].x != x || fowTransitionList[index].y != y) {
                        index = -1;
                    } else {
                        if (fowTransitionList[index].targetAlpha <= targetAlpha && fowTransitionList[index].restoreToAlpha == restoreToAlpha && fowTransitionList[index].restoreDelay == restoreDelay && fowTransitionList[index].restoreDuration == restoreDuration) {
                            // transition running already to target alpha
                            return;
                        }
                    }
                }
            }

            if (index < 0) {
                if (lastTransitionPos >= MAX_SIMULTANEOUS_TRANSITIONS - 1)
                    return;
                index = ++lastTransitionPos;
                fowTransitionIndices[key] = index;
            }

            fowTransitionList[index].x = x;
            fowTransitionList[index].y = y;
            fowTransitionList[index].duration = duration;
            fowTransitionList[index].startTime = now;
            fowTransitionList[index].startDelay = delay;
            fowTransitionList[index].initialAlpha = initialAlpha;
            fowTransitionList[index].targetAlpha = targetAlpha;
            fowTransitionList[index].restoreToAlpha = restoreToAlpha;
            fowTransitionList[index].restoreDelay = restoreDelay;
            fowTransitionList[index].restoreDuration = restoreDuration;

            fowTransitionList[index].enabled = true;
        }


        /// <summary>
        /// Gets the current alpha value of the Fog of War at a given world position
        /// </summary>
        /// <returns>The fog of war alpha.</returns>
        /// <param name="worldPosition">World position.</param>
        public float GetFogOfWarAlpha(Vector3 worldPosition) {
            if (fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0 || _fogOfWarTexture == null)
                return 1f;

            float tx = (worldPosition.x - fogOfWarCenter.x) / fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return 1f;
            float tz = (worldPosition.z - fogOfWarCenter.z) / fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return 1f;

            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            int colorBufferPos = pz * tw + px;
            if (colorBufferPos < 0 || colorBufferPos >= fogOfWarColorBuffer.Length)
                return 1f;
            return fogOfWarColorBuffer[colorBufferPos].a / 255f;
        }

    }


}