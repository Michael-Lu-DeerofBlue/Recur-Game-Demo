using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
#if AEG_DLSS
using AEG.DLSS;
using UnityEngine.NVIDIA;
#endif
#if KAMGAM_POST_PRO_BUILTIN
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Kamgam.SettingsGenerator
{
    public class AlteregoDLSSConnection : ConnectionWithOptions<string>
    {
        protected List<string> _labels;

        /// <summary>
        /// If enabled then the camera detection will search (an prefer) cameras with the SettingsMainCameraMarker component on it.
        /// </summary>
        public bool CheckForCameraMarker = true;

        public AlteregoDLSSConnection()
        {
#if AEG_DLSS
            if (!IsSupported())
            {
                Logger.LogWarning("AlteregoDLSSConnection: Alterego DLSS is not supported. Please contact Alterego Games for more info and support.");
            }
#else
            Logger.LogWarning("AlteregoDLSSConnection: Alterego DLSS is not yet set up. Please add DLSS as a post processing effect and install the NVIDIA package. Please contact Alterego Games for more info and support.");
#endif
        }

#if AEG_DLSS
        protected GraphicsDevice _device;
        public GraphicsDevice device
        {
            get
            {
                if (_device == null)
                {
                    setupDevice();
                }

                return _device;
            }
        }

        protected void setupDevice()
        {
            if (!NVUnityPlugin.IsLoaded())
                return;

            if (!SystemInfo.graphicsDeviceVendor.ToLowerInvariant().Contains("nvidia"))
                return;

            _device = GraphicsDevice.CreateGraphicsDevice();
        }
#endif

        /// <summary>
        /// Checks wether DLSS is compatible using the current build settings 
        /// </summary>
        public bool IsSupported()
        {
#if AEG_DLSS
            // Built-In
            // If it's neither URP nor HDRP then it is the old BuiltIn renderer
            // If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)
            if (device == null)
            {
                return false;
            }

            if (device.IsFeatureAvailable(GraphicsDeviceFeature.DLSS))
            {
                return true;
            }
            return false;
#elif KAMGAM_RENDER_PIPELINE_URP
            // URP
            var cam = RenderUtils.GetCurrentRenderingCamera(CheckForCameraMarker);
            if (cam != null)
            {
                if (cam.TryGetComponent<DLSS_URP>(out DLSS_URP dlssURP))
                {
                    return dlssURP.IsSupported();
                }
                else
                {
                    Logger.LogWarning("AlteregoDLSSConnection: No DLSS_URP component found on current rendering camera.");
                    return false;
                }
            }
            else
            {
                return false;
            }
#else
            // HDRP
            return false;
#endif

#else
            return false;
#endif
        }

        public override List<string> GetOptionLabels()
        {
#if AEG_DLSS
            if (!IsSupported())
                return _labels;

            if (_labels == null)
            {
                _labels = new List<string>();
                var qualityNames = System.Enum.GetNames(typeof(DLSS_Quality));
                foreach (var name in qualityNames)
                {
                    _labels.Add(name);
                }
            }
#else
            Logger.LogWarning("AlteregoDLSSConnection: Alterego DLSS is not yet set up. Please add DLSS as a post processing effect and install the NVIDIA package. Please contact Alterego Games for more info and support.");
#endif
            return _labels;
        }

        protected List<int> _enumOptionsAsIntegers = new List<int>(6);

        protected List<int> getOptionsEnumList()
        {
#if AEG_DLSS
            if (!IsSupported())
                return _enumOptionsAsIntegers;

            if (_enumOptionsAsIntegers.Count == 0)
            {
                var qualityValues = System.Enum.GetValues(typeof(DLSS_Quality));
                foreach (var value in qualityValues)
                {
                    _enumOptionsAsIntegers.Add((int)value);
                }
            }
#else
            Logger.LogWarning("AlteregoDLSSConnection: Alterego DLSS is not yet set up. Please add DLSS as a post processing effect and install the NVIDIA package. Please contact Alterego Games for more info and support.");
#endif
            return _enumOptionsAsIntegers;
        }

#if AEG_DLSS
        protected int getOptionIndexForQuality(DLSS_Quality quality)
        {
            var list = getOptionsEnumList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == (int)quality)
                    return i;
            }

            return 0;
        }

        protected DLSS_Quality getQualityForOptionIndex(int index)
        {
            return (DLSS_Quality)getOptionsEnumList()[index];
        }
#endif

        public override void SetOptionLabels(List<string> optionLabels)
        {
            if (optionLabels == null || optionLabels.Count != 4)
            {
                Debug.LogError("Invalid new labels. Need to be four.");
                return;
            }

            _labels = optionLabels;
        }

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
        }




        // If it's neither URP nor HDRP then it is the old BuiltIn renderer
        // If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)
        ///////////////
        // Built-Int
        ///////////////
        public override int Get()
        {
#if AEG_DLSS
            if (!IsSupported())
                return 0;

            var cam = RenderUtils.GetCurrentRenderingCamera(CheckForCameraMarker);
            if (cam == null)
                return 0;

#if KAMGAM_POST_PRO_BUILTIN
            if (cam.TryGetComponent<PostProcessLayer>(out var ppLayer))
            {
                var quality = getDLSSQuality(ppLayer);
                return getOptionIndexForQuality(quality);
            }
#endif

            return 0;
#else
            Logger.LogWarning("AlteregoDLSSConnection: Alterego DLSS is not yet set up. Please add DLSS as a post processing effect and install the NVIDIA package. Please contact Alterego Games for more info and support.");
            return 0;
#endif
        }

        public override void Set(int index)
        {
#if AEG_DLSS
            if (IsSupported())
            {
#if KAMGAM_POST_PRO_BUILTIN
                var cam = RenderUtils.GetCurrentRenderingCamera(CheckForCameraMarker);
                if (cam != null && cam.TryGetComponent<PostProcessLayer>(out var ppLayer))
                {
                    var quality = (DLSS_Quality)getQualityForOptionIndex(index);

                    // Set anti aliasing to DLSS mode if necessary.
                    if (ppLayer.antialiasingMode == PostProcessLayer.Antialiasing.None && quality != DLSS_Quality.Off)
                        ppLayer.antialiasingMode = (PostProcessLayer.Antialiasing)System.Enum.Parse(typeof(PostProcessLayer.Antialiasing), "DLSS");

                    setDLSSQuality(ppLayer, quality);
                }
#endif
            }
#endif
            NotifyListenersIfChanged(index);
        }

#if AEG_DLSS && KAMGAM_POST_PRO_BUILTIN 
        /// <summary>
        /// Method to access the quality of the patched Alterego Post Processing Layer class.
        /// </summary>
        /// <param name="ppLayer"></param>
        /// <returns>DLSS_Quality</returns>
        private static DLSS_Quality getDLSSQuality(PostProcessLayer ppLayer) // TODO: DLSS is also a type inside the custom post pro package.
        {
            var dlssField = getDLSSField(ppLayer);
            if (dlssField == null)
                return DLSS_Quality.Off;

            var dlss = dlssField.GetValue(ppLayer);
            if (dlss == null)
                return DLSS_Quality.Off;

            var qualityField = getQualityField(dlss);
            if (qualityField == null)
                return DLSS_Quality.Off;

            return (DLSS_Quality)qualityField.GetValue(dlss);
        }

        private static void setDLSSQuality(PostProcessLayer ppLayer, DLSS_Quality quality) // TODO: DLSS is also a type inside the custom post pro package.
        {
            var dlssField = getDLSSField(ppLayer);
            if (dlssField == null)
                return;

            var dlss = dlssField.GetValue(ppLayer);
            if (dlss == null)
                return;

            var qualityField = getQualityField(dlss);
            if (qualityField == null)
                return;

            qualityField.SetValue(dlss, quality);
        }

        private static System.Reflection.FieldInfo getQualityField(object dlss)
        {
            return dlss.GetType().GetField("qualityMode", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        }

        private static System.Reflection.FieldInfo getDLSSField(PostProcessLayer ppLayer)
        {
            return ppLayer.GetType().GetField("dlss", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        }
#endif


#elif KAMGAM_RENDER_PIPELINE_URP
        ///////////////
        // U R P
        ///////////////
        public override int Get()
        {
#if AEG_DLSS
            if (!IsSupported())
                return 0;

            var cam = RenderUtils.GetCurrentRenderingCamera(CheckForCameraMarker);
            if (cam == null)
                return 0;

            if (cam.TryGetComponent<DLSS_URP>(out DLSS_URP dlssURP))
            {
                getOptionIndexForQuality(dlssURP.DLSSQuality);
            }
            else
            {
                Logger.LogWarning("AlteregoDLSSConnection: No DLSS_URP component found on current rendering camera.");
            }

            return 0;
#else
            Logger.LogWarning("AlteregoDLSSConnection: Alterego DLSS is not yet set up. Please add DLSS as a post processing effect and install the NVIDIA package. Please contact Alterego Games for more info and support.");
            return 0;
#endif
        }

        public override void Set(int index)
        {
#if AEG_DLSS
            if (IsSupported())
            {
                var cam = RenderUtils.GetCurrentRenderingCamera(CheckForCameraMarker);
                if (cam != null && cam.TryGetComponent<DLSS_URP>(out var dlssURP))
                {
                    var quality = getQualityForOptionIndex(index);
                    dlssURP.OnSetQuality(quality);
                }
                else
                {
                    Logger.LogWarning("AlteregoDLSSConnection: No DLSS_URP component found on current rendering camera.");
                }
            }
#endif
            NotifyListenersIfChanged(index);
        }



#else
        //////////////////
        // HDRP
        //////////////////

        public override int Get()
        {
            return 0;
        }

        public override void Set(int index)
        {
            NotifyListenersIfChanged(index);
        }
#endif




    }
}