namespace Kamgam.SettingsGenerator
{
    public interface ISettingWithConnectionSO
    {
        ConnectionSO GetConnectionSO();
        void SetConnectionSO(ConnectionSO connectionSO);

        SettingData.DataType GetConnectionSettingDataType();
    }
}