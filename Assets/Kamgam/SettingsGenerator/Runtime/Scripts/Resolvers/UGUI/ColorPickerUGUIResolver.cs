using UnityEngine;
using Kamgam.UGUIComponentsForSettings;

namespace Kamgam.SettingsGenerator
{
    [AddComponentMenu("UI/Settings/ColorPickerUGUIResolver")]
    [RequireComponent(typeof(ColorPickerUGUI))]
    public class ColorPickerUGUIResolver : SettingResolver, ISettingResolver
    {
        protected ColorPickerUGUI colorPickerUGUI;
        public ColorPickerUGUI ColorPickerUGUI
        {
            get
            {
                if (colorPickerUGUI == null)
                {
                    colorPickerUGUI = this.GetComponent<ColorPickerUGUI>();
                }
                return colorPickerUGUI;
            }
        }

        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[] { SettingData.DataType.ColorOption, SettingData.DataType.Int };

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            return supportedDataTypes;
        }

        protected bool stopPropagation = false;

        public override void Start()
        {
            base.Start();

            ColorPickerUGUI.OnSelectionChanged += onSelectionChanged;

            if (HasValidSettingForID(ID, GetSupportedDataTypes()) && HasActiveSettingForID(ID))
            {
                var setting = SettingsProvider.Settings.GetSetting(ID);
                setting.AddPulledFromConnectionListener(Refresh);

                Refresh();
            }
        }

        private void onSelectionChanged(int selectedIndex)
        {
            if (stopPropagation)
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var settingOption = SettingsProvider.Settings.GetColorOption(ID);
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

            var settingOption = SettingsProvider.Settings.GetColorOption(ID);
            if (settingOption != null)
            {
                stopPropagation = true;
                try
                {
                    refreshOptions();
                    ColorPickerUGUI.SelectedIndex = settingOption.GetValue();
                }
                finally
                {
                    stopPropagation = false;
                }
            }
            else
            {
                var settingInt = SettingsProvider.Settings.GetInt(ID);
                if (settingInt != null)
                {
                    stopPropagation = true;
                    try
                    { 
                        settingInt.PullFromConnection();
                        ColorPickerUGUI.SelectedIndex = settingInt.GetValue();
                        stopPropagation = false;
                    }
                    finally
                    {
                        stopPropagation = false;
                    }
                }
            }
        }

        private void refreshOptions()
        {
            if (!HasActiveSettingForID(ID))
                return;

            var settingOptions = SettingsProvider.Settings.GetColorOption(ID);
            if (settingOptions != null)
            {
                if (settingOptions.HasOptions())
                {
                    ColorPickerUGUI.SetColorOptions(settingOptions.GetOptionLabels());
                }
                else
                {
                    var optionsFromUI = ColorPickerUGUI.GetColorOptions();
                    settingOptions.SetOptionLabels(optionsFromUI);
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (ColorPickerUGUI != null)
                ColorPickerUGUI.OnSelectionChanged -= onSelectionChanged;
        }
    }
}
