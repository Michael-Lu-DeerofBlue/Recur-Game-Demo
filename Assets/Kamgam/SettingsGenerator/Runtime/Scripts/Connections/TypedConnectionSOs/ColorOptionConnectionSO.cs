using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    // This base class exists primarily to trick the UnityEditor into
    // filtering the selection options. But it also is a nice way to
    // shorten the implementation classes.

    public abstract class ColorOptionConnectionSO : ConnectionSO, IConnectionSO<IConnectionWithOptions<Color>>
    {
        public abstract IConnectionWithOptions<Color> GetConnection();

        public override SettingData.DataType GetDataType()
        {
            return SettingData.DataType.ColorOption;
        }
    }
}
