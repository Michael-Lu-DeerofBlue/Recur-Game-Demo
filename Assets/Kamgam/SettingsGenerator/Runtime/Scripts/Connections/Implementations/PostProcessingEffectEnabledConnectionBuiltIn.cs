// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)

// Is the PostProcessing stack installed?
#if KAMGAM_POST_PRO_BUILTIN

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Kamgam.SettingsGenerator
{
    // The post processing effects are volume based.

    /// <summary>
    /// Generic class which allows to enable/disable any post processing effect on the camera.
    /// </summary>
    /// <typeparam name="TEffect"></typeparam>
    public partial class PostProcessingEffectEnabledConnectionBuiltIn<TEffect> : Connection<bool> where TEffect : PostProcessEffectSettings
    {
        public PostProcessingEffectEnabledConnectionBuiltIn()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance != null)
            {
                var effect = SettingsVolume.Instance.GetOrAddComponent<TEffect>();
                if (effect != null)
                {
                    // Set inactive (will be activate if the effect should be disabled).
                    effect.active = false;

                    // Set it to disabled (this will override the other volumes to NOT using this setting).
                    effect.enabled.Override(true);
                    effect.enabled.value = false;
                }
            }
        }

        public override bool Get()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return false;

            var effect = SettingsVolume.Instance.GetOrAddComponent<TEffect>();
            if (effect != null)
            {
                // invert because active means "set disabled".
                return !effect.active;
            }

            return false;
        }

        public override void Set(bool enable)
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return;

            var effect = SettingsVolume.Instance.GetOrAddComponent<TEffect>();
            if (effect != null)
            {
                // invert enable because active means "set disabled".
                effect.active = !enable;

                // Make sure the settings volume layer matches the camera layer.
                SettingsVolume.Instance.MatchMainCameraLayer();
            }

            if (enable)
            {
#if UNITY_EDITOR
                // If the user tries to enable it but there is no default volume with this effect then show a log message.
                var defaultEffect = SettingsVolume.Instance.FindDefaultVolumeComponent<TEffect>();
                if (defaultEffect == null)
                {
                    var effectName = typeof(TEffect).Name;
                    var name = this.GetType().Name;
                    Logger.LogWarning(name + ": There was no active '" + effectName + "' PostPro effect (BuiltIn) found. " +
                        "Please add a PostPro Volume with a profile containing '" + effectName + "' to your camera, make sure it is active and the layers do match and, if the volume is not global, add a trigger collider.\n\n" +
                        "Find out more here: https://docs.unity3d.com/Packages/com.unity.postprocessing@3.0/manual/Quick-start.html");
                }
#endif
            }

            NotifyListenersIfChanged(enable);
        }
    }
}

#else

// Fallback if no PostProcessing stack is installed.

using UnityEngine;
namespace Kamgam.SettingsGenerator
{
    public partial class PostProcessingEffectEnabledConnection<TEffect> : Connection<bool>
    {
        public override bool Get()
        {
            return false;
        }

        public override void Set(bool enable)
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