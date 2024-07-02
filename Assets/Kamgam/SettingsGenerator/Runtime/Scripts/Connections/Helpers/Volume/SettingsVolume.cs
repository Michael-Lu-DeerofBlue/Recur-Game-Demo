// The custom defines KAMGAM_RENDER_PIPELINE_CORE and KAMGAM_RENDER_PIPELINE_HDRP, KAMGAM_RENDER_PIPELINE_URP, KAMGAM_POST_PRO_BUILTIN
// Are coming from the runtime assembly definition (see Version Defines).

// It's a bit of an ifdef mess, but a lot of the post pro code is similar.
// Should it become too complicated then we'll split this into partial classes.

using System.Collections.Generic;
using UnityEngine;

// The && parts of the define make sure the PostProStackV2 is NOT used if HDRP or URP are present.
#if KAMGAM_POST_PRO_BUILTIN && (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP)
using UnityEngine.Rendering.PostProcessing;
using Volume = UnityEngine.Rendering.PostProcessing.PostProcessVolume;
using VolumeProfile = UnityEngine.Rendering.PostProcessing.PostProcessProfile;
using VolumeComponent = UnityEngine.Rendering.PostProcessing.PostProcessEffectSettings;
#endif

#if KAMGAM_RENDER_PIPELINE_HDRP || KAMGAM_RENDER_PIPELINE_URP
using UnityEngine.Rendering;
using Volume = UnityEngine.Rendering.Volume;
using VolumeProfile = UnityEngine.Rendering.VolumeProfile;
using VolumeComponent = UnityEngine.Rendering.VolumeComponent;

#if KAMGAM_RENDER_PIPELINE_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif

#if KAMGAM_RENDER_PIPELINE_URP
using UnityEngine.Rendering.Universal;
#endif

#endif

namespace Kamgam.SettingsGenerator
{
    public class SettingsVolume : MonoBehaviour
    {
#if KAMGAM_RENDER_PIPELINE_HDRP || KAMGAM_RENDER_PIPELINE_URP || KAMGAM_POST_PRO_BUILTIN
        private static SettingsVolume _instance;
        public static SettingsVolume Instance
        {
            get
            {
                if (!_instance)
                {
#if UNITY_EDITOR
                    // Keep the instance null outside of play mode to avoid leaking
                    // instances into the scene.
                    if (!UnityEditor.EditorApplication.isPlaying)
                    {
                        return null;
                    }
#endif
                    _instance = new GameObject().AddComponent<SettingsVolume>();
                    _instance.name = _instance.GetType().ToString();
                    _instance.createVolume();

#if UNITY_EDITOR
                    _instance.hideFlags = HideFlags.DontSave;
                    if (UnityEditor.EditorApplication.isPlaying)
                    {
#endif
                        DontDestroyOnLoad(_instance.gameObject);
#if UNITY_EDITOR
                    }
#endif
                }
                return _instance;
            }
        }

        [System.NonSerialized]
        public Volume Volume;

        /// <summary>
        /// The priority with which the volume will be created.<br />
        /// If the volume has already been created then it will
        /// update the priority of the existing volume.
        /// </summary>
        static float _defaultPriority = 99;
        public static float Priority
        {
            get
            {
                if (_instance != null)
                {
                    return _instance.Volume.priority;
                }
                else
                {
                    return _defaultPriority;
                }
            }

            set
            {
                _defaultPriority = value;
                if (_instance != null)
                {
                    _instance.Volume.priority = value;
                }
            }
        }

        /// <summary>
        /// Controls can be used as a proxy for multiple component or if components need
        /// to be coordinated if changed.
        /// </summary>
        protected List<ISettingsVolumeControl> _controls;

        protected virtual void createVolume()
        {
            Volume = gameObject.AddComponent<Volume>();
            Volume.priority = _defaultPriority;
            Volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
            Volume.isGlobal = true;
        }

        /// <summary>
        /// The PostProcessing
        /// </summary>
        public void MatchMainCameraLayer()
        {
            // The && parts of the define make sure the PostProStackV2 is NOT used if HDRP or URP are present.
#if KAMGAM_POST_PRO_BUILTIN && (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP)
            if (Camera.main != null && Volume != null)
            {
                var ppLayer = Camera.main.gameObject.GetComponent<PostProcessLayer>();
                if (ppLayer != null)
                {
                    // Find the first layer included in the mask and use it.
                    int layerIndex = LayerUtils.GetIndexOfFirstLayerInMask(ppLayer.volumeLayer, -1);
                    if (layerIndex >= 0)
                    {
                        Volume.gameObject.layer = layerIndex;
                    }
                }
            }
#elif KAMGAM_RENDER_PIPELINE_HDRP || KAMGAM_RENDER_PIPELINE_URP
            if (Camera.main != null && Volume != null)
            {
#if KAMGAM_RENDER_PIPELINE_HDRP
                var data = Camera.main.GetComponent<HDAdditionalCameraData>();
#else
                var data = Camera.main.GetComponent<UniversalAdditionalCameraData>();
#endif
                int layerIndex = LayerUtils.GetIndexOfFirstLayerInMask(data.volumeLayerMask, -1);
                if (layerIndex >= 0)
                {
                    Volume.gameObject.layer = layerIndex;
                }
            }
#endif
        }

        /// <summary>
        /// Gets or creates the requested VolumeComponent.
        /// </summary>
        /// <typeparam name="TComp"></typeparam>
        /// <returns></returns>
        public TComp GetOrAddComponent<TComp>() where TComp : VolumeComponent
        {
            TComp component;
            // The && parts of the define make sure the PostProStackV2 is NOT used if HDRP or URP are present.
#if KAMGAM_POST_PRO_BUILTIN && (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP)
            if (!Volume.profile.TryGetSettings<TComp>(out component))
            {
                component = Volume.profile.AddSettings<TComp>();
            }
#else
            if (!Volume.profile.TryGet<TComp>(out component))
            {
                component = Volume.profile.Add<TComp>();
            }
#endif

            return component;
        }

        /// <summary>
        /// Gets or created the requested ISettingsVolumeControl.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetOrCreateControl<T>() where T : ISettingsVolumeControl, new()
        {
            return GetOrCreateControl<T>(out _);
        }

        /// <summary>
        /// Gets or created the requested ISettingsVolumeControl.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isNew">Whether or not the control was newly created (true if created, false if it already existed)</param>
        /// <returns></returns>
        public T GetOrCreateControl<T>(out bool isNew) where T : ISettingsVolumeControl, new()
        {
            if (_controls == null)
            {
                _controls = new List<ISettingsVolumeControl>();
            }

            foreach (var obj in _controls)
            {
                if (obj is T)
                {
                    isNew = false;
                    return (T)obj;
                }
            }

            var control = new T();
            control.Initialize(this);
            _controls.Add(control);
            isNew = true;
            return control;
        }

        /// <summary>
        /// Returns the active component of the first found global volume or a volume that is child of the main camera.<br />
        /// The SettingsVolume itself is excluded from the search.<br />
        /// May return NULL if no component of type T is found or active.<br />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="useStackAsFallback">Only used in HDRP and URP.</param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public T FindDefaultVolumeComponent<T>(bool useStackAsFallback = false, int layerMask = Physics.AllLayers) where T : VolumeComponent
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return null;
            // The && parts of the define make sure the PostProStackV2 is NOT used if HDRP or URP are present.
#if KAMGAM_POST_PRO_BUILTIN && (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP)
            var postProLayer = Camera.main.GetComponent<PostProcessLayer>();
            List<Volume> volumes = new List<Volume>();
            if (postProLayer != null)
            {
                Post​Process​Manager.instance.GetActiveVolumes(postProLayer, volumes, skipDisabled: true, skipZeroWeight: true);
            }
#else
            var volumes = VolumeManager.instance.GetVolumes(layerMask);
#endif
            foreach (var volume in volumes)
            {
                // It is good practice to not use global volumes but local volumes which
                // are attached to the camera. So we allow those too.
                bool volumeIsAlignedWithMainCamera = mainCamera != null && (volume.transform.IsChildOf(mainCamera.transform) || volume.transform == mainCamera.transform);
                if (!volume.isGlobal && !volumeIsAlignedWithMainCamera)
                    continue;

                if (!volume.isActiveAndEnabled)
                    continue;

                if (volume.profile == null)
                    continue;

                // Skip self
                if (volume == this.Volume)
                    continue;

                T comp;
                // The && parts of the define make sure the PostProStackV2 is NOT used if HDRP or URP are present.
#if KAMGAM_POST_PRO_BUILTIN && (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP)
                if (volume.profile.TryGetSettings(out comp))
#else
                if (volume.profile.TryGet(out comp))
#endif
                {
                    if (!comp.active)
                        continue;

                    return comp;
                }
            }

#if KAMGAM_RENDER_PIPELINE_HDRP || KAMGAM_RENDER_PIPELINE_URP
            // Fall back on the stack at the main camera position if no volume was found.
            if (useStackAsFallback)
            {

                // Unregister this override volume (we do not want the values on this volume to pollute the result).
                if (Camera.main != null)
                {
                    VolumeManager.instance.Unregister(Volume, Camera.main.gameObject.layer);

                    VolumeManager.instance.Update(VolumeManager.instance.stack, Camera.main.transform, Physics.AllLayers);
                    if (VolumeManager.instance.stack != null)
                    {
                        var component = VolumeManager.instance.stack.GetComponent<T>();

                        // Reregister the volume
                        VolumeManager.instance.Register(Volume, Camera.main.gameObject.layer);

                        return component;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    var component = VolumeManager.instance.stack.GetComponent<T>();
      
                    return component;
                }
            }
#endif

            return null;
        }

#endif
    }
}