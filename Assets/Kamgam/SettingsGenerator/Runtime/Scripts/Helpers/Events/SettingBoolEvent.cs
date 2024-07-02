using UnityEngine;
using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Add this to a GameObject in your scene and use the UnityEvents to react to setting changes.
    /// </summary>
    public class SettingBoolEvent : SettingEvent<bool>
    {
        public static string FalseStringValue0 = "";
        public static string FalseStringValue1 = null;
        public static int FalseIntValue = 0;
        public static float FalseFloatValue = 0f;
        public static Color FalseColorValue = Color.black;

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            if (_supportedDataTypes == null)
                _supportedDataTypes = new SettingData.DataType[] { 
                    SettingData.DataType.Bool,
                    SettingData.DataType.Int, 
                    SettingData.DataType.Float,
                    SettingData.DataType.Color,
                    SettingData.DataType.String
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
                if (setting.GetDataType() == SettingData.DataType.Bool)
                {
                    var value = SettingsProvider.Settings.GetBool(ID).GetValue();
                    OnValueChanged?.Invoke(value);
                }
                else if (setting.GetDataType() == SettingData.DataType.Int)
                {
                    var intValue = SettingsProvider.Settings.GetInt(ID).GetValue();
                    var boolValue = intValue != FalseIntValue;
                    OnValueChanged?.Invoke(boolValue);
                }
                else if (setting.GetDataType() == SettingData.DataType.Float)
                {
                    var floatValue = SettingsProvider.Settings.GetFloat(ID).GetValue();
                    var boolValue = floatValue != FalseFloatValue;
                    OnValueChanged?.Invoke(boolValue);
                }
                else if (setting.GetDataType() == SettingData.DataType.Color)
                {
                    var colorValue = SettingsProvider.Settings.GetColor(ID).GetValue();
                    var boolValue = colorValue != FalseColorValue;
                    OnValueChanged?.Invoke(boolValue);
                }
                else if (setting.GetDataType() == SettingData.DataType.String)
                {
                    var stringValue = SettingsProvider.Settings.GetString(ID).GetValue();
                    var boolValue = stringValue != FalseStringValue0 && stringValue != FalseStringValue1;
                    OnValueChanged?.Invoke(boolValue);
                }
            }
        }
    }
}
