using UnityEditor;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : Editor
    {
        public Settings settings;

        public void OnEnable()
        {
            settings = target as Settings;
        }

        override public void OnInspectorGUI()
        {
            // Go back to UI
            GUI.enabled = EditorRuntimeUtils.LastOpenedResolverWithProvider != null;
            if (GUILayout.Button(new GUIContent("Back to UI", "Selected the last used settings resolver ui.")))
            {
                Selection.objects = new Object[] { EditorRuntimeUtils.LastOpenedResolverWithProvider.gameObject };
            }
            GUI.enabled = true;

            if (GUILayout.Button(new GUIContent("Back to Provider", "Opens the last used provider.")))
            {
                var guids = AssetDatabase.FindAssets("t:SettingsProvider");
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var provider = AssetDatabase.LoadAssetAtPath<SettingsProvider>(path);
                    if (provider.SettingsAsset == settings)
                    {
                        Selection.objects = new Object[] { provider };
                        break;
                    }
                }
            }

            serializedObject.Update();

            base.OnInspectorGUI();

            if (serializedObject.hasModifiedProperties)
            {
                EditorUtility.SetDirty(settings);
            }
            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("Disable All", "Disables all settings.")))
            {
                settings.RebuildSettingsCache();
                foreach (var setting in settings.GetAllSettings())
                {
                    setting.IsActive = false;
                }
                EditorUtility.SetDirty(settings);
            }
            if (GUILayout.Button(new GUIContent("Enable All", "Disables all settings.")))
            {
                settings.RebuildSettingsCache();
                foreach (var setting in settings.GetAllSettings())
                {
                    setting.IsActive = true;
                }
                EditorUtility.SetDirty(settings);
            }
        }
    }
}
