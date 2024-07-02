namespace Kamgam.SettingsGenerator
{
    public interface ISettingWithConnection<TValue> : ISettingWithValue<TValue>
    {
        void SetConnection(IConnection<TValue> connection);
        IConnection<TValue> GetConnection();
    }
}