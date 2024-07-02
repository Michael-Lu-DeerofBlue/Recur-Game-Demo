using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [System.Serializable]
    public class SettingOption : SettingWithValue<int>, ISettingWithOptions<string>
    {
        public const SettingData.DataType DataType = SettingData.DataType.Option;

        public event System.Action<SettingOption> OnSettingOptionChanged;
        public event System.Action<int> OnValueChanged;

        [SerializeField]
        [Tooltip("The names of the options.\n" +
            "These define which options the user can choose from. The resulting value will the index of the selected option label (index is starting with 0).\n" +
            "These are IGNORED if a connection is set.")]
        protected List<string> _optionLabels;

        [SerializeField]
        [Tooltip("Should the option labels be taken from the static ones defined here even if a connection is set?" +
            "\n\nUsually if a connection is set then the labels are fetched dynamically from the connection.\n" +
            "Be aware that if you set the labels here then the number of labels has to match the connection options. Otherwise this will have no effect.")]
        [DisableIf("ConnectionObject", null)]
        protected bool _overrideConnectionLabels = false;

        [System.NonSerialized]
        public IConnectionWithOptions<string> Connection;

        [SerializeField]
        [Tooltip("The connection which is used to dynamically fill the option names and value.\n" +
            "If this is set then the OptionLabels and DefaultIndex fields are ignored.\nYou can override this by enabling 'overrideConnectionLabels'.")]
        OptionConnectionSO ConnectionObject;

        public override ConnectionSO GetConnectionSO()
        {
            return ConnectionObject;
        }

        public override void SetConnectionSO(ConnectionSO connectionSO)
        {
            ConnectionObject = connectionSO as OptionConnectionSO;
        }

        [System.NonSerialized]
        protected int _selectedIndex;

        public override int GetValue()
        {
            return _selectedIndex;
        }

        protected bool _valueInitialized = false;

        public override void SetValue(int selectedIndex, bool propagateChange = true)
        {
            if (_selectedIndex == selectedIndex && _valueInitialized)
                return;
            _valueInitialized = true;

            _selectedIndex = selectedIndex;

            if (propagateChange)
            {
                OnChanged();

                if (ApplyImmediately)
                {
                    PushToConnection();
                }
            }
        }

        public SettingOption(SettingData data, List<string> groups = null, List<string> optionNames = null) : base(data, groups)
        {
            _optionLabels = optionNames;
        }

        public SettingOption(string path, int selectedIndex, List<string> groups = null, List<string> optionNames = null) : base(path, groups)
        {
            SetValue(selectedIndex);
            _optionLabels = optionNames;
        }

        protected override void triggerOnSettingChanged()
        {
            base.triggerOnSettingChanged();

            OnSettingOptionChanged?.Invoke(this);
            OnValueChanged?.Invoke(GetValue());
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
        }

        public void SetOverrideConnectionLabels(bool overrideLabels)
        {
            _overrideConnectionLabels = overrideLabels;
            if (HasConnection())
            {
                if (_overrideConnectionLabels)
                {
                    Connection.SetOptionLabels(_optionLabels);
                }
                else
                {
                    Connection.RefreshOptionLabels();
                    _optionLabels = Connection.GetOptionLabels();
                }
                invokePulledFromConnectionListeners();
            }
        }

        protected void initLabelOverridesAfterNewConnection()
        {
            if (Connection != null)
            {
                if (_overrideConnectionLabels)
                {
#if UNITY_EDITOR
                    if (UnityEditor.EditorApplication.isPlaying)
                    {
#endif
                        Connection.SetOptionLabels(_optionLabels);
#if UNITY_EDITOR
                    }
#endif
                }
            }
        }

        public bool GetOverrideConnectionLabels()
        {
            return _overrideConnectionLabels;
        }

        /// <summary>
        /// Resets the value to default.<br />
        /// Pushes the value to the connection (if there is one).
        /// </summary>
        public override void ResetToDefault()
        {
            SetValue(_defaultValue);

            if (HasConnection() && ApplyImmediately)
                PushToConnection();
        }

        /// <summary>
        /// Returns whether or not option are filled.<br />
        /// If the options are driven by a connection then the options form the connection are checked.
        /// </summary>
        public bool HasOptions()
        {
            if (HasConnection())
            {
                return Connection.HasOptions();
            }
            else
            {
                return _optionLabels != null && _optionLabels.Count > 0;
            }
        }

        /// <summary>
        /// Returns the option labels.<br />
        /// If the options are driven by a connection then the options form the connection are returned.
        /// </summary>
        public List<string> GetOptionLabels()
        {
            if (HasConnection())
            {
                return Connection.GetOptionLabels();
            }
            else
            {
                return _optionLabels;
            }
        }

        /// <summary>
        /// Sets the option labels.<br />
        /// NOTICE: If the options are driven by a connection then these changes will be overwritten. Use the SetOptionLabels() method on the connection instead.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="localize">Should the options be sent through the localizationCallback?</param>
        public void SetOptionLabels(List<string> options)
        {
            if(options != _optionLabels) // Don't clear the new options themselves.
                ClearOptionLabels();

            if (options != null)
            {
                AddOptionLabels(options);
                SetOverrideConnectionLabels(_overrideConnectionLabels);
            }
        }

        /// <summary>
        /// Clears option labels.<br />
        /// NOTICE: If the options are driven by a connection then these changes will be overwritten.
        /// </summary>
        public void ClearOptionLabels()
        {
            if(_optionLabels != null)
                _optionLabels.Clear();
        }

        /// <summary>
        /// Adds option labels.<br />
        /// NOTICE: If the options are driven by a connection then these changes will be overwritten.
        /// </summary>
        /// <param name="optionsToAdd"></param>
        /// <param name="localize">Should the options be sent through the localizationCallback?</param>
        public void AddOptionLabels(IEnumerable<string> optionsToAdd)
        {
            if (_optionLabels == null)
                _optionLabels = new List<string>();

            _optionLabels.AddRange(optionsToAdd);
        }

        public override void SetValueFromObject(object value, bool propagateChange = true)
        {
            SetValue((int)value, propagateChange);
        }

        public override object GetValueAsObject()
        {
            return GetValue();
        }

        public override SettingData.DataType GetDataType()
        {
            return DataType;
        }

        public override SettingData SerializeValueToData()
        {
            var data = new SettingData(ID, DataType);

            // serialize into primitives
            data.IntValues = new int[] { GetValue() };

            return data;
        }

        public override void DeserializeValueFromData(SettingData data)
        {
            if (!checkDataType(data.Type, DataType))
                return;

            // deserialize from primitives
            SetValue(data.IntValues[0], propagateChange: false);
        }

        protected void extractConnectionFromObject()
        {
            if (Connection == null && ConnectionObject != null)
            {
                if (ConnectionObject is IConnectionSO<IConnectionWithOptions<string>> typedObject)
                {
                    Connection = typedObject.GetConnection();
                    if(Connection != null)
                        initLabelOverridesAfterNewConnection();
                }
            }
        }

        public override bool HasConnection()
        {
            extractConnectionFromObject();
            return Connection != null;
        }

        public override bool HasConnectionObject()
        {
            return ConnectionObject != null;
        }

        public override void PullFromConnection()
        {
            PullFromConnection(propagateChange: false);
        }

        public override void PullFromConnection(bool propagateChange)
        {
            if (HasConnection())
            {
                ClearOptionLabels();
                var options = Connection.GetOptionLabels();
                if (options != null)
                {
                    AddOptionLabels(options);
                }

                SetValue(Connection.Get(), propagateChange);
                invokePulledFromConnectionListeners();
            }
        }

        public override void PushToConnection()
        {
            base.PushToConnection();

            if (HasConnection())
                Connection.Set(GetValue());
        }

        public override int GetConnectionOrder()
        {
            if (HasConnection())
                return Connection.GetOrder();
            else
                return ConnectionDefaults.Order;
        }

        public void SetConnection(IConnectionWithOptions<string> connection)
        {
            Connection = connection;

            if (connection != null)
            {
                InitializeConnection();
                initLabelOverridesAfterNewConnection();
            }
        }

        public void SetConnection(IConnection<int> connection)
        {
            throw new System.Exception("IConnectionWithOptions<string> required but IConnection<int> given.");
        }

        public IConnection<int> GetConnection()
        {
            return Connection;
        }

        public override IConnection GetConnectionInterface()
        {
            return Connection;
        }

        public override void OnQualityChanged(int qualityLevel)
        {
            if (HasConnection())
            {
                Connection.OnQualityChanged(qualityLevel);
            }
        }
    }
}
