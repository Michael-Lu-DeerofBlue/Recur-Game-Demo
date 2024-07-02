using UnityEngine;
using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Add this to a GameObject in your scene and use the UnityEvents to react to setting changes.
    /// </summary>
    public class SettingStringEvent : SettingEvent<string>
    {
        public string FloatFormat = "{0:0.00}";

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            if (_supportedDataTypes == null)
                _supportedDataTypes = new SettingData.DataType[] {
                    SettingData.DataType.String,
                    SettingData.DataType.Bool,
                    SettingData.DataType.Int, 
                    SettingData.DataType.Float
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
            if(setting != null)
            {
                if (setting.GetDataType() == SettingData.DataType.String)
                {
                    var value = SettingsProvider.Settings.GetString(ID).GetValue();
                    OnValueChanged?.Invoke(value);
                }
                else if (setting.GetDataType() == SettingData.DataType.Bool)
                {
                    var value = SettingsProvider.Settings.GetBool(ID).GetValue();
                    OnValueChanged?.Invoke(value.ToString());
                }
                else if (setting.GetDataType() == SettingData.DataType.Int)
                {
                    var intValue = SettingsProvider.Settings.GetInt(ID).GetValue();
                    OnValueChanged?.Invoke(intValue.ToString());
                }
                else if (setting.GetDataType() == SettingData.DataType.Float)
                {
                    var floatValue = SettingsProvider.Settings.GetFloat(ID).GetValue();
                    OnValueChanged?.Invoke(string.Format(FloatFormat, floatValue));
                }
            }
        }
    }
}
