using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    // This base class exists primarily to trick the UnityEditor into
    // filtering the selection options. But it also is a nice way to
    // shorten the implementation classes.

    public abstract class ColorConnectionSO : ConnectionSO, IConnectionSO<IConnection<Color>>
    {
        public abstract IConnection<Color> GetConnection();

        public override SettingData.DataType GetDataType()
        {
            return SettingData.DataType.Color;
        }
    }
}
