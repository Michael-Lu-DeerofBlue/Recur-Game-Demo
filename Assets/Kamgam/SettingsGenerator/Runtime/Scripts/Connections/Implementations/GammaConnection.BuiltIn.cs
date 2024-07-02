// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)

// Is the PostProcessing stack installed?
#if KAMGAM_POST_PRO_BUILTIN

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Kamgam.SettingsGenerator
{
    // The post processing effects are camera based. Thus we need to keep track of active cameras.

    public partial class GammaConnection : Connection<float>
    {
        public GammaConnection()
        {
            CameraDetector.Instance.OnNewCameraFound += onNewCameraFound;
        }

        protected void onNewCameraFound(Camera cam)
        {
            setOnCamera(cam, lastKnownValue);
        }

        public override float Get()
        {
            if (Camera.main == null)
                return 0f;

            var volume = Camera.main.GetComponentInChildren<PostProcessVolume>();
            if (volume != null && volume.profile != null)
            {
                ColorGrading effect;
                volume.profile.TryGetSettings(out effect);
                if (effect != null)
                {
                    if (effect.gamma.overrideState)
                    {
                        float gamma = effect.gamma.value.w;
                        return gamma;
                    }
                    else
                    {
                        return 0f;
                    }
                }
            }

            return 0f;
        }

        public override void Set(float gamma)
        {
            bool found = false;

            var cameras = Camera.allCameras;
            foreach (var cam in cameras)
            {
                if (cam.gameObject.activeInHierarchy && cam.isActiveAndEnabled)
                {
                    found |= setOnCamera(cam, gamma);
                }
            }

            if (!found)
            {
#if UNITY_EDITOR
                var effectName = typeof(ColorGrading).Name;
                var name = this.GetType().Name;
                Logger.LogWarning(name + ": There was no '" + effectName + "' PostPro (BuiltIn) effect found. " +
                    "Please add a PostPro Volume with a profile containing '" + effectName + "' to your camera, make sure it is active and the layers do match and, if the volume is not global, add a trigger collider.\n\n" +
                    "Find out more here: https://docs.unity3d.com/Packages/com.unity.postprocessing@3.0/manual/Quick-start.html");
#endif
            }

            NotifyListenersIfChanged(gamma);
        }

        /// <summary>
        /// Returns true if the setting could be set.<br />
        /// Returns false if cam is null or if there is no PostPro volume with AO on it.
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        private static bool setOnCamera(Camera cam, float gamma)
        {
            if (cam == null)
                return false;

            var volume = cam.GetComponentInChildren<PostProcessVolume>();
            if (volume != null && volume.profile != null)
            {
                ColorGrading effect;
                volume.profile.TryGetSettings(out effect);
                if (effect != null)
                {
                    effect.active = true;
                    effect.enabled.Override(true);
                    effect.enabled.value = true;
                    
                    var value = effect.gamma.value;
                    value.w = gamma;
                    effect.gamma.Override(value);

                    return true;
                }
            }

            return false;
        }
    }
}

#else

// Fallback if no PostProcessing stack is installed.

using UnityEngine;
namespace Kamgam.SettingsGenerator
{
    public partial class GammaConnection : Connection<float>
    {
        public override float Get()
        {
            return 0f;
        }

        public override void Set(float gamma)
        {
#if UNITY_EDITOR
            var name = this.GetType().Name;
            Logger.LogWarning(
                name + " (BuiltIn): There is no PostProcessing stack installed. This will do nothing.\n" +
                "Here is how to install it: https://docs.unity3d.com/Packages/com.unity.postprocessing@3.0/manual/Installation.html"
                );
#endif
        }
    }
}

#endif

#endif