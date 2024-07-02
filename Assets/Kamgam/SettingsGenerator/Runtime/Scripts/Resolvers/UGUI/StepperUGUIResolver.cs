using UnityEngine;
using Kamgam.UGUIComponentsForSettings;

namespace Kamgam.SettingsGenerator
{
    [AddComponentMenu("UI/Settings/StepperUGUIResolver")]
    [RequireComponent(typeof(StepperUGUI))]
    public class StepperUGUIResolver : SettingResolver, ISettingResolver
    {
        protected StepperUGUI stepperUGUI;
        public StepperUGUI StepperUGUI
        {
            get
            {
                if (stepperUGUI == null)
                {
                    stepperUGUI = this.GetComponent<StepperUGUI>();
                }
                return stepperUGUI;
            }
        }

        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[] { SettingData.DataType.Int, SettingData.DataType.Float };

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            return supportedDataTypes;
        }

        protected bool stopPropagation = false;

        public override void Start()
        {
            base.Start();

            StepperUGUI.WholeNumbers = GetDataType() == SettingData.DataType.Int;
            StepperUGUI.OnValueChanged += onValueChanged;

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

            if (StepperUGUI != null)
                StepperUGUI.OnValueChanged -= onValueChanged;
        }

        private void onValueChanged(float value)
        {
            if (stopPropagation)
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var settingInt = SettingsProvider.Settings.GetInt(ID);
            if(settingInt != null)
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
                    StepperUGUI.Value = settingInt.GetValue();
                }
                else
                {
                    var settingFloat = SettingsProvider.Settings.GetFloat(ID);
                    if (settingFloat != null)
                    {
                        StepperUGUI.Value = settingFloat.GetValue();
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
