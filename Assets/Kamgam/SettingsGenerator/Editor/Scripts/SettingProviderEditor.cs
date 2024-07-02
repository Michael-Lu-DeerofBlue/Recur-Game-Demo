using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CustomEditor(typeof(SettingsProvider))]
    public class SettingProviderEditor : Editor
    {
        [System.Obsolete("Obsolete: Use EditorRuntimeUtils.LastOpenedProvider instead.")]
        public static SettingsProvider LastOpenedProvider;

        public SettingsProvider provider;

        protected string settingFileName;

        public void OnEnable()
        {
            provider = target as SettingsProvider;

#pragma warning disable CS0618 // Type or member is obsolete
            LastOpenedProvider = provider; // here only for backwards compatibility.
#pragma warning restore CS0618 // Type or member is obsolete
            EditorRuntimeUtils.LastOpenedProvider = provider;
        }

        public override void OnInspectorGUI()
        {
            // Go back to UI
            GUI.enabled = EditorRuntimeUtils.LastOpenedResolverWithProvider != null;
            if (GUILayout.Button(new GUIContent("Back to UI", "Selected the last used settings resolver ui.")))
            {
                Selection.objects = new Object[] { EditorRuntimeUtils.LastOpenedResolverWithProvider.gameObject };
            }
            GUI.enabled = true;

            // Go to Settings
            GUI.enabled = provider.SettingsAsset != null;
            if (GUILayout.Button(new GUIContent("Open Settings", "Opens the settings object.")))
            {
                Selection.objects = new Object[] { provider.SettingsAsset };
            }
            GUI.enabled = true;

            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Delete", "NOTICE: If you are using a custom delete method then this will only work during PLAY MODE.")))
            {
                provider.Delete();
            }
            GUI.enabled = EditorApplication.isPlaying;
            if (GUILayout.Button("Save"))
            {
                provider.Save();
            }
            if (GUILayout.Button("Load"))
            {
                provider.Load();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            // Settings Creation
            GUILayout.Space(10);
            GUI.enabled = provider.SettingsAsset == null;

            GUILayout.Label("Settings Creation:");

            if (settingFileName == null)
                settingFileName = "Settings (" + Application.productName + ")";
            settingFileName = EditorGUILayout.TextField("Setting File Nane:", settingFileName);

            if (GUILayout.Button(new GUIContent("Create Settings", "Creates a Settings object at the same location as this provider and assigns it to it.")))
            {
                var path = AssetDatabase.GetAssetPath(provider);
                path = Path.GetDirectoryName(path).Replace("\\", "/") + "/" + settingFileName + ".asset";

                Settings settings = Settings.CreateInstance<Settings>();
                AssetDatabase.CreateAsset(settings, path);

                Debug.Log("Settings created at " + path);

                settings.GetOrCreateFloat("dummy", 1f);

                provider.SettingsAsset = settings;
                EditorUtility.SetDirty(provider);
            }
            GUI.enabled = true;
        }
    }
}
