namespace Kamgam.SettingsGenerator
{
    public interface ISettingResolver
    {
        /// <summary>
        /// Updates the target with the value from the setting. Usually that target is a UI.<br />
        /// It must NOT trigger a ValueChanged event which propagates back to the setting.
        /// This is important to avoid unintended infinite loops.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Returns the ID of the resolver.<br />
        /// This has to match one (and only one) setting ID.
        /// </summary>
        /// <returns></returns>
        string GetID();

        /// <summary>
        /// Registers this resolver in the settings<br />
        /// Whenever a resolver gets activated (set to be visible for the first time).
        /// It should register itself to the current settings object.<br />
        /// This is done so that the settings know which resolvers to notify if the
        /// data changes (if multiple settings are changed due to a quality change for example).
        /// </summary>
        void RegisterAsActivated();

        /// <summary>
        /// Unregisters this resolver from the settings<br />
        /// Whenever a resolver get removed permanently then it should also unregister from
        /// the settings. Though this is not strictly necessary as the settings do skip null
        /// resolvers automatically. Still, it's good practice to use it in order to keep
        /// the null values in the resolver list at a minimun.
        /// </summary>
        void Unregister();

        /// <summary>
        /// The list of supported data types.<br />
        /// A Resolver may support multipe data types (float and int for example).
        /// </summary>
        /// <returns></returns>
        SettingData.DataType[] GetSupportedDataTypes();
    }
}
