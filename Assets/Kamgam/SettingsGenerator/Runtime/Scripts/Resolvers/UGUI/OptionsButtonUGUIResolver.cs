using UnityEngine;
using Kamgam.UGUIComponentsForSettings;
using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    [AddComponentMenu("UI/Settings/OptionsButtonUGUIResolver")]
    [RequireComponent(typeof(OptionsButtonUGUI))]
    public class OptionsButtonUGUIResolver : SettingResolver
    {
        protected OptionsButtonUGUI optionsButtonUGUI;
        public OptionsButtonUGUI OptionsButtonUGUI
        {
            get
            {
                if (optionsButtonUGUI == null)
                {
                    optionsButtonUGUI = this.GetComponent<OptionsButtonUGUI>();
                }
                return optionsButtonUGUI;
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

            OptionsButtonUGUI.OnValueChanged += onValueChanged;

            // Hook up with the localization.
            OptionsButtonUGUI.OptionToTextFunc = LocalizationProvider.GetLocalization().Get;
            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
            {
                LocalizationProvider.GetLocalization().AddOnLanguageChangedListener(onLanguageChanged);
            }

            if (HasValidSettingForID(ID, GetSupportedDataTypes()) && HasActiveSettingForID(ID))
            {
                var setting = SettingsProvider.Settings.GetSetting(ID);
                setting.AddPulledFromConnectionListener(Refresh);

                Refresh();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (OptionsButtonUGUI != null)
            {
                OptionsButtonUGUI.OnValueChanged -= onValueChanged;
                OptionsButtonUGUI.OptionToTextFunc = null;
            }

            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
                LocalizationProvider.GetLocalization().RemoveOnLanguageChangedListener(onLanguageChanged);
        }

        protected void onLanguageChanged(string language)
        {
            Refresh();
        }

        private void onValueChanged(int selectedIndex)
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

            OptionsButtonUGUI.UpdateText();

            try
            {
                stopPropagation = true;

                var settingOption = SettingsProvider.Settings.GetOption(ID);
                if (settingOption != null)
                {
                    refreshOptions();
                    OptionsButtonUGUI.SelectedIndex = settingOption.GetValue();
                }
                else
                {
                    var settingInt = SettingsProvider.Settings.GetInt(ID);
                    if (settingInt != null)
                    {
                        OptionsButtonUGUI.SelectedIndex = settingInt.GetValue();
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
            if (settingOptions != null)
            {
                if (settingOptions.HasOptions())
                {
                    // localize labels
                    var labels = settingOptions.GetOptionLabels();

                    if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
                    {
                        // We do not want to change the original labels because they are used as the localization terms.
                        // Thus we use a local list to get a copy of the translated labels.
                        LocalizationProvider.GetLocalization().LocalizeList(labels, _localizedOptionLabels);
                        OptionsButtonUGUI.SetOptions(_localizedOptionLabels);
                    }
                    else
                    {
                        // Options from GUI

                        // We do not want to change the original labels because they are used as the localization terms.
                        // Thus we use a local list to get a copy of the translated labels.
                        LocalizationProvider.GetLocalization().LocalizeList(OptionsButtonUGUI.GetOptions(), _localizedOptionLabels);
                        OptionsButtonUGUI.SetOptions(_localizedOptionLabels);
                    }

                }
            }
        }

    }
}
