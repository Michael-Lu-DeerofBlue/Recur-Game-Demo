using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    // Sadly JsonUtility still does not support a JsonIgnore Attribute.
    // Therefore we have to used this workaround.
    public static class SettingsSerializer
    {
        public static string ToJson(Settings settings)
        {
            var data = new SettingFieldsData(new List<SettingData>());

            var fields = settings.GetAllSettings();
            foreach (var setting in fields)
            {
                if (string.IsNullOrEmpty(setting.GetID()))
                    continue;

                if (!setting.IsActive)
                    continue;

                var settingData = setting.SerializeValueToData();

                // Ignore unknown data types
                if (settingData.Type == SettingData.DataType.Unknown)
                {
                    Debug.LogError("SGSettings: Unknown data type for path '" + settingData.ID + "'. Ignoring.");
                    continue;
                }

                data.Fields.Add(settingData);
            }

            var json = JsonUtility.ToJson(data);

            // Debug.Log(json);

            return json;
        }


        /// <summary>
        /// Deserializes the data from json to settings and marks the settings which
        /// have been deserialized as having user data.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="settings"></param>
        public static void FromJson(string json, Settings settings)
        {
            var data = JsonUtility.FromJson<SettingFieldsData>(json);
            var fields = settings.GetAllSettings();

            // Fill known fields
            foreach (var setting in fields)
            {
                foreach (var settingData in data.Fields)
                {
                    if (settingData.ID == setting.GetID())
                    {
                        if (settingData.Type == SettingData.DataType.Unknown)
                        {
                            Debug.LogError("SGSettings: Unknown data type for path '" + settingData.ID + "'. Ignoring.");
                            break;
                        }

                        setting.DeserializeValueFromData(settingData);
                        // Mark as having user data
                        setting.SetHasUserData(true);
                        break;
                    }
                }
            }

            // Create settings for data fields which are not in the settings.
            // These might be conntected later (via script).
            foreach (var settingData in data.Fields)
            {
                bool found = false;
                foreach (var setting in fields)
                {
                    if (settingData.ID == setting.GetID())
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    if (settingData.Type == SettingData.DataType.Unknown)
                    {
                        Debug.LogError("SGSettings: Unknown data type for path '" + settingData.ID + "'. Ignoring.");
                        break;
                    }

                    ISetting newSetting = null;
                    switch (settingData.Type)
                    {
                        case SettingData.DataType.Int:
                            newSetting = settings.AddIntFromSerializedData(settingData);
                            break;
                        case SettingData.DataType.Float:
                            newSetting = settings.AddFloatFromSerializedData(settingData);
                            break;
                        case SettingData.DataType.Bool:
                            newSetting = settings.AddBoolFromSerializedData(settingData);
                            break;
                        case SettingData.DataType.String:
                            newSetting = settings.AddStringFromSerializedData(settingData);
                            break;
                        case SettingData.DataType.Color:
                            newSetting = settings.AddColorFromSerializedData(settingData);
                            break;
                        case SettingData.DataType.KeyCombination:
                            newSetting = settings.AddKeyCombinationFromSerializedData(settingData);
                            break;
                        case SettingData.DataType.Option:
                            newSetting = settings.AddOptionFromSerializedData(settingData);
                            break;
                        case SettingData.DataType.ColorOption:
                            newSetting = settings.AddColorOptionFromSerializedData(settingData);
                            break;
                        default:
                        case SettingData.DataType.Unknown:
                            break;
                    }
                    // Mark as having user data
                    if(newSetting != null)
                        newSetting.SetHasUserData(true);
                }
            }

            settings.RebuildSettingsCache();
        }
    }
}
