using UnityEngine;
using Kamgam.UGUIComponentsForSettings;

namespace Kamgam.SettingsGenerator
{
    [AddComponentMenu("UI/Settings/TextfieldUGUIResolver")]
    [RequireComponent(typeof(TextfieldUGUI))]
    public class TextfieldUGUIResolver : SettingResolver, ISettingResolver
    {
        protected TextfieldUGUI textfieldUGUI;
        public TextfieldUGUI TextfieldUGUI
        {
            get
            {
                if (textfieldUGUI == null)
                {
                    textfieldUGUI = this.GetComponent<TextfieldUGUI>();
                }
                return textfieldUGUI;
            }
        }

        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[] { SettingData.DataType.String };

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            return supportedDataTypes;
        }

        protected bool stopPropagation = false;

        public override void Start()
        {
            base.Start();

            TextfieldUGUI.OnTextChanged += onTextChanged;

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

            if (TextfieldUGUI != null)
                TextfieldUGUI.OnTextChanged -= onTextChanged;
        }

        private void onTextChanged(string text)
        {
            if (stopPropagation)
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var setting = SettingsProvider.Settings.GetString(ID);
            if (setting != null)
            {
                setting.SetValue(text);
            }
        }

        public override void Refresh()
        {
            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            try
            {
                stopPropagation = true;

                var setting = SettingsProvider.Settings.GetString(ID);
                if (setting != null)
                {
                    TextfieldUGUI.Text = setting.GetValue();
                }
            }
            finally
            {
                stopPropagation = false;
            }
        }
    }
}
