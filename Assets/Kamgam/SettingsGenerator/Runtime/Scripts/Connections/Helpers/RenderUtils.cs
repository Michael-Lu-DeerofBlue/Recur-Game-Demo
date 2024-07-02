using UnityEngine;
using UnityEngine.Rendering;

namespace Kamgam.SettingsGenerator
{
    public static class RenderUtils
    {
        public enum RenderPipe { BuiltIn, URP, HDRP }

        public static RenderPipe GetCurrentRenderPipeline()
        {
            // The render pipeline selection process mirrors how Unity selected the active RP.
            // See "Determining the active render pipeline" under
            // https://docs.unity3d.com/Manual/srp-setting-render-pipeline-asset.html

            // Try to get the render pipeline asset for the current quality.
            RenderPipelineAsset rpa = GraphicsSettings.currentRenderPipeline;
            // If there is none defined for the current quality then fall back on the default (will be null if Built-In is used).
            if (rpa == null)
            {
                rpa = GraphicsSettings.defaultRenderPipeline;
            }
            if (rpa != null)
            {
                switch (rpa.GetType().Name)
                {
                    case "UniversalRenderPipelineAsset": return RenderPipe.URP;
                    case "HDRenderPipelineAsset": return RenderPipe.HDRP;
                }
            }

            return RenderPipe.BuiltIn;
        }

        static Camera[] _tmpAllCameras = new Camera[10];

        public static int GetAllCameras(out Camera[] cameras)
        {
            int allCamerasCount = Camera.allCamerasCount;

            // Alloc new array only if needed (increase size by 5)
            if (allCamerasCount > _tmpAllCameras.Length)
            {
                _tmpAllCameras = new Camera[allCamerasCount + 5];
            }

            // Get cameras
            Camera.GetAllCameras(_tmpAllCameras);

            // Null out old references
            for (int i = _tmpAllCameras.Length - 1; i >= 0; i--)
            {
                if (i >= allCamerasCount)
                {
                    _tmpAllCameras[i] = null;
                }
            }

            cameras = _tmpAllCameras;
            return allCamerasCount;
        }

        public static Camera GetCurrentRenderingCamera(bool checkForMarker)
        {
            int allCamerasCount = 0;
            Camera[] cameras = null;

            if (checkForMarker)
            {
                allCamerasCount = GetAllCameras(out cameras);

                // Check if we find a camera that has the settings camera marker on it
                for (int i = cameras.Length - 1; i >= 0; i--)
                {
                    // Null out old references
                    if (i >= allCamerasCount)
                    {
                        cameras[i] = null;
                        continue;
                    }

                    var cCam = cameras[i];
                    if (cCam.TryGetComponent<SettingsMainCameraMarker>(out var marker))
                    {
                        return cCam;
                    }
                }
            }

            var cam = Camera.main;
            if (cam == null)
            {
                if (!checkForMarker)
                {
                    allCamerasCount = GetAllCameras(out cameras);
                }

                // We sort by depth and start from the back because we assume
                // that among cameras with equal depth the last takes precedence.
                float maxDepth = float.MinValue;
                for (int i = cameras.Length - 1; i >= 0; i--)
                {
                    // Null out old references
                    if (i >= allCamerasCount)
                    {
                        cameras[i] = null;
                        continue;
                    }

                    var cCam = cameras[i];

                    if (!cCam.isActiveAndEnabled)
                        continue;

                    // Only take full screen cameras that are not rendering into render textures
                    if (cCam.depth > maxDepth && cCam.targetTexture == null && cCam.rect.width >= 1f && cCam.rect.height >= 1f)
                    {
                        maxDepth = cCam.depth;
                        cam = cCam;
                    }
                }
            }

            return cam;
        }
    }
}
