namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Add it to a connection if that connection that needs to know which settings object it belongs too.<br />
    /// The most notable one is the QualityConnection. If this is added to a connection then the settings
    /// system will notice and call SetSettings() after load but before InitializeConnection().
    /// </summary>
    public interface IConnectionWithSettingsAccess
    {
        public void SetSettings(Settings settings);
        public Settings GetSettings();
    }
}
