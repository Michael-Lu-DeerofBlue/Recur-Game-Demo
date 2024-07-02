// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using Kamgam.LocalizationForSettings;
using Kamgam.LocalizationForSettings.UIElements;
using UnityEditor;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CustomEditor(typeof(UIDocumentLocalizations), editorForChildClasses: false)]
    public class UIDocumentLocalizationsEditor : Editor
    {
        public UIDocumentLocalizations localizations;

        public virtual void OnEnable()
        {
            localizations = target as UIDocumentLocalizations;

            // Auto select if settings provider is null
            string[] assetGUIDs;
            if (localizations.LocalizationProvider == null)
            {
                assetGUIDs = AssetDatabase.FindAssets("t:" + typeof(LocalizationProvider).Name);
                if (assetGUIDs.Length > 0)
                {
                    localizations.LocalizationProvider = AssetDatabase.LoadAssetAtPath<LocalizationProvider>(AssetDatabase.GUIDToAssetPath(assetGUIDs[0]));
                    markAsChangedIfEditing();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(new GUIContent("Create or Update Localizations", "Triggers the CreateOrUpdateLocalizations() method.")))
            {
                localizations.CreateOrUpdateLocalizers();
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
            EditorUtility.SetDirty(localizations);

            // Make sure the Prefab recognizes the changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(localizations);
        }
    }
}
#endif
