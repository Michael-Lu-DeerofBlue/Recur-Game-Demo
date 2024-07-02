// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)

// Is the PostProcessing stack installed?
#if KAMGAM_POST_PRO_BUILTIN
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Kamgam.SettingsGenerator
{
    public partial class VignetteConnection
#if KAMGAM_POST_PRO_BUILTIN
        : PostProcessingEffectEnabledConnectionBuiltIn<Vignette>
    { }
#else
        : Connection<bool>
        {
            public override bool Get() { return false; }
            public override void Set(bool value) {
#if UNITY_EDITOR
            var name = this.GetType().Name;
            Logger.LogWarning(
                name + " (BuiltIn): There is no PostProcessing stack installed. This will do nothing.\n" +
                "Here is how to install it: https://docs.unity3d.com/Packages/com.unity.postprocessing@3.0/manual/Installation.html"
                );
#endif
            }
        }
#endif
}

#endif