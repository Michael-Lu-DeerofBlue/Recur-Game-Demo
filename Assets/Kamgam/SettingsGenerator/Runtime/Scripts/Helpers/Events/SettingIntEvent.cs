using UnityEngine;
using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Add this to a GameObject in your scene and use the UnityEvents to react to setting changes.
    /// </summary>
    public class SettingIntEvent : SettingEvent<int>
    {
        public enum FloatToIntConversion { Round, Ceil, Floor }

        [Tooltip("If the input value is a float then this defines how it will be converted to an int.")]
        public FloatToIntConversion FloatToInt = FloatToIntConversion.Round;

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
            if(setting != null)
            {
                if (setting.GetDataType() == SettingData.DataType.Int)
                {
                    var value = SettingsProvider.Settings.GetInt(ID).GetValue();
                    OnValueChanged?.Invoke(value);
                }
                else if (setting.GetDataType() == SettingData.DataType.Float)
                {
                    var value = SettingsProvider.Settings.GetFloat(ID).GetValue();
                    int intValue = 0;
                    switch (FloatToInt)
                    {
                        case FloatToIntConversion.Ceil:
                            intValue = Mathf.CeilToInt(value);
                            break;
                        case FloatToIntConversion.Floor:
                            intValue = Mathf.FloorToInt(value);
                            break;
                        case FloatToIntConversion.Round:
                        default:
                            intValue = Mathf.RoundToInt(value);
                            break;
                    }

                    OnValueChanged?.Invoke(intValue);
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
