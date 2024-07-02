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
        protected ListView _genericSettingsListView;
        [SerializeField]
        protected List<SettingTypeEntry> _genericSettingsEntries = new List<SettingTypeEntry>();
        protected int _genericSettingsLastSelectedIndex = -1;

        protected ListView _prefabsListView;
        protected List<PrefabEntry> _prefabsEntries = new List<PrefabEntry>();
        protected int _prefabsLastSelectedIndex = -1;

        private void onShowChooseSetting(ClickEvent evt)
        {
            _prefabsEntries = PrefabEntry.FindAllSettingPrefabs();
            _prefabsListView.itemsSource = _prefabsEntries;
            _prefabsListView.Rebuild();

            _chooseSettingContainer.style.display = DisplayStyle.Flex;
            _genericSettingsListView.ScrollToSelectedItem();

            _chooseProviderContainer.style.display = DisplayStyle.None;
            _chooseVisualContainer.style.display = DisplayStyle.None;
        }

        private void createChooseSettingGUI(VisualElement root)
        {
            _chooseSettingContainer = root.AddContainer("ChooseSetting", "grow", "content-container");
            var container = _chooseSettingContainer;

            container.AddLabel("Setting", "h1");
            var providerObjField = container.AddObjectField<SettingsProvider>("Provider:", _selectedSettingsProvider, onChangeSettingsProvider, allowSceneObjects: false, "mb-10", "dont-shrink");
            providerObjField.bindingPath = "_selectedSettingsProvider";
            providerObjField.Bind(_serializedObject);

            var tabContainer = new List<VisualElement>();
            var tabbar = container.AddTabbar(new List<string>() { "Prefabs", "Generics" }, tabContainer);
            tabbar.AddToClassList("dont-shrink");

            var prefabsContainer = container.AddContainer("PrefabsContainer", "grow");
            var genericsContainer = container.AddContainer("GenericsContainer", "grow");
            genericsContainer.style.display = DisplayStyle.None;
            tabContainer.Add(prefabsContainer);
            tabContainer.Add(genericsContainer);

            createPrefabsSettingsContent(prefabsContainer);
            createGenericSettingsContent(genericsContainer);
        }

        private void createPrefabsSettingsContent(VisualElement genericContainer)
        {
            var genericSearchbar = genericContainer.AddContainer("Header", "horizontal");
            genericSearchbar.AddTextField(onFilterPrefabs, "grow");

            Button createBtn = null;

            _prefabsListView = new ListView();
            _prefabsListView.showBoundCollectionSize = false;
            _prefabsListView.itemsSource = _prefabsEntries;
            _prefabsListView.makeItem = () => new Label();
            _prefabsListView.bindItem = (e, i) => bindPrefabItem(e as Label, i, _prefabsListView);
            _prefabsListView.reorderable = false;
            _prefabsListView.AddToClassList("grow");
#if UNITY_2022_1_OR_NEWER
            _prefabsListView.selectionChanged += (e) =>
#else
            _prefabsListView.onSelectionChange += (e) =>
#endif
            {
                onPrefabSelectionChange(_prefabsListView.selectedIndex, _prefabsEntries);
                createBtn.SetEnabled(_prefabsListView.selectedIndex >= 0);
            };
#if UNITY_2022_1_OR_NEWER
            _prefabsListView.itemsChosen += (e) =>
#else
            _prefabsListView.onItemsChosen += (e) =>
#endif
            {
                _prefabsLastSelectedIndex = _prefabsListView.selectedIndex;
                onCreateFromPrefab(null);
                createBtn.SetEnabled(_prefabsListView.selectedIndex >= 0);
            };
            genericContainer.Add(_prefabsListView);

            // Bottom menu
            var bottomMenu = genericContainer.AddContainer("BottomMenu", "horizontal", "dont-shrink");
            // Back
            bottomMenu.AddButton("Back", onShowChooseProvider, "grow");
            // Create
            createBtn = bottomMenu.AddButton("Create", onCreateFromPrefab, "grow");
            createBtn.SetEnabled(_prefabsListView.selectedIndex >= 0);
            bottomMenu.Add(createBtn);
        }

        private void bindPrefabItem(Label field, int index, ListView listView)
        {
            field.text = ((PrefabEntry)listView.itemsSource[index]).GetName();
        }

        private void onFilterPrefabs(ChangeEvent<string> evt)
        {
            var list = _prefabsEntries.FindAll(entry =>
            {
                string entryName = entry.GetName().ToLower().Replace(" ", "");
                string filterPart = evt.newValue.ToLower().Replace(" ", "");
                return entryName.Contains(filterPart);
            });
            _prefabsListView.itemsSource = list;
            _prefabsListView.Rebuild();
        }

        private void onPrefabSelectionChange(int index, List<PrefabEntry> settingTypes)
        {
            if (index < 0)
                return;

            _prefabsLastSelectedIndex = index;
        }


        private void createGenericSettingsContent(VisualElement genericContainer)
        {
            var genericSearchbar = genericContainer.AddContainer("Header", "horizontal");
            genericSearchbar.AddTextField(onFilterGenericSettingTypes, "grow");

            _genericSettingsEntries.Clear();
            _genericSettingsEntries.Add(new SettingTypeEntry(null, "Generic: Float", SettingData.DataType.Float));
            _genericSettingsEntries.Add(new SettingTypeEntry(null, "Generic: Int", SettingData.DataType.Int));
            _genericSettingsEntries.Add(new SettingTypeEntry(null, "Generic: String", SettingData.DataType.String));
            _genericSettingsEntries.Add(new SettingTypeEntry(null, "Generic: Option", SettingData.DataType.Option));
            _genericSettingsEntries.Add(new SettingTypeEntry(null, "Generic: Color", SettingData.DataType.Color));
            _genericSettingsEntries.Add(new SettingTypeEntry(null, "Generic: ColorOption", SettingData.DataType.ColorOption));
            _genericSettingsEntries.Add(new SettingTypeEntry(null, "Generic: KeyCombination", SettingData.DataType.KeyCombination));

            var connectionGUIDs = AssetDatabase.FindAssets("t:ConnectionSO");
            // First all but the bindings
            foreach (var guid in connectionGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.Contains("InputBinding"))
                    _genericSettingsEntries.Add(new SettingTypeEntry(path, null, SettingData.DataType.Unknown));
            }
            // Input bindings last (they may be many).
            foreach (var guid in connectionGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("InputBinding"))
                    _genericSettingsEntries.Add(new SettingTypeEntry(path, null, SettingData.DataType.Unknown));
            }

            Button nextBtn = null;

            _genericSettingsListView = new ListView();
            _genericSettingsListView.itemsSource = _genericSettingsEntries;
            _genericSettingsListView.makeItem = () => new Label();
            _genericSettingsListView.bindItem = (e, i) => bindSettingTypeItem(e as Label, i, _genericSettingsListView);
            _genericSettingsListView.reorderable = false;
            _genericSettingsListView.AddToClassList("grow");
#if UNITY_2022_1_OR_NEWER
            _genericSettingsListView.selectionChanged += (e) =>
#else
            _genericSettingsListView.onSelectionChange += (e) =>
#endif
            {
                onSettingTypeSelectionChange(_genericSettingsListView.selectedIndex, _genericSettingsEntries);
                nextBtn.SetEnabled(_genericSettingsListView.selectedIndex >= 0);
            };
#if UNITY_2022_1_OR_NEWER
            _genericSettingsListView.itemsChosen += (e) =>
#else
            _genericSettingsListView.onItemsChosen += (e) =>
#endif
            {
                _genericSettingsLastSelectedIndex = _genericSettingsListView.selectedIndex;
                onShowChooseSettingsProvider(null);
                nextBtn.SetEnabled(_genericSettingsListView.selectedIndex >= 0);
            };
            genericContainer.Add(_genericSettingsListView);

            // Bottom menu
            var bottomMenu = genericContainer.AddContainer("BottomMenu", "horizontal", "dont-shrink");
            // Back
            bottomMenu.AddButton("Back", onShowChooseProvider, "grow");
            // Next
            nextBtn = bottomMenu.AddButton("Next", onShowChooseSettingsProvider, "grow");
            nextBtn.SetEnabled(_genericSettingsListView.selectedIndex >= 0);
            bottomMenu.Add(nextBtn);
        }

        private void onFilterGenericSettingTypes(ChangeEvent<string> evt)
        {
            var list = _genericSettingsEntries.FindAll(entry =>
            {
                string entryName = entry.GetName().ToLower().Replace(" ", "");
                string filterPart = evt.newValue.ToLower().Replace(" ", "");
                return entryName.Contains(filterPart);
            });
            _genericSettingsListView.itemsSource = list;
            _genericSettingsListView.Rebuild();
        }

        private void bindSettingTypeItem(Label field, int index, ListView listView)
        {
            field.text = (listView.itemsSource[index] as SettingTypeEntry).GetName();
        }

        private void onSettingTypeSelectionChange(int index, List<SettingTypeEntry> settingTypes)
        {
            if (index < 0)
                return;

            _genericSettingsLastSelectedIndex = index;
        }

        private void onShowChooseSettingsProvider(ClickEvent evt)
        {
            var entry = _genericSettingsListView.itemsSource[_genericSettingsLastSelectedIndex] as SettingTypeEntry;
            onShowChooseVisual(null);
        }
    }
}