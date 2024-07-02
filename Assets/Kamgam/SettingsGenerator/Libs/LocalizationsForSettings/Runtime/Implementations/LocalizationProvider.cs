using UnityEngine;

namespace Kamgam.LocalizationForSettings
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LocalizationProvider", menuName = "SettingsGenerator/LocalizationProvider", order = 5)]
    public class LocalizationProvider : ScriptableObject, ILocalizationProvider
    {
        [SerializeField]
        protected Localization _localization;

        public static bool IsUsable(LocalizationProvider provider)
        {
            return provider != null && provider.HasLocalization();
        }

        public bool HasLocalization()
        {
            return _localization != null;
        }

        public ILocalization GetLocalization()
        {
            if (_localization == null)
            {
                _localization = new Localization();
            }

            return _localization;
        }

        public string Get(string term)
        {
            return Get(term, null);
        }

        public string Get(string term, string defaultValue)
        {
            if(HasLocalization() && GetLocalization().HasTerm(term))
            {
                return GetLocalization().Get(term);
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
