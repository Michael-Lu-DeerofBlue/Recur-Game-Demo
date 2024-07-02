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
        protected ListView _visualsListView;
        protected List<SettingVisualEntry> _visualsEntriesList = new List<SettingVisualEntry>();
        protected int _visualsLastSelectedIndex = -1;
        protected ObjectField _targetObjectField;

        private void onShowChooseVisual(ClickEvent evt)
        {
            _chooseProviderContainer.style.display = DisplayStyle.None;
            _chooseSettingContainer.style.display = DisplayStyle.None;

            _chooseVisualContainer.style.display = DisplayStyle.Flex;
            _visualsListView.ScrollToSelectedItem();
            onFilterVisuals(null);
        }

        private void createChooseVisualGUI(VisualElement root)
        {
            _chooseVisualContainer = root.AddContainer("ChooseVisual", "grow", "content-container");
            var container = _chooseVisualContainer;

            container.AddLabel("Visuals", "h1");
            var providerObjField = container.AddObjectField<SettingsProvider>("Provider:", _selectedSettingsProvider, onChangeSettingsProvider, allowSceneObjects: false, "dont-shrink");
            providerObjField.bindingPath = "_selectedSettingsProvider";
            providerObjField.Bind(_serializedObject);
            _targetObjectField = container.AddObjectField<GameObject>("UI Container:", Selection.activeGameObject, null, allowSceneObjects: true, "mb-10", "dont-shrink");
            _targetObjectField.SetEnabled(false);

            var searchbar = container.AddContainer("Header", "horizontal");
            searchbar.AddTextField(onFilterVisuals, "grow");

            Button nextBtn = null;
            _visualsListView = new ListView();
            _visualsListView.itemsSource = _visualsEntriesList;
            _visualsListView.makeItem = () => new Label();
            _visualsListView.bindItem = (e, i) => bindVisualsItem(e as Label, i, _visualsListView);
            _visualsListView.reorderable = false;
            _visualsListView.AddToClassList("grow");
#if UNITY_2022_1_OR_NEWER
            _visualsListView.selectionChanged += (e) =>
#else
            _visualsListView.onSelectionChange += (e) =>
#endif
            {
                onVisualsSelectionChange(_visualsListView.selectedIndex);
                nextBtn.SetEnabled(_visualsListView.selectedIndex >= 0);
            };
#if UNITY_2022_1_OR_NEWER
            _visualsListView.itemsChosen += (e) =>
#else
            _visualsListView.onItemsChosen += (e) =>
#endif
            {
                _visualsLastSelectedIndex = _visualsListView.selectedIndex;
                onCreateSetting(null);
                nextBtn.SetEnabled(_visualsListView.selectedIndex >= 0);
            };
            container.Add(_visualsListView);

            // Bottom menu
            var bottomMenu = container.AddContainer("BottomMenu", "horizontal", "dont-shrink");
            // Back
            bottomMenu.AddButton("Back", onShowChooseSetting, "grow");
            // Next
            nextBtn = bottomMenu.AddButton("Create", onCreateSetting, "grow");
            nextBtn.SetEnabled(_genericSettingsListView.selectedIndex >= 0);
            bottomMenu.Add(nextBtn);
        }

        private void onChangeSettingsProvider(ChangeEvent<SettingsProvider> evt)
        {
            _selectedSettingsProvider = evt.newValue;
        }

        private void bindVisualsItem(Label field, int index, ListView listView)
        {
            field.text = (listView.itemsSource[index] as SettingVisualEntry).GetName();
        }

        private void onVisualsSelectionChange(int index)
        {
            if (index < 0)
                return;

            _visualsLastSelectedIndex = index;
        }


        private void onFilterVisuals(ChangeEvent<string> evt)
        {
            var settingType = _genericSettingsListView.GetFirstSelectedValue<SettingTypeEntry>();

            var list = new List<SettingVisualEntry>(_visualsEntriesList);

            // Filter by supported data types
            if (settingType != null)
            {
                list = list.FindAll(entry =>
                {
                    var result = settingType.SupportsAny(entry.SupportedTypes);
                    return result;
                });
            }

            // Filter by search term
            if (evt != null)
            {
                string searchTerm = evt.newValue;
                if (string.IsNullOrEmpty(searchTerm.Trim()))
                {
                    list = list.FindAll(entry =>
                    {
                        string entryName = entry.GetName().ToLower().Replace(" ", "");
                        string filterPart = searchTerm.ToLower().Replace(" ", "");
                        return entryName.Contains(filterPart);
                    });
                }
            }

            _visualsListView.itemsSource = list;
            _visualsListView.Rebuild();
        }
    }
}