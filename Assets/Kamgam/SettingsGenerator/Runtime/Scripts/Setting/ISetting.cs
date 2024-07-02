using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Base interface for every setting.
    /// </summary>
    public interface ISetting : ISerializationCallbackReceiver, IQualityChangeReceiver, ISettingWithConnectionSO
    {
        event System.Action<ISetting> OnSettingChanged;

        bool HasUserData();
        void SetHasUserData(bool hasUserData);

        string GetID();
        bool MatchesID(string path);

        bool IsActive { get; set; }

        SettingData.DataType GetDataType();
        bool MatchesAnyDataType(IList<SettingData.DataType> dataTypes);

        List<string> GetGroups();
        void SetGroups(List<string> groups);
        bool MatchesAnyGroup(string[] groups);

        object GetValueAsObject();
        void SetValueFromObject(object value, bool propagateChange = true);
        void ResetToDefault();

        SettingData SerializeValueToData();
        void DeserializeValueFromData(SettingData data);

        /// <summary>
        /// Called if the value of the settings has changed.
        /// </summary>
        void OnChanged();

        void AddPulledFromConnectionListener(System.Action onPulled);
        void RemovePulledFromConnectionListener(System.Action onPulled);

        /// <summary>
        /// Pushes the value to the connection. Then pulls it.<br />
        /// Clears the changed flag.<br />
        /// Informs all applyListeners of the change.<br />
        /// </summary>
        void Apply();

        /// <summary>
        /// Returns whether or not this setting has some unapplied changes.<br />
        /// Freshly loaded settings should be marked as "changed".
        /// </summary>
        /// <returns></returns>
        bool HasUnappliedChanges();

        /// <summary>
        /// Marks the setting as changed.
        /// </summary>
        void MarkAsChanged();
        void MarkAsUnchanged();

        /// <summary>
        /// Fetches the default value from the connection. If no user data (saved setting)
        /// was loaded then this will also update the settings current value to the fetched default.
        /// <br /><br />
        /// NOTICE: This does NOT use PullConnection() and therefore does not trigger any listeners.
        /// </summary>
        void InitializeConnection();
        bool HasConnection();
        bool HasConnectionObject();
        int GetConnectionOrder();
        void PullFromConnection();
        void PullFromConnection(bool propagateChange);
        void PushToConnection();
        IConnection GetConnectionInterface();
    }
}
