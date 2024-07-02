// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using Kamgam.LocalizationForSettings;
using UnityEditor;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CustomEditor(typeof(UIDocumentSettingsResolver))]
    public class UIDocumentSettingsResolverEditor : Editor
    {
        public UIDocumentSettingsResolver resolver;

        public virtual void OnEnable()
        {
            resolver = target as UIDocumentSettingsResolver;

            // Auto select if settings provider is null
            if (resolver.SettingsProvider == null)
            {
                string[] providerGUIDs = AssetDatabase.FindAssets("t:" + typeof(SettingsProvider).Name);
                if (providerGUIDs.Length > 0)
                {
                    resolver.SettingsProvider = AssetDatabase.LoadAssetAtPath<SettingsProvider>(AssetDatabase.GUIDToAssetPath(providerGUIDs[0]));
                    markAsChangedIfEditing();
                }
            }

            // Auto select if provider is null
            if (resolver.LocalizationProvider == null)
            {
                string[] localizationsGUIDs = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(LocalizationProvider).Name);
                if (localizationsGUIDs.Length > 0)
                {
                    resolver.LocalizationProvider = UnityEditor.AssetDatabase.LoadAssetAtPath<LocalizationProvider>(UnityEditor.AssetDatabase.GUIDToAssetPath(localizationsGUIDs[0]));
                    UnityEditor.EditorUtility.SetDirty(this);
                    markAsChangedIfEditing();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(new GUIContent("Create or Update Resolvers", "Triggers the CreateOrUpdateResolvers() method.")))
            {
                resolver.CreateOrUpdateResolvers();
            }
        }

        protected void markAsChangedIfEditing()
        {
            if (EditorApplication.isPlaying)
                return;

            // Schedule an update to the scene view will rerender (otherwise
            // the change would not be visible unless clicked into the scene view).
            EditorApplication.QueuePlayerLoopUpdate();

            // Make sure the scene can be saved
            EditorUtility.SetDirty(resolver);

            // Make sure the Prefab recognizes the changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(resolver);
        }
    }
}
#endif
