using System;
using System.Collections;
using UnityEngine;
// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using UnityEngine.UIElements;
#endif

namespace Kamgam.SettingsGenerator.Examples
{
    public class SettingsUIToolkitDemo : MonoBehaviour
    {
        /// <summary>
        /// The SettingsProvider ScriptableObject is always needed. It serves as the
        /// access point for anything related to the settings. Simply drag in your
        /// SettingsProvider asset.
        /// </summary>
        public SettingsProvider SettingsProvider;

        public void Awake()
        {
            // A call to the .Settings property of the provider will automatically
            // create and (synchronously) LOAD the a Settings object.
            var _ = SettingsProvider.Settings;
        }

        public void Start()
        {
            var playerNameSetting = SettingsProvider.Settings.GetOrCreateString("playerName");
            playerNameSetting.AddChangeListener(onPlayerNameChanged);

            // We'll have to wait for the menu scene to load and then within it the UIDocument has to load as well.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
            StartCoroutine(waitForUIDocumentToLoad());
#endif
        }

#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
        public IEnumerator waitForUIDocumentToLoad()
        {
            UIDocument document;
            do
            {
#if UNITY_2023_1_OR_NEWER
                document = GameObject.FindFirstObjectByType<UIDocument>(FindObjectsInactive.Include);
#else
                document = GameObject.FindObjectOfType<UIDocument>(includeInactive: true);
#endif
                yield return new WaitForSeconds(0.1f);
            }
            while (document == null || document.rootVisualElement == null);
            
            if (document != null)
            {
                var resetBtn = document.rootVisualElement.Query("SettingsResetButton");
                var applyBtn = document.rootVisualElement.Query("SettingsApplyButton");
                var saveBtn = document.rootVisualElement.Query("SettingsSaveButton");
            }
        }
#endif

        void onPlayerNameChanged(string playerName)
        {
            Debug.Log("Player name changed to: " + playerName);
        }

        public void Apply()
        {
            SettingsProvider.Apply();
        }

        public void Save()
        {
            SettingsProvider.Save();
        }

        public void Reset()
        {
            SettingsProvider.Reset();
        }
    }
}
