using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.LocalizationForSettings
{
    /// <summary>
    /// Usually the localization does NOT need and initialization as it is
    /// initialized on demand. However, if you want to bind another localization
    /// solution to it then this may be a handy templpate.
    /// </summary>
    [DefaultExecutionOrder(-11)] // Not strictly necessary. Just to be sure it runs before the settings are initialized.
    public class LocalizationInitializerTemplate : MonoBehaviour
    {
        /// <summary>
        /// Don't forget to hook this up with the right provider in the inspector.
        /// </summary>
        public LocalizationProvider Provider;

        // Uncomment this if you want to use I2 Localizatino for example.
        /*
        public void Awake()
        {
            var localization = Provider.GetLocalization();

            // Tell the settings localization to ask I2L for translations.
            localization.SetDynamicLocalizationCallback(dynamicLocalization);

            // Hook into I2 Localization and listen for language changes.
            I2.Loc.LocalizationManager.OnLocalizeEvent += onI2LLocalizeEvent;
        }

        protected void onI2LLocalizeEvent()
        {
            // Trigger language change event to force a refresh on all translations.
            var localization = Provider.GetLocalization();
            localization.TriggerLanguageChangeEvent();
        }

        protected string dynamicLocalization(string term, string language)
        {
            string translation;

            if (!I2.Loc.LocalizationManager.TryGetTranslation(term, out translation))
            {
                var localization = Provider.GetLocalization();
                // Get translation without dynamic (to avoid infinite loop).
                translation = localization.Get(term, ignoreDynamic: true);
            }

            return translation;
        }
        */


        #region Editor Stuff
#if UNITY_EDITOR
        public void Reset()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            editorAutoSelectProvider();
        }

        bool _editorOnValidated;

        public void OnValidate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(this.gameObject);
            if (prefabStatus != PrefabInstanceStatus.Connected)
                return;

            if (!_editorOnValidated)
            {
                _editorOnValidated = true;
                editorAutoSelectProvider();
            }
        }

        protected void editorAutoSelectProvider()
        {
            if (Provider == null)
            {
                var providerGuids = AssetDatabase.FindAssets("t:LocalizationProvider");
                if (providerGuids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(providerGuids[0]);
                    if (path != null)
                    {
                        var provider = AssetDatabase.LoadAssetAtPath<LocalizationProvider>(path);
                        if (provider != null)
                        {
                            Provider = provider;
                            EditorUtility.DisplayDialog($"Using localization provider \"{provider.name}\"",
                                $"The LocalizationInitializer has automatically chosen the provider: \"{provider.name}\".\n\n" +
                                $"If that is not the provider you want then please assign it in the inspector.",
                                "Ok");
                            EditorUtility.SetDirty(this);
                            EditorUtility.SetDirty(this.gameObject);
                        }
                    }
                }
            }
        }
#endif
        #endregion
    }
}
