using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    // We need this class file because Unity requires one file to be named exactly
    // like the class it contains.

    // See .BuiltIn, .URP or .HDRP for the specific implementations.

    public partial class ShadowDistanceConnection : ConnectionWithOptions<string>
    {
        public List<float> QualityDistances;
        public bool UseQualitySettingsAsFallback;
    }
}
