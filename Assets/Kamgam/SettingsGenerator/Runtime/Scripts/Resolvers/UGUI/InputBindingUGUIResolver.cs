using UnityEngine;
using Kamgam.UGUIComponentsForSettings;
using System.Text.RegularExpressions;

namespace Kamgam.SettingsGenerator
{
    [AddComponentMenu("UI/Settings/InputBindingUGUIResolver")]
    [RequireComponent(typeof(InputBindingUGUI))]
    public class InputBindingUGUIResolver : SettingResolver, ISettingResolver
    {
        protected InputBindingUGUI inputBindingUGUI;
        public InputBindingUGUI InputBindingUGUI
        {
            get
            {
                if (inputBindingUGUI == null)
                {
                    inputBindingUGUI = this.GetComponent<InputBindingUGUI>();
                }
                return inputBindingUGUI;
            }
        }

        [System.NonSerialized]
        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[]
        {
            SettingData.DataType.String
        };

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            return supportedDataTypes;
        }

        protected bool stopPropagation = false;

        public override void Start()
        {
            base.Start();

            InputBindingUGUI.OnChanged += onChanged;

            // Hook up with the localization.
            InputBindingUGUI.PathToDisplayNameFunc = localizeKeyCode;
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

            if (InputBindingUGUI != null)
            {
                InputBindingUGUI.OnChanged -= onChanged;
                InputBindingUGUI.PathToDisplayNameFunc = null;
            }

            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
                LocalizationProvider.GetLocalization().RemoveOnLanguageChangedListener(onLanguageChanged);
        }

        protected void onLanguageChanged(string language)
        {
            Refresh();
        }

        protected string localizeKeyCode(string bindingPath)
        {
            // Only use the translation if it is available.
            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
            {
                string term = bindingPath;
                if (LocalizationProvider.GetLocalization().HasTerm(term))
                {
                    return LocalizationProvider.GetLocalization().Get(term);
                }
            }
            
            return bindingPathToDisplayName(bindingPath);
        }

        protected void onChanged(string bindingPath)
        {
            if (stopPropagation)
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var setting = SettingsProvider.Settings.GetString(ID);
            setting.SetValue(bindingPath);
        }

        public override void Refresh()
        {
            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var setting = SettingsProvider.Settings.GetString(ID);

            if (setting == null)
                return;

            InputBindingUGUI.InputBinding.SetBindingPath(setting.GetValue());
            InputBindingUGUI.UpdateDisplayName();

            try
            {
                stopPropagation = true;

                if (InputBindingUGUI.PathToDisplayNameFunc == null)
                {
                    InputBindingUGUI.PathToDisplayNameFunc = localizeKeyCode;
                }

                InputBindingUGUI.DisplayName = localizeKeyCode(setting.GetValue());
            }
            finally
            {
                stopPropagation = false;
            }
        }

        protected string bindingPathToDisplayName(string bindingPath)
        {
            if (bindingPath == null)
                return null;

            // This is geared toward paths like "<Keyboard>/s" => "S";
            bindingPath = Regex.Replace(bindingPath, "<[^>]*>/", "");
            if (bindingPath.Length < 6)
                bindingPath = bindingPath.ToUpper();

            return bindingPath;
        }
    }
}
