// If it's neither URP nor HDRP then it is the old BuiltIn renderer
// If both HDRP and URP are set then we also assume BuiltIn until this ambiguity is resolved by the AssemblyDefinitionUpdater
#if (!KAMGAM_RENDER_PIPELINE_HDRP && !KAMGAM_RENDER_PIPELINE_URP) || (KAMGAM_RENDER_PIPELINE_URP && KAMGAM_RENDER_PIPELINE_HDRP)

using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public partial class ShadowConnection
    {
        ShadowQuality previousQuality;

        public override bool Get()
        {
            // Remember ON setting
            if (QualitySettings.shadows != ShadowQuality.Disable)
                previousQuality = QualitySettings.shadows;

            return QualitySettings.shadows != ShadowQuality.Disable;
        }

        public override void Set(bool enable)
        {
            // Remember ON setting
            if (QualitySettings.shadows != ShadowQuality.Disable)
                previousQuality = QualitySettings.shadows;

            if (!enable)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
            }
            else
            {
                if (previousQuality != ShadowQuality.Disable)
                    QualitySettings.shadows = previousQuality;
                else
                    QualitySettings.shadows = ShadowQuality.HardOnly;
            }
            
            NotifyListenersIfChanged(enable);
        }
    }
}
#endif
