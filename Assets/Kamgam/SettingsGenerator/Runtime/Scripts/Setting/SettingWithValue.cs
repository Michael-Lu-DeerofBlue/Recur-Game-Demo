using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Base class for every Setting. Settings are serializable objects
    /// within the Settings Scriptable Object.
    /// </summary>
    [System.Serializable]
    public abstract class SettingWithValue<TValue> : ISettingWithValue<TValue>
    {
        /// <summary>
        /// Was this setting filled with some saved and loaded user data?
        /// </summary>
        [System.NonSerialized]
        protected bool _hasUserData = false;

        [Tooltip("If a settings is disabled then the settings system will ignore it.\n" +
            "This is useful if you want to keep a setting in the list but disable it.\n" +
            "You should not change this at runtime.")]
        [SerializeField]
        protected bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isPlaying)
                {
#endif
                    Logger.LogWarning("Changing the IsActive state of settings at runtime is not recommended.");
#if UNITY_EDITOR
                }
#endif
            }
        }

        public string ID;

        /// <summary>
        /// If true then any changes made are immediately sent to the connection.<br />
        /// This means you do not have to call Apply() to "apply" the change.
        /// </summary>
        public bool ApplyImmediately = true;

        public virtual ConnectionSO GetConnectionSO()
        {
            return null;
        }
        
        public virtual void SetConnectionSO(ConnectionSO connectionSO) {}

        public virtual SettingData.DataType GetConnectionSettingDataType()
        {
            return SettingData.DataType.Unknown;
        }

        /// <summary>
        /// Settings can be part of groups. This makes it easier to reset only some of them.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("Groups")]
        protected List<string> _groups;

        [SerializeField]
        [FormerlySerializedAs("_value")]
        [DisableIf("ConnectionObject", null, invertBehaviour: true)]
        [Tooltip("The default value which will be used if no user setting data was found (first boot) and if no connection is set." +
            "\n\nNOTICE: If a connection is set then this will be ignored as the value will come from the connection.")]
        protected TValue _defaultValue;

        protected bool _hasChanged = false;
        protected System.Func<string, string> _translateFunc;

        protected List<Action<TValue>> _applyListeners;
        protected List<Action<TValue>> _changeListeners;
        protected List<Action<TValue>> _pulledFromConnectionListeners;
        protected List<Action> _genericPulledFromConnectionListeners;

        public event Action<ISetting> OnSettingChanged;

        public SettingWithValue(SettingData data, List<string> groups)
        {
            // Be aware that these contructors are only called if 
            // a new setting is created via script!
            //
            // Usually it will be part of the Settings SO and
            // thus only the default constructor is used.
            // If saved user settings are loaded then only
            // DeserializeValueFromData() is called.

            ID = data.ID;
            DeserializeValueFromData(data);
            _groups = groups;
            ApplyImmediately = true;
        }

        public SettingWithValue(string id, List<string> groups)
        {
            // Be aware that these contructors are only caled if 
            // a new setting is created via script (see above)!

            ID = id;
            _groups = groups;
            ApplyImmediately = true;
        }

        public virtual void OnBeforeSerialize(){}

        public virtual void OnAfterDeserialize()
        {
            ID = ID.Trim();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void InitializeConnection()
        {
            if (HasConnection())
            {
                // Pull the default value from the connection.
                
                // If there already is some loaded user data then
                // do NOT override the current value with the value
                // from the connection but keep the user data instead.
                if (_hasUserData)
                {
                    // Update _defaultValue
                    SetDefaultFromConnection((IConnection<TValue>)GetConnectionInterface());
                    // The _value was loaded from saved settings so we leave it alone.
                }
                else
                {
                    // Update _defaultValue
                    SetDefaultFromConnection((IConnection<TValue>)GetConnectionInterface());
                    // Update _value
                    SetValue(_defaultValue);
                }
            }
        }

        public string GetID()
        {
            return ID;
        }

        public void SetHasUserData(bool loaded)
        {
            _hasUserData = loaded;
        }

        public bool HasUserData()
        {
            return _hasUserData;
        }

        public abstract SettingData.DataType GetDataType();

        public abstract TValue GetValue();
        public abstract void SetValue(TValue value, bool propagateChange = true);

        public abstract void SetValueFromObject(object value, bool propagateChange = true);

        public bool MatchesID(string id)
        {
            if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(id))
                return false;

            return ID == id;
        }

        public virtual void SetDefault(TValue defaultValue)
        {
            _defaultValue = defaultValue;
        }

        public virtual void SetDefaultFromConnection(IConnection<TValue> connection)
        {
            if (HasConnection())
            {
                _defaultValue = connection.GetDefault();
            }
        }

        public abstract void ResetToDefault();

        public bool MatchesAnyGroup(string[] groups)
        {
            if (groups == null || groups.Length == 0 || _groups == null || _groups.Count == 0)
                return false;

            foreach (var tag in groups)
            {
                foreach (var myTag in _groups)
                {
                    if (myTag == tag)
                        return true;
                }
            }
            return false;
        }

        public List<string> GetGroups()
        {
            return _groups;
        }

        public void SetGroups(List<string> groups)
        {
            _groups = groups;
        }

        protected bool checkDataType(SettingData.DataType serializedDataType, SettingData.DataType dataType)
        {
            if (serializedDataType != dataType)
            {
                Debug.LogError("SGSettings: The serialized data type is '" + serializedDataType + "' instead of the expected '" + dataType + "' for settings path '" + ID + "'.");
                return false;
            }

            return true;
        }

        public bool MatchesAnyDataType(IList<SettingData.DataType> dataTypes)
        {
            if (dataTypes == null)
                return false;

            int count = dataTypes.Count;
            for (int i = 0; i < count; i++)
            {
                if (dataTypes[i] == GetDataType())
                    return true;
            }
            return false;
        }

        public abstract SettingData SerializeValueToData();
        public abstract void DeserializeValueFromData(SettingData data);

        public void AddChangeListener(Action<TValue> onChanged)
        {
            if (_changeListeners == null)
                _changeListeners = new List<Action<TValue>>();

            if (!_changeListeners.Contains(onChanged))
            {
                _changeListeners.Add(onChanged);
            }
        }

        public void RemoveChangeListener(Action<TValue> onChanged)
        {
            if (_changeListeners == null)
                return;

            _changeListeners.Remove(onChanged);
        }

        public void AddApplyListener(Action<TValue> onApplied)
        {
            if (_applyListeners == null)
                _applyListeners = new List<Action<TValue>>();

            if (!_applyListeners.Contains(onApplied))
            {
                _applyListeners.Add(onApplied);
            }
        }

        public void RemoveApplyListener(Action<TValue> onApplied)
        {
            if (_applyListeners == null)
                return;

            _applyListeners.Remove(onApplied);
        }

        protected void invokeApplyListeners()
        {
            if (_applyListeners != null)
            {
                foreach (var listener in _applyListeners)
                {
                    if (listener != null)
                        listener?.Invoke(GetValue());
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Apply()
        {
            _hasChanged = false;

            if (HasConnection())
            {
                PushToConnection();
                PullFromConnection();
            }

            invokeApplyListeners();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void OnChanged()
        {
            MarkAsChanged();

            triggerOnSettingChanged();

            if (_changeListeners != null)
            {
                foreach (var listener in _changeListeners)
                {
                    listener?.Invoke(GetValue());
                }
            }
        }

        protected virtual void triggerOnSettingChanged()
        {
            OnSettingChanged?.Invoke(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void MarkAsChanged()
        {
            _hasChanged = true;
        }

        public void MarkAsUnchanged()
        {
            _hasChanged = false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool HasUnappliedChanges()
        {
            return _hasChanged;
        }

        public void AddPulledFromConnectionListener(Action<TValue> onApply)
        {
            if (_pulledFromConnectionListeners == null)
                _pulledFromConnectionListeners = new List<Action<TValue>>();

            if (!_pulledFromConnectionListeners.Contains(onApply))
            {
                _pulledFromConnectionListeners.Add(onApply);
            }
        }

        public void RemovePulledFromConnectionListener(Action<TValue> onApply)
        {
            if (_pulledFromConnectionListeners == null)
                return;

            _pulledFromConnectionListeners.Remove(onApply);
        }

        protected void invokePulledFromConnectionListeners()
        {
            if (HasConnection() && _pulledFromConnectionListeners != null)
            {
                foreach (var listener in _pulledFromConnectionListeners)
                {
                    if (listener != null)
                        listener?.Invoke(GetValue());
                }
            }

            invokeGenericPulledFromConnectionListeners();
        }

        public void AddPulledFromConnectionListener(Action onApply)
        {
            if (_genericPulledFromConnectionListeners == null)
                _genericPulledFromConnectionListeners = new List<Action>();

            if (!_genericPulledFromConnectionListeners.Contains(onApply))
            {
                _genericPulledFromConnectionListeners.Add(onApply);
            }
        }

        public void RemovePulledFromConnectionListener(Action onApply)
        {
            if (_genericPulledFromConnectionListeners == null)
                return;

            _genericPulledFromConnectionListeners.Remove(onApply);
        }

        protected void invokeGenericPulledFromConnectionListeners()
        {
            if (HasConnection() && _genericPulledFromConnectionListeners != null)
            {
                foreach (var listener in _genericPulledFromConnectionListeners)
                {
                    if (listener != null)
                        listener?.Invoke();
                }
            }
        }

        public void RemoveAllListeners()
        {
            _changeListeners?.Clear();
            _pulledFromConnectionListeners?.Clear();
            _genericPulledFromConnectionListeners?.Clear();
            _applyListeners?.Clear();
        }

        public abstract object GetValueAsObject();
        public abstract bool HasConnection();
        public abstract bool HasConnectionObject();
        public abstract void PullFromConnection();
        public abstract void PullFromConnection(bool propagateChange);
        public abstract IConnection GetConnectionInterface();

        public virtual void PushToConnection()
        {
            _hasChanged = false;
        }

        public abstract int GetConnectionOrder();

        public abstract void OnQualityChanged(int qualityLevel);
    }
}
