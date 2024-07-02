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

    public partial class GammaVolumeConnection : Connection<float>
    {
        public GammaVolumeConnection()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance != null)
            {
                var effect = SettingsVolume.Instance.GetOrAddComponent<ColorGrading>();
                if (effect != null)
                {
                    effect.active = true;
                    effect.enabled.Override(true);
                    effect.enabled.value = true;
                }
            }
        }

        public override float Get()
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return 1f;

            var effect = SettingsVolume.Instance.GetOrAddComponent<ColorGrading>();
            if (effect != null)
            {
                var value = effect.gamma.value.w;
                return value;
            }

            return 1f;
        }

        public override void Set(float gamma)
        {
            // If this is called during EDIT Mode then the Instance will be null.
            if (SettingsVolume.Instance == null)
                return;

            var effect = SettingsVolume.Instance.GetOrAddComponent<ColorGrading>();
            if (effect != null)
            {
                effect.active = true;
                effect.enabled.Override(true);
                effect.enabled.value = true;

                var value = effect.gamma.value;
                value.w = gamma;
                effect.gamma.Override(value);
            }

            NotifyListenersIfChanged(gamma);
        }
    }
}

#else

// Fallback if no PostProcessing stack is installed.

using UnityEngine;
namespace Kamgam.SettingsGenerator
{
    public partial class GammaVolumeConnection : Connection<float>
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