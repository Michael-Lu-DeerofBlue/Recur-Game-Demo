using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public static class ISettingExtensions
    {
        public static float GetFloatValue(this ISetting setting)
        {
            var typedSetting = setting as SettingFloat;
            if (typedSetting == null)
            {
                throw new System.Exception("Setting is not a float setting!");
            }

            return typedSetting.GetValue();
        }

        public static float GetIntValue(this ISetting setting)
        {
            var intSetting = setting as SettingInt;
            if (intSetting != null)
                return intSetting.GetValue();

            var optionSetting = setting as SettingOption;
            if (optionSetting != null)
                return optionSetting.GetValue();

            var colorOptionSetting = setting as SettingColorOption;
            if (colorOptionSetting != null)
                return colorOptionSetting.GetValue();

            throw new System.Exception("Setting is not an integer, option or color option setting!");
        }

        public static bool GetBoolValue(this ISetting setting)
        {
            var typedSetting = setting as SettingBool;
            if (typedSetting == null)
            {
                throw new System.Exception("Setting is not a bool setting!");
            }

            return typedSetting.GetValue();
        }

        public static string GetStringValue(this ISetting setting)
        {
            var typedSetting = setting as SettingString;
            if (typedSetting == null)
            {
                throw new System.Exception("Setting is not a string setting!");
            }

            return typedSetting.GetValue();
        }

        public static Color GetColorValue(this ISetting setting)
        {
            var typedSetting = setting as SettingColor;
            if (typedSetting == null)
            {
                throw new System.Exception("Setting is not a color setting!");
            }

            return typedSetting.GetValue();
        }

        public static int GetColorOptionValue(this ISetting setting)
        {
            var typedSetting = setting as SettingColorOption;
            if (typedSetting == null)
            {
                throw new System.Exception("Setting is not a color option setting!");
            }

            return typedSetting.GetValue();
        }

        public static KeyCombination GetKeyCombinationValue(this ISetting setting)
        {
            var typedSetting = setting as SettingKeyCombination;
            if (typedSetting == null)
            {
                throw new System.Exception("Setting is not a KeyCombination setting!");
            }

            return typedSetting.GetValue();
        }

        public static int GetOptionValue(this ISetting setting)
        {
            var typedSetting = setting as SettingOption;
            if (typedSetting == null)
            {
                throw new System.Exception("Setting is not a option setting!");
            }

            return typedSetting.GetValue();
        }
    }
}
