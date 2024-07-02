using UnityEngine;
using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Add this to a GameObject in your scene and use the UnityEvents to react to setting changes.
    /// </summary>
    public class SettingColorEvent : SettingEvent<Color>
    {
        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            if (_supportedDataTypes == null)
                _supportedDataTypes = new SettingData.DataType[] { SettingData.DataType.Color, SettingData.DataType.ColorOption };

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
            if (setting == null)
                return;

            if (setting.GetDataType() == SettingData.DataType.Color)
            {
                var color = SettingsProvider.Settings.GetColor(ID).GetValue();
                OnValueChanged?.Invoke(color);
            }
            else if (setting.GetDataType() == SettingData.DataType.ColorOption)
            {
                var color = SettingsProvider.Settings.GetColorOption(ID).GetColorValue(Color.white);
                OnValueChanged?.Invoke(color);
            }
        }
    }
}
