using UnityEngine;
using Kamgam.UGUIComponentsForSettings;

namespace Kamgam.SettingsGenerator
{
    [AddComponentMenu("UI/Settings/InputKeyUGUIResolver")]
    [RequireComponent(typeof(InputKeyUGUI))]
    public class InputKeyUGUIResolver : SettingResolver, ISettingResolver
    {
        protected InputKeyUGUI inputKeyUGUI;
        public InputKeyUGUI InputKeyUGUI
        {
            get
            {
                if (inputKeyUGUI == null)
                {
                    inputKeyUGUI = this.GetComponent<InputKeyUGUI>();
                }
                return inputKeyUGUI;
            }
        }

        [System.NonSerialized]
        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[]
        {
            SettingData.DataType.KeyCombination
        };

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            return supportedDataTypes;
        }

        protected bool stopPropagation = false;

        public override void Start()
        {
            base.Start();

            InputKeyUGUI.OnChanged += onChanged;

            // Hook up with the localization.
            InputKeyUGUI.KeyCodeToKeyNameFunc = localizeKeyCode;
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

            if (InputKeyUGUI != null)
            {
                InputKeyUGUI.OnChanged -= onChanged;
                InputKeyUGUI.KeyCodeToKeyNameFunc = null;
            }

            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
                LocalizationProvider.GetLocalization().RemoveOnLanguageChangedListener(onLanguageChanged);
        }

        protected void onLanguageChanged(string language)
        {
            Refresh();
        }

        protected string localizeKeyCode(UniversalKeyCode keyCode)
        {
            // Only use the translation if it available.
            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
            {
                string term = keyCode.ToString();
                if (LocalizationProvider.GetLocalization().HasTerm(term))
                {
                    return LocalizationProvider.GetLocalization().Get(term);
                }
            }
            
            return InputUtils.UniversalKeyName(keyCode);
        }

        protected void onChanged(UniversalKeyCode key, UniversalKeyCode modifierKey)
        {
            if (stopPropagation)
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var settingKey = SettingsProvider.Settings.GetKeyCombination(ID);
            if (settingKey != null)
            {
                settingKey.SetValue(new KeyCombination(key, modifierKey));
            }
        }

        public override void Refresh()
        {
            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            InputKeyUGUI.UpdateKeyName();

            var setting = SettingsProvider.Settings.GetKeyCombination(ID);

            if (setting == null)
                return;

            try
            {
                stopPropagation = true;

                if (InputKeyUGUI.KeyCodeToKeyNameFunc == null)
                {
                    InputKeyUGUI.KeyCodeToKeyNameFunc = localizeKeyCode;
                }

                InputKeyUGUI.Key = setting.GetValue().Key;
                InputKeyUGUI.ModifierKey = setting.GetValue().ModifierKey;
            }
            finally
            {
                stopPropagation = false;
            }
        }
    }
}
