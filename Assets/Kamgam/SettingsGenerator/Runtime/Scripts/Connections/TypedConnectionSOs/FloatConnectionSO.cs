namespace Kamgam.SettingsGenerator
{
    // This base class exists primarily to trick the UnityEditor into
    // filtering the selection options. But it also is a nice way to
    // shorten the implementation classes.

    // This class is also used to identify the data type in the generator window.

    public abstract class FloatConnectionSO : ConnectionSO, IConnectionSO<IConnection<float>>
    {
        public abstract IConnection<float> GetConnection();

        public override SettingData.DataType GetDataType()
        {
            return SettingData.DataType.Float;
        }
    }
}
