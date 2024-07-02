using UnityEngine;
using Kamgam.UGUIComponentsForSettings;
using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    [AddComponentMenu("UI/Settings/DropDownUGUIResolver")]
    [RequireComponent(typeof(DropDownUGUI))]
    public class DropDownUGUIResolver : SettingResolver, ISettingResolver
    {
        protected DropDownUGUI dropDownUGUI;
        public DropDownUGUI DropDownUGUI
        {
            get
            {
                if (dropDownUGUI == null)
                {
                    dropDownUGUI = this.GetComponent<DropDownUGUI>();
                }
                return dropDownUGUI;
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

            DropDownUGUI.OnSelectionChanged += onSelectionChanged;

            // Hook up with the localization.
            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
            {
                LocalizationProvider.GetLocalization().AddOnLanguageChangedListener(onLanguageChanged);
            }

            if (HasValidSettingForID(ID, GetSupportedDataTypes()) && HasActiveSettingForID(ID))
            {
                var setting = SettingsProvider.Settings.GetSetting(ID);
                setting.AddPulledFromConnectionListener(Refresh);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (DropDownUGUI != null)
                DropDownUGUI.OnSelectionChanged -= onSelectionChanged;

            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
                LocalizationProvider.GetLocalization().RemoveOnLanguageChangedListener(onLanguageChanged);
        }

        protected void onLanguageChanged(string language)
        {
            Refresh();
        }

        protected void onSelectionChanged(int selectedIndex)
        {
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
                        DropDownUGUI.SelectedIndex = settingOption.GetValue();
                }
                else
                {
                    var settingInt = SettingsProvider.Settings.GetInt(ID);
                    if (settingInt != null)
                    {
                        DropDownUGUI.SelectedIndex = settingInt.GetValue();
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
            if (!HasActiveSettingForID(ID))
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
                    DropDownUGUI.SetOptions(_localizedOptionLabels);
                }
                else
                {
                    DropDownUGUI.SetOptions(labels);
                }
            }
            else
            {
                // Options from GUI

                // We do not want to change the original labels because they are used as the localization terms.
                // Thus we use a local list to get a copy of the translated labels.
                LocalizationProvider.GetLocalization().LocalizeList(DropDownUGUI.GetOptions(), _localizedOptionLabels);
                DropDownUGUI.SetOptions(_localizedOptionLabels);
            }
        }
    }
}
