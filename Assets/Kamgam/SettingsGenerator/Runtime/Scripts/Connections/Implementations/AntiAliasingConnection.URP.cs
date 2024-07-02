#if KAMGAM_RENDER_PIPELINE_URP && !KAMGAM_RENDER_PIPELINE_HDRP

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Kamgam.SettingsGenerator
{
    // This settings is camera based. Thus we need to keep track of active cameras.

    public partial class AntiAliasingConnection : ConnectionWithOptions<string>
    {
        protected List<string> _labels;

        public AntiAliasingConnection()
        {
            CameraDetector.Instance.OnNewCameraFound += onNewCameraFound;
        }

        protected void onNewCameraFound(Camera cam)
        {
            setOnCamera(cam, lastKnownValue);
        }

        public override List<string> GetOptionLabels()
        {
            if (_labels == null)
            {
                _labels = new List<string>();
                _labels.Add("Disabled");
                _labels.Add("FAA");
                _labels.Add("SMAA");
            }
            
            return _labels;
        }

        public override void SetOptionLabels(List<string> optionLabels)
        {
            if (optionLabels == null || optionLabels.Count != 3)
            {
                Debug.LogError("Invalid new labels. Need to be three.");
                return;
            }

            _labels = optionLabels;
        }

        public override void RefreshOptionLabels()
        {
            _labels = null;
            GetOptionLabels();
        }

        /// <summary>
        /// Returns 0 if no camera is active.
        /// </summary>
        /// <returns></returns>
        public override int Get()
        {
            if (Camera.main == null)
                return 0;

            // Fetch from current camera
            var settings = Camera.main.GetComponent<UniversalAdditionalCameraData>();
            if (settings == null)
                return 0;

            switch (settings.antialiasing)
            {
                case AntialiasingMode.None:
                    return 0;
                case AntialiasingMode.FastApproximateAntialiasing:
                    return 1;
                case AntialiasingMode.SubpixelMorphologicalAntiAliasing:
                    return 2;
                default:
                    return 0;
            }
        }

        public override void Set(int index)
        {
            var cameras = Camera.allCameras;
            foreach (var cam in cameras)
            {
                if (cam.gameObject.activeInHierarchy && cam.isActiveAndEnabled)
                {
                    setOnCamera(cam, index);
                }
            }

            NotifyListenersIfChanged(index);
        }

        private static void setOnCamera(Camera cam, int index)
        {
            var settings = cam.GetComponent<UniversalAdditionalCameraData>();
            if (settings == null)
                return;

#if UNITY_EDITOR
            if (!settings.renderPostProcessing)
            {
                Logger.Log("Post Processing is OFF on the camera (" + cam.name + "). Antialiasing setting will have no visible effect on this camera.");
            }
#endif

            if (index == 0)
                settings.antialiasing = AntialiasingMode.None;
            if (index == 1)
                settings.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
            if (index == 2)
                settings.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
        }
    }
}

#endif