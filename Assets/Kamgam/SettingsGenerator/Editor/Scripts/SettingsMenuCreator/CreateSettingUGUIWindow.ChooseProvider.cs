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
using UnityEngine.SceneManagement;

namespace Kamgam.SettingsGenerator
{
    public partial class CreateSettingUGUIWindow : EditorWindow
    {
        [SerializeField]
        protected bool _isFirstTimeUser;

        [SerializeField]
        protected List<SettingsProvider> _providers = new List<SettingsProvider>();

        [SerializeField]
        protected SettingsProvider _selectedSettingsProvider;

        protected ListView _providersListView;

        private void onShowChooseProvider(ClickEvent evt)
        {
            // prepare data
            _providersListView.ScrollToSelectedItem();
            bool isDemoScene = false;
            {
                var resolver = GameObjectUtils.FindObjectOfType<SettingResolver>(includeInactive: true);

                if (resolver != null)
                {
                    var path = AssetDatabase.GetAssetPath(resolver.SettingsProvider);
                    isDemoScene = path.Contains("Kamgam");
                }
            }
            _providers = SettingsProvider.EditorFindAllProviders(excludeExampleProviders: !isDemoScene);

            // show/hide
            _chooseProviderContainer.style.display = DisplayStyle.Flex;
            _chooseSettingContainer.style.display = DisplayStyle.None;
            _chooseVisualContainer.style.display = DisplayStyle.None;
        }

        private void createChooseProviderGUI(VisualElement root)
        {
            var settings = SettingsGeneratorSettings.GetOrCreateSettings();

            _chooseProviderContainer = root.AddContainer("ChooseProvider", "grow", "content-container");
            var container = _chooseProviderContainer;

            container.AddLabel("Provider", "h1");
            var header = container.AddContainer("Header", "horizontal");

            container.AddLabel("All settings data is managed by one object called the 'Settings Provider'. \n\n" +
                "If your scene already contains settings UI then their provider can be used and you can skip this step in the future.", "word-wrap");
            container.AddToggleLeft("Skip if provider was found in scene.", settings.SkipProviderSelectionInMenuCreator, onSkipIfProviderWasFound);

            var firstTimeContainer = container.AddContainer("FirstTime", "warning-container", "pt-10", "pl-10", "pr-10", "pb-10", "mt-10", "br-5");
            firstTimeContainer.AddLabel("It seems this is the first time you are creating a setting.", "word-wrap", "h2");
            if (_selectedSettingsProvider != null)
            {
                firstTimeContainer.AddLabel("We have created a SettingsProvider for you under:\n'"
                    + AssetDatabase.GetAssetPath(_selectedSettingsProvider) + ".\n\n" +
                    "Please hit the 'Next' button below to proceed.\n" +
                    "HINT: This step can be skipped automatically in the future.",
                    "word-wrap", "mt-10"
                    );
            }
            firstTimeContainer.style.display = _isFirstTimeUser ? DisplayStyle.Flex : DisplayStyle.None;

            container.AddLabel("Please select the provider you wish to add the new setting to:", "word-wrap", "mt-10", "h3");

            var providerObjField = container.AddObjectField<SettingsProvider>("Provider:", _selectedSettingsProvider, onChangeSettingsProvider, allowSceneObjects: false, "mb-10", "dont-shrink");
            providerObjField.bindingPath = "_selectedSettingsProvider";
            providerObjField.Bind(_serializedObject);

            // Providers List View
            Button nextBtn = null;
            _providersListView = new ListView();
            _providersListView.showBoundCollectionSize = false;
            _providersListView.makeItem = () => new Label();
            _providersListView.bindItem = (e, i) => bindProviderItem(e as Label, i, _providersListView);
            _providersListView.reorderable = false;
            _providersListView.AddToClassList("grow");
#if UNITY_2022_1_OR_NEWER
            _providersListView.selectionChanged += (e) =>
#else
            _providersListView.onSelectionChange += (e) =>
#endif
            {
                onProviderSelectionChange(_providersListView.selectedIndex, _providersListView);
                nextBtn.SetEnabled(_selectedSettingsProvider != null);
            };
#if UNITY_2022_1_OR_NEWER
            _providersListView.itemsChosen += (e) =>
#else
            _providersListView.onItemsChosen += (e) =>
#endif
            {
                _genericSettingsLastSelectedIndex = _providersListView.selectedIndex;
                onShowChooseSetting(null);
                nextBtn.SetEnabled(_selectedSettingsProvider != null);
            };
            if (_selectedSettingsProvider != null)
            {
                var index = _providers.IndexOf(_selectedSettingsProvider);
                if (index >= 0)
                    _providersListView.SetSelectionWithoutNotify(new int[] { });
            }
            container.Add(_providersListView);
            _providersListView.bindingPath = "_providers";
            _providersListView.Bind(_serializedObject);

            var bottomMenu = container.AddContainer("BottomMenu", "horizontal", "mt-auto", "dont-shrink");
            nextBtn = bottomMenu.AddButton("Next", onShowChooseSetting, "grow");
            nextBtn.SetEnabled(_selectedSettingsProvider != null);
        }

        private void bindProviderItem(Label field, int index, ListView listView)
        {
            var provider = listView.GetItemSourceAt<SettingsProvider>(index);
            if (provider != null)
            {
                field.text = provider.name;
                var path = AssetDatabase.GetAssetPath(provider);
                if (path.Contains("Kamgam"))
                    field.AddToClassList("warning");
                else
                    field.RemoveFromClassList("warning");
            }
        }

        private void onProviderSelectionChange(int index, ListView listView)
        {
            if (index < 0)
            {
                _selectedSettingsProvider = null;
                return;
            }

            _selectedSettingsProvider = listView.GetItemSourceAt<SettingsProvider>(index);
        }

        private void onSkipIfProviderWasFound(ChangeEvent<bool> evt)
        {
            var settings = SettingsGeneratorSettings.GetOrCreateSettings();
            settings.SkipProviderSelectionInMenuCreator = evt.newValue;
            EditorUtility.SetDirty(settings);
        }
    }
}
