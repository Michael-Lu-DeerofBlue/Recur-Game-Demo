# if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Kamgam.SettingsGenerator
{
    public static class EditorRuntimeUtils
    {
        public static SettingsProvider LastOpenedProvider;
        public static SettingResolver LastOpenedResolverWithProvider;

        public static SettingsProvider FindPreferredSettingsProvider()
        {
            // Auto select if settings provider is null
            string[] providerGUIDs;

            // First try using the last used provider
            if (LastOpenedProvider != null)
            {
                return LastOpenedProvider;
            }
            // Second try the provider from the last used UI.
            else if (LastOpenedResolverWithProvider != null && LastOpenedResolverWithProvider.SettingsProvider != null)
            {
                return LastOpenedResolverWithProvider.SettingsProvider;
            }
            // Third: search in assets and used the first found
            // Right now the shallower the path of a settings file is the soon it will be returned.
            // Which is okay too since user settings are usually in shallower paths than the default
            // examples or templates.
            // TODO/N2H: sort by modification time and the select the newest.
            else
            {
                providerGUIDs = AssetDatabase.FindAssets("t:" + typeof(SettingsProvider).Name);
                if (providerGUIDs.Length > 0)
                {
                    return AssetDatabase.LoadAssetAtPath<SettingsProvider>(AssetDatabase.GUIDToAssetPath(providerGUIDs[0]));
                }
            }

            return null;
        }
    }
}
#endif
