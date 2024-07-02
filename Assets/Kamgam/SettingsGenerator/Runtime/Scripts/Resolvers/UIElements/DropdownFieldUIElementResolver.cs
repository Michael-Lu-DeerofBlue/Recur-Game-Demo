// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Kamgam.LocalizationForSettings;

namespace Kamgam.SettingsGenerator
{
    public class DropdownFieldUIElementResolver : SettingResolverForVisualElement, ISettingResolver
    {
        protected DropdownField _dropDown;
        public DropdownField DropDown
        {
            get
            {
                if ((_dropDown == null && VisualElement != null) || _dropDown != VisualElement)
                {
                    _dropDown = VisualElement as DropdownField;
                    if (_dropDown != null)
                    {
                        _dropDown.RegisterValueChangedCallback(onSelectionChanged);
                    }
                }
                return _dropDown;
            }
        }

        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[] { SettingData.DataType.Option, SettingData.DataType.Int };

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            return supportedDataTypes;
        }

        protected bool stopPropagation = false;

        public override void Start()
        {
            base.Start();

            // Hook up with the localization.
            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
            {
                LocalizationProvider.GetLocalization().AddOnLanguageChangedListener(onLanguageChanged);
            }

            if (HasValidSettingForID(ID, GetSupportedDataTypes()))
            {
                var setting = SettingsProvider.Settings.GetSetting(ID);
                setting.AddPulledFromConnectionListener(Refresh);
            }
        }

        public override void OnDisable()
        {
            // Rest to trigger fetching the label again OnEnable.
            _dropDown = null;
            base.OnDisable();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (DropDown != null)
                DropDown.UnregisterValueChangedCallback(onSelectionChanged);

            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
                LocalizationProvider.GetLocalization().RemoveOnLanguageChangedListener(onLanguageChanged);
        }

        protected void onLanguageChanged(string language)
        {
            Refresh();
        }

        protected void onSelectionChanged(ChangeEvent<string> evt)
        {
            int selectedIndex = DropDown.choices.IndexOf(evt.newValue);
            if (selectedIndex < 0)
                return;

            if (stopPropagation)
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var settingOption = SettingsProvider.Settings.GetOption(ID);
            if (settingOption != null)
            {
                settingOption.SetValue(selectedIndex);
            }
            else
            {
                var settingInt = SettingsProvider.Settings.GetInt(ID);
                if (settingInt != null)
                {
                    settingInt.SetValue(selectedIndex);
                }
            }
        }

        public override void Refresh()
        {
            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            try
            {
                stopPropagation = true;

                var settingOption = SettingsProvider.Settings.GetOption(ID);
                if (settingOption != null)
                {
                        refreshOptions();
                        DropDown.index = settingOption.GetValue();
                }
                else
                {
                    var settingInt = SettingsProvider.Settings.GetInt(ID);
                    if (settingInt != null)
                    {
                        DropDown.index = settingInt.GetValue();
                    }
                }
            }
            finally
            {
                stopPropagation = false;
            }
        }

        protected List<string> _localizedOptionLabels = new List<string>(3);

        protected void refreshOptions()
        {
            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            // If the settings has options then override them.
            var settingOptions = SettingsProvider.Settings.GetOption(ID);
            if (settingOptions.HasOptions())
            {
                var labels = new List<string>(settingOptions.GetOptionLabels());

                // localize labels
                if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
                {
                    // We do not want to change the original labels because they are used as the localization terms.
                    // Thus we use a local list to get a copy of the translated labels.
                    LocalizationProvider.GetLocalization().LocalizeList(labels, _localizedOptionLabels);
                    DropDown.choices = _localizedOptionLabels;
                }
                else
                {
                    DropDown.choices = labels;
                }
            }
            else
            {
                // Options from GUI

                // We do not want to change the original labels because they are used as the localization terms.
                // Thus we use a local list to get a copy of the translated labels.
                LocalizationProvider.GetLocalization().LocalizeList(DropDown.choices, _localizedOptionLabels);
                DropDown.choices = _localizedOptionLabels;
            }
        }
    }
}
#endif