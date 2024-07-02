namespace Kamgam.SettingsGenerator
{
    // We need this class file because Unity requires one file to be named exactly
    // like the class it contains.

    // The GammaVolumeConnection is a new version of the GammaConnection. The URP and HDRP implementations
    // are actually identical but the Built-In implemementation uses the global SettingsVolume instead of
    // local camera volumes. The GammaConnection will probably be deprectated in the next major release.

    // See .BuiltIn, .URP or .HDRP for the specific implementations.

    public partial class GammaVolumeConnection
    {
    }
}
