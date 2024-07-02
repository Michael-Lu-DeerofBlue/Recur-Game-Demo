namespace Kamgam.SettingsGenerator
{
    // This base class exists primarily to trick the UnityEditor into
    // filtering the selection options. But it also is a nice way to
    // shorten the implementation classes.

    public abstract class StringConnectionSO : ConnectionSO, IConnectionSO<IConnection<string>>
    {
        public abstract IConnection<string> GetConnection();

        public override SettingData.DataType GetDataType()
        {
            return SettingData.DataType.String;
        }
    }
}
