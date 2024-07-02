namespace Kamgam.LocalizationForSettings
{
    public interface ILocalizationProvider
    {
        bool HasLocalization();
        ILocalization GetLocalization();
    }
}
