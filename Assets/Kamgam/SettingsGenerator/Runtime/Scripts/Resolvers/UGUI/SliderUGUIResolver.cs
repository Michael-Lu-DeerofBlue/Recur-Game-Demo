using UnityEngine;
using Kamgam.UGUIComponentsForSettings;

namespace Kamgam.SettingsGenerator
{
    [AddComponentMenu("UI/Settings/SliderUGUIResolver")]
    [RequireComponent(typeof(SliderUGUI))]
    public class SliderUGUIResolver : SettingResolver, ISettingResolver
    {
        protected SliderUGUI _sliderUGUI;
        public SliderUGUI SliderUGUI
        {
            get
            {
                if (_sliderUGUI == null)
                {
                    _sliderUGUI = this.GetComponent<SliderUGUI>();
                }
                return _sliderUGUI;
            }
        }

        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[] { SettingData.DataType.Int, SettingData.DataType.Float };

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            return supportedDataTypes;
        }

        protected float _lastValue = float.NegativeInfinity;
        protected bool stopPropagation = false;

        public override void Start()
        {
            base.Start();

            SliderUGUI.WholeNumbers = GetDataType() == SettingData.DataType.Int;
            SliderUGUI.OnValueChanged += onValueChanged;

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

            if (SliderUGUI != null)
                SliderUGUI.OnValueChanged -= onValueChanged;
        }

        private void onValueChanged(float value)
        {
            if (stopPropagation)
                return;

            if (Mathf.Approximately(_lastValue, value))
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var settingInt = SettingsProvider.Settings.GetInt(ID);
            if (settingInt != null)
            {
                settingInt.SetValue(Mathf.RoundToInt(value));
            }
            else
            {
                var settingFloat = SettingsProvider.Settings.GetFloat(ID);
                if(settingFloat != null)
                {
                    settingFloat.SetValue(value);
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

                var settingInt = SettingsProvider.Settings.GetInt(ID);
                if (settingInt != null)
                {
                    SliderUGUI.Value = settingInt.GetValue();
                }
                else
                {
                    var settingFloat = SettingsProvider.Settings.GetFloat(ID);
                    if (settingFloat != null)
                    {
                        SliderUGUI.Value = settingFloat.GetValue();
                    }
                }
            }
            finally
            {
                stopPropagation = false;
            }
        }
    }
}
