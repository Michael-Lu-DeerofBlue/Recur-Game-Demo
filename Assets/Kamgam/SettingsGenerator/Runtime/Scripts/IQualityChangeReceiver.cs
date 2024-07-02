namespace Kamgam.SettingsGenerator
{
    public interface IQualityChangeReceiver
    {
        /// <summary>
        /// Called after the qulity level has been changed but before the setting connections are pulled.<br />
        /// Useful for changing setting values in reaction to quality changes (in case the setting is independent
        /// of the default quality settings).
        /// </summary>
        /// <param name="qualityLevel"></param>
        void OnQualityChanged(int qualityLevel);
    }
}
