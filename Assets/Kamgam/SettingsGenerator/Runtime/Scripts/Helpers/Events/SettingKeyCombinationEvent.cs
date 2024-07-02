using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Kamgam.UGUIComponentsForSettings;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Add this to a GameObject in your scene and use the UnityEvents to react to setting changes.
    /// </summary>
    public class SettingKeyCombinationEvent : SettingEvent<KeyCombination>
    {
        [System.NonSerialized]
        protected KeyCombination _combo;

        public UnityEvent<KeyCombination> OnDown;
        public UnityEvent<KeyCombination> OnUp;
        public UnityEvent<KeyCombination> OnHold;

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            if (_supportedDataTypes == null)
                _supportedDataTypes = new SettingData.DataType[] { 
                    SettingData.DataType.KeyCombination
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
                if (setting.GetDataType() == SettingData.DataType.KeyCombination)
                {
                    var value = SettingsProvider.Settings.GetKeyCombination(ID).GetValue();
                    OnValueChanged?.Invoke(value);
                    _combo = value;
                }
            }
        }

        public void Update()
        {
            if (OnDown != null)
            {
                bool modifierCheckPassed = InputUtils.GetUniversalKeyDown(_combo.ModifierKey) || _combo.ModifierKey == UniversalKeyCode.None || _combo.ModifierKey == UniversalKeyCode.Unknown;
                if (modifierCheckPassed && InputUtils.GetUniversalKeyDown(_combo.Key))
                {
                    OnDown?.Invoke(_combo);
                }
            }

            if (OnUp != null)
            {
                bool modifierCheckPassed = InputUtils.GetUniversalKeyUp(_combo.ModifierKey) || _combo.ModifierKey == UniversalKeyCode.None || _combo.ModifierKey == UniversalKeyCode.Unknown;
                if (modifierCheckPassed && InputUtils.GetUniversalKeyUp(_combo.Key))
                {
                    OnUp?.Invoke(_combo);
                }
            }

            if (OnHold != null)
            {
                bool modifierCheckPassed = InputUtils.GetUniversalKey(_combo.ModifierKey) || _combo.ModifierKey == UniversalKeyCode.None || _combo.ModifierKey == UniversalKeyCode.Unknown;
                if (modifierCheckPassed && InputUtils.GetUniversalKey(_combo.Key))
                {
                    OnHold?.Invoke(_combo);
                }
            }
        }
    }
}
