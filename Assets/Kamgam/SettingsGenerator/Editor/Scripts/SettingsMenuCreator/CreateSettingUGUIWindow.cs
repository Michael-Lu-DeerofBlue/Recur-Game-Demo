using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Kamgam.SettingsGenerator;
using System;
using TMPro;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

using SettingsProvider = Kamgam.SettingsGenerator.SettingsProvider;

namespace Kamgam.SettingsGenerator
{
    public partial class CreateSettingUGUIWindow : EditorWindow
    {
        public StyleSheet m_StyleSheet;

        const string StyleSheedName = "SettingsMenuCreatorWindow";
        public StyleSheet GetStyleSheet()
        {
            if (m_StyleSheet == null)
            {
                var guids = AssetDatabase.FindAssets("t:StyleSheet " + StyleSheedName);
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    m_StyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                }
            }

            return m_StyleSheet;
        }

        [System.NonSerialized]
        public static CreateSettingUGUIWindow LastOpenedWindow;

        [MenuItem("Tools/Settings Generator/Create Setting UGUI", priority = 0)]
        [MenuItem("GameObject/UI/Settings Generator/Create Setting UGUI", priority = 2001)]
        public static void ShowMenuCreator()
        {
            //if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<RectTransform>() == null)
            //{
            //    EditorUtility.DisplayDialog(
            //        "Select UI",
            //        "Please select the UI container (GameObject with a RectTransform) you want to create the setting in before calling this action.",
            //        "Ok");
            //    return;
            //}

            LastOpenedWindow = GetWindow<CreateSettingUGUIWindow>();
            //var icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor/Images/gear.png");
            LastOpenedWindow.titleContent = new GUIContent("Setting Creator"); //, icon);
            LastOpenedWindow.Show();
            LastOpenedWindow.minSize = new Vector2(200, 200);
            var pos = LastOpenedWindow.position;
            pos.width = 600;
            pos.height = 700;
            CenterOnMainWin();
            LastOpenedWindow.init();
        }

        public static void CenterOnMainWin()
        {
            Rect main = EditorGUIUtility.GetMainWindowPosition();
            Rect pos = LastOpenedWindow.position;
            float centerWidth = (main.width - pos.width) * 0.5f;
            float centerHeight = (main.height - pos.height) * 0.5f;
            pos.x = main.x + centerWidth;
            pos.y = main.y + centerHeight;
            LastOpenedWindow.position = pos;
        }

        protected VisualElement _chooseProviderContainer;
        protected VisualElement _chooseSettingContainer;
        protected VisualElement _chooseVisualContainer;

        private SerializedObject _serializedObject;

        public void CreateGUI()
        {
            if (LastOpenedWindow == null)
            {
                init();
                return;
            }

            var root = rootVisualElement;
            root.styleSheets.Add(GetStyleSheet());
            root.Clear();

            createChooseProviderGUI(root);
            createChooseSettingGUI(root);
            createChooseVisualGUI(root);
        }

        private void init()
        {
            _serializedObject = new SerializedObject(this);
            _serializedObject.Update();

            if (_visualsEntriesList == null || _visualsEntriesList.Count == 0)
            {
                _visualsEntriesList = new List<SettingVisualEntry>();
                var prefabGUIDs = AssetDatabase.FindAssets("t:Prefab (Setting)");

                // First console style UIs
                foreach (var guid in prefabGUIDs)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var name = System.IO.Path.GetFileNameWithoutExtension(path);
                    if (name.ToLower().Contains("console"))
                    {
                        addVisualEntry(path, name);
                    }
                }

                // Second, PC style UIs
                foreach (var guid in prefabGUIDs)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var name = System.IO.Path.GetFileNameWithoutExtension(path);
                    if (!name.ToLower().Contains("console"))
                    {
                        addVisualEntry(path, name);
                    }
                }
            }

            LastOpenedWindow = this;
            CreateGUI();

            // Make sure rootVisualElement has the focus when we start.
            rootVisualElement.focusable = true;
            rootVisualElement.pickingMode = PickingMode.Position;
            rootVisualElement.Focus();

            Selection.selectionChanged -= onSelectionChanged;
            Selection.selectionChanged += onSelectionChanged;

            // Check if the user has already created a SettingsProvider.
            var settings = SettingsGeneratorSettings.GetOrCreateSettings();
            if (!SettingsProvider.DoesCustomProviderExist())
            {
                _isFirstTimeUser = true;

                // Create settings provider.
                _selectedSettingsProvider = SettingsProvider.EditorCreateProviderBasedOnTemplate(dirName: "SettingsGenerator", pingAfterCreation: true);
                _selectedSettingsProvider.SettingsAsset.SetAllActive(false);
                SettingsProvider.LastUsedSettingsProvider = _selectedSettingsProvider;

                onShowChooseProvider(null);
            }
            else
            {
                _isFirstTimeUser = false;

                // Try to automatically find a settings provider
                if (_selectedSettingsProvider == null && Selection.activeGameObject != null)
                {
                    // Find reference to settings provider in scene, if one is found then use it.
                    SettingsProvider provider = null;
                    var scene = Selection.activeGameObject.scene;
                    if (scene.IsValid() && scene.isLoaded)
                    {
                        var roots = scene.GetRootGameObjects();
                        foreach (var r in roots)
                        {
                            var resolver = r.GetComponentInChildren<SettingResolver>();
                            if (resolver != null && resolver.SettingsProvider != null)
                            {
                                provider = resolver.SettingsProvider;
                                break;
                            }
                        }
                    }

                    // Try last used provider
                    if (provider == null)
                    {
                        provider = SettingsProvider.LastUsedSettingsProvider;
                    }

                    // Try custom provider asset
                    if (provider == null)
                    {
                        provider = SettingsProvider.GetFirstCustomProvider();
                    }

                    _selectedSettingsProvider = provider;
                }

                if (_selectedSettingsProvider != null)
                {
                    var index = _providers.IndexOf(_selectedSettingsProvider);
                    if (index >= 0)
                        _providersListView.SetSelectionWithoutNotify(new int[] { });
                }

                if (_selectedSettingsProvider != null && settings.SkipProviderSelectionInMenuCreator)
                {
                    onShowChooseSetting(null);
                }
                else
                {
                    onShowChooseProvider(null);
                }
            }
        }

        private void onSelectionChanged()
        {
            if (_targetObjectField != null)
                _targetObjectField.value = Selection.activeGameObject;
        }

        private void addVisualEntry(string path, string name)
        {
            // Extract supported types from prefab.
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var resolver = prefab.GetComponentInChildren<SettingResolver>(includeInactive: true);
            SettingData.DataType[] supportedTypes;
            if (resolver != null)
            {
                var types = resolver.GetSupportedDataTypes();
                supportedTypes = new SettingData.DataType[types.Length];
                Array.Copy(types, supportedTypes, types.Length);

                var entry = new SettingVisualEntry(path, name, supportedTypes);
                _visualsEntriesList.Add(entry);
            }
        }

        public void OnDestroy()
        {
            LastOpenedWindow = null;
        }

        private void onCreateFromPrefab(ClickEvent evt)
        {
            var provider = _selectedSettingsProvider;
            var parent = (_targetObjectField.value as GameObject);
            if (parent == null || EditorUtility.IsPersistent(parent) || parent.transform as RectTransform == null)
            {
                EditorUtility.DisplayDialog(
                    "No UI container selected!",
                    "Please select the ui container you wish to add the setting to.",
                    "Ok"
                    );
                return;
            }

            var entry = _prefabsListView.GetFirstSelectedValue<PrefabEntry>();
            var prefab = entry.GetPrefab();
            if (prefab != null)
            {
                var instance = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
                var resolver = instance.GetComponentInChildren<SettingResolver>();
                Undo.RegisterCreatedObjectUndo(instance, "Added new setting UI");
                if (resolver != null && provider.SettingsAsset != null && parent != null && parent.scene != null && !EditorUtility.IsPersistent(parent))
                {
                    resolver.SettingsProvider = _selectedSettingsProvider;

                    var setting = _selectedSettingsProvider.SettingsAsset.GetSetting(resolver.ID);
                    if (setting != null)
                    {
                        setting.IsActive = true;
                    }
                }

                EditorUtility.SetDirty(provider);
                AssetDatabase.SaveAssetIfDirty(provider);

                EditorUtility.SetDirty(provider.SettingsAsset);
                AssetDatabase.SaveAssetIfDirty(provider.SettingsAsset);

                EditorUtility.SetDirty(parent);

                EditorGUIUtility.PingObject(instance);
            }
        }

        private void onCreateSetting(ClickEvent evt)
        {
            var settingType = _genericSettingsListView.GetFirstSelectedValue<SettingTypeEntry>();
            var visual = _visualsListView.GetFirstSelectedValue<SettingVisualEntry>();
            var provider = _selectedSettingsProvider;
            var parent = (_targetObjectField.value as GameObject);
            if (parent == null || EditorUtility.IsPersistent(parent) || parent.transform as RectTransform == null)
            {
                EditorUtility.DisplayDialog(
                    "No UI container selected!",
                    "Please select the ui container you wish to add the setting to.",
                    "Ok"
                    );
                return;
            }

            var prefab = visual.GetPrefab();
            var instance = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
            var resolver = instance.GetComponentInChildren<SettingResolver>();
            Undo.RegisterCreatedObjectUndo(instance, "Added new setting UI.");
            if (resolver != null && provider.SettingsAsset != null && parent != null && parent.scene != null && !EditorUtility.IsPersistent(parent))
            {
                Undo.RecordObject(provider.SettingsAsset, "New setting added to provider.");

                // Link to provider
                resolver.SettingsProvider = provider;
                EditorUtility.SetDirty(resolver);

                ISetting setting = null;
                if (settingType.IsGeneric)
                {
                    // Create in settings list
                    string id = settingType.GetSupportedDataType().ToString() + UnityEngine.Random.Range(999, 900900);
                    setting = provider.SettingsAsset.GetOrCreate(id, settingType.GetSupportedDataType());
                    setting.IsActive = true;

                    // Link to setting
                    resolver.ID = id;
                }
                else
                {
                    // Check if a setting that uses this connection exists in the settings.
                    // If not, then create one and link the connection SO.
                    // If yes, then activate and use that settings.
                    var connectionSO = settingType.GetConnectionSO();
                    setting = provider.SettingsAsset.GetFirstSettingWithConnectionSO(connectionSO);
                    if (setting == null)
                    {
                        string id = settingType.GetName().Replace(" ", "");
                        id = char.ToLower(id[0]) + id.Substring(1); // First char lower
                        setting = provider.SettingsAsset.GetOrCreate(id, settingType.GetSupportedDataType());
                        setting.SetConnectionSO(settingType.GetConnectionSO());
                    }

                    if (setting != null)
                    {
                        setting.IsActive = true;
                        resolver.ID = setting.GetID();
                    }
                }

                if (setting != null)
                {
                    instance.name = instance.name.Replace("(Setting)", "(" + setting.GetID() + ")");
                }
            }

            EditorUtility.SetDirty(provider);
            AssetDatabase.SaveAssetIfDirty(provider);

            EditorUtility.SetDirty(provider.SettingsAsset);
            AssetDatabase.SaveAssetIfDirty(provider.SettingsAsset);

            EditorUtility.SetDirty(parent);

            EditorGUIUtility.PingObject(instance);
        }
    }
}