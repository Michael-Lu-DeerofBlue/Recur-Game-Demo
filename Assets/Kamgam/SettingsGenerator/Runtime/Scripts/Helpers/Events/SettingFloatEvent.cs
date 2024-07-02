using UnityEngine;
using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Add this to a GameObject in your scene and use the UnityEvents to react to setting changes.
    /// </summary>
    public class SettingFloatEvent : SettingEvent<float>
    {
        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            if (_supportedDataTypes == null)
                _supportedDataTypes = new SettingData.DataType[] {
                    SettingData.DataType.Int,
                    SettingData.DataType.Float,
                    SettingData.DataType.Option,
                    SettingData.DataType.ColorOption
                };

            return _supportedDataTypes;
        }

        /// <summary>
        /// Pulls the value from the setting and then triggers the event.<br />
        /// </summary>
        public override void TriggerEvent()
        {
            if (!HasActiveSettingForID(ID))
                return;

            var setting = GetSetting();
            if (setting != null)
            {
                if (setting.GetDataType() == SettingData.DataType.Int)
                {
                    var value = SettingsProvider.Settings.GetInt(ID).GetValue();
                    OnValueChanged?.Invoke(value);
                }
                else if (setting.GetDataType() == SettingData.DataType.Float)
                {
                    var value = SettingsProvider.Settings.GetFloat(ID).GetValue();
                    OnValueChanged?.Invoke(value);
                }
                else if (setting.GetDataType() == SettingData.DataType.Option)
                {
                    var value = SettingsProvider.Settings.GetOption(ID).GetValue();
                    OnValueChanged?.Invoke(value);
                }
                else if (setting.GetDataType() == SettingData.DataType.ColorOption)
                {
                    var value = SettingsProvider.Settings.GetColorOption(ID).GetValue();
                    OnValueChanged?.Invoke(value);
                }
            }
        }
    }
}
