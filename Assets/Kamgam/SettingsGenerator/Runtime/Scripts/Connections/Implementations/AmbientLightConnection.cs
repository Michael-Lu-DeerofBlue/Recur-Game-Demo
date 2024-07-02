namespace Kamgam.SettingsGenerator
{
    // We need this class file because Unity requires one file to be named exactly
    // like the class it contains.

    // See .BuiltIn, .URP or .HDRP for the specific implementations.

    public partial class AmbientLightConnection : Connection<float>
    {
        /// <summary>
        /// The allowed min ambient color intensity.<br />
        /// Only used if ambient light is set to color.<br />
        /// Useful to avoid losing the color info if ambient is set to black.<br />
        /// NOTICE: This is only used in Built-In and URP, not HDRP.
        /// </summary>
        public float MinColorIntensity = 0.01f;

        /// <summary>
        /// Max color intensity. 2f by default. You may want to change it depending on your ambient color (HD or SD).<br />
        /// Only used if ambient light is set to color.<br />
        /// NOTICE: This is only used in Built-In and URP, not HDRP.
        /// </summary>
        public float MaxColorIntensity = 2f;
    }
}
