using Kamgam.LocalizationForSettings;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// The purpose of a SettingResolver is to connect a UI with a setting.<br />
    /// It's supposed to be used as a component which you add to your UI GameObject.<br />
    /// You need to extend this class with a concrete implementation.
    /// </summary>
    public abstract class SettingResolver : MonoBehaviour, ISettingResolver
    {
        [Tooltip("The global settings provider asset.")]
        public SettingsProvider SettingsProvider;

        [Tooltip("The global localization provider asset.")]
        public LocalizationProvider LocalizationProvider;

        [Tooltip("The ID of the setting within the Settings asset.")]
        public string ID;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string GetID()
        {
            return ID;
        }

        public abstract SettingData.DataType[] GetSupportedDataTypes();

        public virtual void Start()
        {
            RegisterAsActivated();
        }

        public virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(ID))
            {
                Logger.LogWarning("Resolver '" + this.name + "' has no ID (type: " + this.GetType().Name + ").", this);
            }
            else if (!HasActiveSettingForID(ID))
            {
                Logger.LogWarning("Resolver '" + this.name + "' uses a setting ID ('" + ID + "') that is DISABLED. It will do nothing. Please activate the setting in the settings list.", this);
            }
#endif
            Refresh();
        }

        public virtual void OnDisable() { }

        public virtual void OnDestroy()
        {
            if (SettingsProvider != null && SettingsProvider.HasSettings())
            {
                SettingsProvider.Settings.UnregisterResolver(this);

                var setting = SettingsProvider.Settings.GetSetting(ID);
                if (setting != null)
                {
                    setting.RemovePulledFromConnectionListener(Refresh);
                }
            }
        }

        /// <summary>
        /// Checks whether or not the setting with the given ID exists and if the setting matches one of the
        /// given types. If no types are given then the check is omitted.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="allowedTypes"></param>
        /// <returns></returns>
        public bool HasValidSettingForID(string id, params SettingData.DataType[] allowedTypes)
        {
            if (SettingsProvider == null || SettingsProvider.Settings == null)
            {
                Logger.LogError("SGSettingResolver: Settings or SettingsProvider is NULL (on Object: '" + this.gameObject.name+ "', ID: '" + id + "').", this);
                return false;
            }

            if (string.IsNullOrEmpty(id))
            {
#if UNITY_EDITOR
                Logger.LogWarning("SGSettingResolver: The resolver on Object: '" + gameObject.name + "' has no settings ID set. It will do nothing.", this);
#endif
                return false;
            }

            bool hasID = SettingsProvider.Settings.HasID(id);

            // Show a warning to make finding typos easier.
            if(!hasID)
            {
                Logger.LogWarning("SGSettingResolver: No setting with ID '" + id + "' found in '" + this.name + "'. This setting will NOT be saved!", this);
            }

            if (allowedTypes != null && allowedTypes.Length > 0)
            {
                var setting = SettingsProvider.Settings.GetSetting(id);
                if (setting != null && !setting.MatchesAnyDataType(allowedTypes))
                {
                    // Show detailed logs in editor only.
#if UNITY_EDITOR
                    string types = "";
                    foreach (var type in allowedTypes)
                    {
                        types += type.ToString();
                    }
                    Logger.LogError("SGSettingResolver: Type mismatch for ID '" + id + "'. Got '" + setting.GetType().ToString() + "' but can only handle types: " + types + ".", this);
#endif
                    return false;
                }          
            }

            return hasID;
        }

        public bool HasSettingForID(string id)
        {
            return SettingsProvider.Settings.HasID(id);
        }

        /// <summary>
        /// Checks whether or not the setting with the given ID exists and is active.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasActiveSettingForID(string id)
        {
            return SettingsProvider.Settings.HasActiveID(id);
        }

        /// <summary>
        /// Returns the data type of the setting (setting is searched for by ID).
        /// </summary>
        /// <returns></returns>
        public SettingData.DataType GetDataType()
        {
            if (SettingsProvider == null || SettingsProvider.Settings == null)
                return SettingData.DataType.Unknown;

            var setting = SettingsProvider.Settings.GetSetting(ID);
            if (setting == null)
                return SettingData.DataType.Unknown;

            return setting.GetDataType();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void RegisterAsActivated()
        {
            if (SettingsProvider == null || SettingsProvider.Settings == null)
                return;

            SettingsProvider.Settings.RegisterResolver(GetComponent<ISettingResolver>());
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Unregister()
        {
            if (SettingsProvider == null || SettingsProvider.Settings == null)
                return;

            SettingsProvider.Settings.UnregisterResolver(GetComponent<ISettingResolver>());
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract void Refresh();

#if UNITY_EDITOR
        public virtual void OnValidate()
        {
            if (!string.IsNullOrEmpty(ID))
            {
                ID = ID.Trim();
            }
        }
#endif
    }
}
