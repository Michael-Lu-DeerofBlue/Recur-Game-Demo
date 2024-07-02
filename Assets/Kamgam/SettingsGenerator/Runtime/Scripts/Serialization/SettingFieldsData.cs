using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    [System.Serializable]
    public class SettingFieldsData
    {
        public List<SettingData> Fields;

        public SettingFieldsData(List<SettingData> fields)
        {
            Fields = fields;
        }
    }
}
