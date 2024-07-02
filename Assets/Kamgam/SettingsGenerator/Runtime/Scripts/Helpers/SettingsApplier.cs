using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Applies the settings in Star().<br />
    /// Add this to any scene that is not loaded additively.
    /// </summary>
    public class SettingsApplier : MonoBehaviour
    {
        /// <summary>
        /// Don't forget to hook this up with the right provider in the inspector.
        /// </summary>
        public SettingsProvider Provider;

        [Header("Start")]
        public bool ApplyOnStart = true;

        [Tooltip("On start delay in seconds.")]
        public float ApplyOnStartDelay = 0f;

        [Header("Update")]
        [Tooltip("Only use this as a last resort if another system keeps overriding your settings.\n" +
            "You really should find out what system that is and route the settings through that instead of using this.")]
        public bool ApplyOnLateUpdate = false;

        [Header("Limit applied settings")]
        [Tooltip("Leave empty to apply all settings")]
        public List<string> SettingIds = new List<string>();

        public IEnumerator Start()
        {
            yield return new WaitForSecondsRealtime(ApplyOnStartDelay);

            if (Provider == null)
            {
                Debug.LogError("You have not set the Provider on you SettingsApplier. Please set a provider!", this);
                throw new System.Exception("Missing Provider on Settings Initializer.");
            }

            if (ApplyOnStart)
                Apply();
        }

        public void LateUpdate()
        {
            if (ApplyOnLateUpdate)
                Apply();
        }

        public void Apply()
        {
            if (SettingIds == null || SettingIds.Count == 0)
            {
                // Apply the settings to all connections.
                Provider.Settings.Apply(changedOnly: false);
            }
            else
            {
                // Apply only those in the settings ids.
                foreach (var id in SettingIds)
                {
                    var setting = Provider.Settings.GetSetting(id);
                    setting?.Apply();
                }
            }
        }


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
                var providerGuids = AssetDatabase.FindAssets("t:SettingsProvider");
                if (providerGuids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(providerGuids[0]);
                    if (path != null)
                    {
                        var provider = AssetDatabase.LoadAssetAtPath<SettingsProvider>(path);
                        if (provider != null)
                        {
                            Provider = provider;
                            EditorUtility.DisplayDialog($"Using provider \"{provider.name}\"",
                                $"The SettingsApplier has automatically chosen the provider: \"{provider.name}\".\n\n" +
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
