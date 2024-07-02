namespace Kamgam.SettingsGenerator
{
    public interface IConnection : IQualityChangeReceiver
    {
        int GetOrder();
        void SetOrder(int order);
    }

    public interface IConnection<TValue> : IConnection
    {
        TValue Get();

        /// <summary>
        /// Returns the default value. In most cases this is just
        /// an alias for calling Get().
        /// <br /><br />
        /// You can override this in your connectoin if the default
        /// should differ from what the normal Get() method would return.
        /// <br /><br />
        /// This is called BEFORE any call to Get() is made. It is based
        /// on the assumption that before the settings system starts no
        /// values are set. Therefore whichever value is returned first
        /// must be the default.
        /// <br /> <br />
        /// NOTICE: This is only executed once to initialize the connection.
        /// Each Setting has a default value too which will override this
        /// if set to a different value, see SettingsWithValue.SetDefault(TValue defaultValue).
        /// </summary>
        /// <returns></returns>
        TValue GetDefault();

        void Set(TValue value);

        void AddChangeListener(System.Action<TValue> listener);
        void RemoveChangeListener(System.Action<TValue> listener);

        void Destroy();
    }
}
