using Kamgam.LocalizationForSettings;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CustomEditor(typeof(SettingResolver), editorForChildClasses: true)]
    public class SettingResolverEditor : Editor
    {
        public SettingResolver resolver;
        public string[] availableIDs;
        public SettingsProvider availableIDsFromProvider;

        protected List<string> _handledSerializedProperties = new List<string>();
        protected SerializedProperty _settingProviderProp;
        protected SerializedProperty _localizationProviderProp;
        protected SerializedProperty _idProp;
        protected Vector2 scrollViewPos;

        protected List<string> _fieldsToHide = new List<string>();
        protected bool _showIdSuggestions = true;

        public virtual bool AlwaysAutoAssignProvider => false;

        public virtual void OnEnable()
        {
            _idProp = serializedObject.FindProperty("ID");
            _handledSerializedProperties.Add(_idProp.propertyPath);

            _settingProviderProp = serializedObject.FindProperty("SettingsProvider");
            _handledSerializedProperties.Add(_settingProviderProp.propertyPath);

            _localizationProviderProp = serializedObject.FindProperty("LocalizationProvider");
            _handledSerializedProperties.Add(_localizationProviderProp.propertyPath);


            resolver = target as SettingResolver;

            // Remember last resolver with provider.
            if (resolver.SettingsProvider != null)
                EditorRuntimeUtils.LastOpenedResolverWithProvider = resolver;

            // Auto select if settings provider is null
            // Only do this if the object is a prefab instance.
            if (resolver.SettingsProvider == null && (AlwaysAutoAssignProvider || PrefabUtility.IsPartOfNonAssetPrefabInstance(resolver.gameObject)))
            {
                AutoAssignSettingsProvider();
            }

            // Auto select if localization provider is null
            if (resolver.LocalizationProvider == null)
            {
                var providerGUIDs = AssetDatabase.FindAssets("t:" + typeof(LocalizationProvider).Name);
                if (providerGUIDs.Length > 0)
                {
                    resolver.LocalizationProvider = AssetDatabase.LoadAssetAtPath<LocalizationProvider>(AssetDatabase.GUIDToAssetPath(providerGUIDs[0]));
                    markAsChangedIfEditing();
                }
            }

            updateAvailableIDs();

            checkSettingActiveState(resolver);

            if (resolver.SettingsProvider != null)
            {
                SettingsProvider.LastUsedSettingsProvider = resolver.SettingsProvider;
            }
        }

        public void AutoAssignSettingsProvider()
        {
            var preferredProvider = EditorRuntimeUtils.FindPreferredSettingsProvider();
            if (preferredProvider != null)
            {
                resolver.SettingsProvider = preferredProvider;
                EditorRuntimeUtils.LastOpenedResolverWithProvider = resolver;
                markAsChangedIfEditing();

                Logger.LogMessage($"The UI '{resolver.name}' had no provider assigned and automatically chose '{resolver.SettingsProvider.name}'. Please check if that's correct.", this);
            }
        }

#if UNITY_EDITOR
        protected void checkSettingActiveState(SettingResolver resolver)
        {
            EditorApplication.delayCall += () =>
            {
                // Do nothing at runtime
                if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                if (string.IsNullOrEmpty(resolver.ID)
                    || resolver.SettingsProvider == null
                    || resolver.SettingsProvider.SettingsAsset == null
                    || !resolver.SettingsProvider.SettingsAsset.HasID(resolver.ID))
                    return;

                if (!resolver.SettingsProvider.SettingsAsset.HasActiveID(resolver.ID))
                {
                    bool activate = UnityEditor.EditorUtility.DisplayDialog(
                        $"Setting '{resolver.ID}' is DISABLED",
                        $"Would you like to ENABLE '{resolver.ID}' in your settings asset?",
                        "Yes (recommended)", "No");
                    if (activate)
                    {
                        var setting = resolver.SettingsProvider.SettingsAsset.GetSetting(resolver.ID);
                        setting.IsActive = true;
                        UnityEditor.EditorUtility.SetDirty(resolver.SettingsProvider.SettingsAsset);
                        UnityEditor.AssetDatabase.SaveAssetIfDirty(resolver.SettingsProvider.SettingsAsset);
                    }
                }
            };
        }
#endif

        private void updateAvailableIDs()
        {
            // Fetch available setting ids from settings provider
            if (resolver.SettingsProvider != null && resolver.SettingsProvider.SettingsAsset != null)
            {
                availableIDs = resolver.SettingsProvider.SettingsAsset.GetSettingIDsOrderedByName(true, resolver.GetSupportedDataTypes());
                availableIDsFromProvider = resolver.SettingsProvider;
            }
        }

        public override void OnInspectorGUI()
        {
            Color tmpColor, tmpBgColor;

            serializedObject.Update();

            string id = _idProp.stringValue;

            // Localization
            if (!_fieldsToHide.Contains("m_LocalizationProvider"))
                EditorGUILayout.PropertyField(_localizationProviderProp);

            // Provider field
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_settingProviderProp);
            GUI.enabled = resolver.SettingsProvider != null && resolver.SettingsProvider.SettingsAsset != null;
            if (GUILayout.Button(new GUIContent(" ➔ ", "Click here to jump to the settings list."), GUILayout.MaxWidth(30)))
            {
                Selection.objects = new Object[] { resolver.SettingsProvider.SettingsAsset };
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            // ID field (red if not set)
            tmpColor = GUI.color;
            tmpBgColor = GUI.backgroundColor;
            if (string.IsNullOrEmpty(resolver.ID))
            {
                GUI.backgroundColor = Color.red;
                GUI.color = new Color(1f, 0.2f, 0.2f);
            }
            GUI.SetNextControlName("idInput");
            EditorGUILayout.PropertyField(_idProp);
            GUI.color = tmpColor;
            GUI.backgroundColor = tmpBgColor;

            // Force update if provider changed.
            if (resolver != null && resolver.SettingsProvider != availableIDsFromProvider)
            {
                updateAvailableIDs();
            }

            // Warnings and ID suggestions
            if (resolver.SettingsProvider != null)
            {
                // Warnings
                if (resolver.SettingsProvider.SettingsAsset != null)
                {
                    var setting = resolver.SettingsProvider.SettingsAsset.GetSetting(resolver.ID);
                    if (setting != null)
                    {
                        if (setting.HasConnectionObject())
                        {
                            GUILayout.Label(new GUIContent("This setting uses a CONNECTION.", "The setting of this resolver is using a Connection and is filled dynamically.\nThis means that any options you add in the UI will be ignored."));
                        }
                        else
                        {
                            if (setting is SettingOption settingWithOptions && settingWithOptions.HasOptions())
                            {
                                GUILayout.Label(new GUIContent("This setting has static OPTIONS.", "The setting of this resolver has some static options defined.\nThis means that any options you add in the UI will be ignored."));
                            }
                        }
                        if (setting.IsActive)
                        {
                            GUILayout.Label(new GUIContent("This setting is ACTIVE.", "The setting of this resolver is active."));
                        }
                        else
                        {
                            var col = GUI.color;
                            GUI.color = new Color(1f, 0.2f, 0.2f);
                            GUILayout.Label(new GUIContent("This setting is NOT ACTIVE.", "The setting of this resolver is not active. It will be ignored by the settings system."));
                            if (GUILayout.Button("Activate Setting (" + resolver.ID + ")"))
                            {
                                resolver.SettingsProvider.SettingsAsset.RebuildSettingsCache();
                                resolver.SettingsProvider.SettingsAsset.SetActive(resolver.ID, true);
                                EditorUtility.SetDirty(resolver.SettingsProvider.SettingsAsset);
                                AssetDatabase.SaveAssetIfDirty(resolver.SettingsProvider.SettingsAsset);
                            }
                            GUI.color = col;
                        }
                    }
                    if (string.IsNullOrEmpty(resolver.ID))
                    {
                        tmpColor = GUI.color;
                        GUI.color = new Color(1f, 0.2f, 0.2f);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(new GUIContent("Please set an ID.", "This resolver has no ID set. It will do nothing as it will not be able to find any setting."));
                        GUILayout.EndHorizontal();
                        GUI.color = tmpColor;
                    }
                }

                // ID Suggestions
                if (availableIDs != null)
                {
                    _showIdSuggestions = EditorGUILayout.Foldout(
                        _showIdSuggestions,
                        new GUIContent("Suggested IDs:",
                        "These are IDs of settings within your 'DefaultSettings.asset'." +
                        "\n\nIt lists only IDs which match the supported data type of this resolver.")
                        );

                    if (_showIdSuggestions)
                    {
                        scrollViewPos = GUILayout.BeginScrollView(scrollViewPos, GUILayout.MaxHeight(100));
                        string idLower = resolver.ID == null ? "" : resolver.ID.Trim().ToLower();
                        string firstCandidate = null;
                        int shownIDs = 0;
                        for (int i = 0; i < availableIDs.Length; i++)
                        {
                            if (availableIDs[i].ToLower().StartsWith(idLower))
                            {
                                shownIDs++;
                                GUI.enabled = availableIDs[i] != resolver.ID;

                                if (firstCandidate == null && GUI.enabled)
                                    firstCandidate = availableIDs[i];

                                if (GUILayout.Button(availableIDs[i]))
                                {
                                    resolver.ID = availableIDs[i];
                                    _idProp.stringValue = resolver.ID;
                                    GUI.FocusControl(null);
                                    markAsChangedIfEditing();
                                }

                                GUI.enabled = true;
                            }
                        }
                        // Show containing as fallback
                        if (shownIDs == 0)
                        {
                            for (int i = 0; i < availableIDs.Length; i++)
                            {
                                if (availableIDs[i].ToLower().Contains(idLower))
                                {
                                    shownIDs++;
                                    GUI.enabled = availableIDs[i] != resolver.ID;

                                    if (firstCandidate == null && GUI.enabled)
                                        firstCandidate = availableIDs[i];

                                    if (GUILayout.Button(availableIDs[i]))
                                    {
                                        resolver.ID = availableIDs[i];
                                        _idProp.stringValue = resolver.ID;
                                        GUI.FocusControl(null);
                                        markAsChangedIfEditing();
                                    }

                                    GUI.enabled = true;
                                }
                            }
                        }
                        GUILayout.EndScrollView();

                        // apply first candidate if the down arrow is pressed
                        if (GUI.GetNameOfFocusedControl() == "idInput" && Event.current.keyCode == KeyCode.DownArrow && firstCandidate != null && resolver.ID != firstCandidate)
                        {
                            resolver.ID = firstCandidate;
                            _idProp.stringValue = resolver.ID;
                            GUI.FocusControl(null);
                            markAsChangedIfEditing();
                        }

                        if (shownIDs == 0)
                        {
                            tmpColor = GUI.color;
                            GUI.color = new Color(1f, 1f, 0.2f);
                            GUILayout.Label(new GUIContent("No machting IDs found! Is it a dynamic setting?", "Resolving this settings will fail unless a setting with this id is added dynamically via code."));
                            GUI.color = tmpColor;
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (id != _idProp.stringValue)
            {
                checkSettingActiveState(resolver);
            }


            EditorGUILayout.Space(7);

            // draw the rest of the properties (if there are any)
            serializedObject.Update();
            var prop = serializedObject.FindProperty(_handledSerializedProperties[_handledSerializedProperties.Count - 1]);
            while (prop.NextVisible(enterChildren: !_fieldsToHide.Contains(prop.name)))
            {
                // Skip script field?
                if (_handledSerializedProperties.Contains(prop.propertyPath) || prop.name == "Script")
                    continue;

                if (!_fieldsToHide.Contains(prop.name))
                    EditorGUILayout.PropertyField(prop);
            }
            serializedObject.ApplyModifiedProperties();
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
            if (resolver.gameObject.scene != null)
                EditorSceneManager.MarkSceneDirty(resolver.gameObject.scene);

            // Make sure the Prefab recognizes the changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(resolver);
        }
    }
}
