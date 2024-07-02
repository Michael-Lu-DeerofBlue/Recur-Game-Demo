using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [System.Serializable]
    public class SettingColorOption : SettingWithValue<int>, ISettingWithOptions<Color>
    {
        public const SettingData.DataType DataType = SettingData.DataType.ColorOption;

        public event System.Action<SettingColorOption> OnSettingColorOptionChanged;
        public event System.Action<int> OnValueChanged;

        [SerializeField]
        [Tooltip("The color options.\n" +
            "These define which colors the user can choose from. The resulting value will the selected color.\n" +
            "These are IGNORED if a connection is set.")]
        protected List<Color> _options;

        [SerializeField]
        [Tooltip("Should the option labels be taken from the static ones defined here even if a connection is set?" +
            "\n\nUsually if a connection is set then the labels are fetched dynamically from the connection.\n" +
            "Be aware that if you set the labels here then the number of labels has to match the connection options. Otherwise this will have no effect.")]
        [DisableIf("ConnectionObject", null)]
        protected bool _overrideConnectionLabels = false;

        [System.NonSerialized]
        public IConnectionWithOptions<Color> Connection;

        [SerializeField]
        ColorOptionConnectionSO ConnectionObject;

        public override ConnectionSO GetConnectionSO()
        {
            return ConnectionObject;
        }

        public override void SetConnectionSO(ConnectionSO connectionSO)
        {
            ConnectionObject = connectionSO as ColorOptionConnectionSO;
        }

        [System.NonSerialized]
        protected int _selectedIndex;

        public override int GetValue()
        {
            return _selectedIndex;
        }

        public Color GetColorValue()
        {
            return GetColorValue(Color.black);
        }

        public Color GetColorValue(Color defaultColor)
        {
            var index = GetValue();
            var colors = GetOptionLabels();
            if (index < 0 || colors == null || index >= colors.Count)
                return defaultColor;
            else
                return colors[index];
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
                    PushToConnection();
            }
        }

        public SettingColorOption(SettingData data, List<string> groups = null, List<Color> options = null) : base(data, groups)
        {
            _options = options;
        }

        public SettingColorOption(string path, int selectedIndex, List<string> groups = null, List<Color> options = null) : base(path, groups)
        {
            SetValue(selectedIndex);
            _options = options;
        }


        protected override void triggerOnSettingChanged()
        {
            base.triggerOnSettingChanged();

            OnSettingColorOptionChanged?.Invoke(this);
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
                    Connection.SetOptionLabels(_options);
                }
                else
                {
                    Connection.RefreshOptionLabels();
                    _options = Connection.GetOptionLabels();
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
                        Connection.SetOptionLabels(_options);
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

        public override void ResetToDefault()
        {
            SetValue(_defaultValue);

            if (HasConnection() && ApplyImmediately)
                PushToConnection();
        }

        public void SetOptionLabels(List<Color> options)
        {
            ClearOptions();
            if (options != null)
            {
                AddOptions(options);
                SetOverrideConnectionLabels(_overrideConnectionLabels);
            }
        }

        public bool HasOptions()
        {
            if (HasConnection())
            {
                return Connection.HasOptions();
            }
            else
            {
                return _options != null && _options.Count > 0;
            }
        }

        public List<Color> GetOptionLabels()
        {
            if (HasConnection())
            {
                return Connection.GetOptionLabels();
            }
            else
            {
                return _options;
            }
        }

        public void ClearOptions()
        {
            if (_options != null)
                _options.Clear();
        }

        public void AddOptions(IEnumerable<Color> optionsToAdd)
        {
            if (_options == null)
                _options = new List<Color>();

            _options.AddRange(optionsToAdd);
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
                if (ConnectionObject is IConnectionSO<IConnectionWithOptions<Color>> typedObject)
                {
                    Connection = typedObject.GetConnection();
                    if (Connection != null)
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
                ClearOptions();
                var options = Connection.GetOptionLabels();
                if (options != null)
                {
                    AddOptions(options);
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

        /// <summary>
        /// Sets the connection and updates the default value for reset.
        /// </summary>
        /// <param name="connection"></param>
        public void SetConnection(IConnectionWithOptions<Color> connection)
        {
            Connection = connection;

            // Reset the default value if the connection was changed.
            if (connection != null)
            {
                InitializeConnection();
                initLabelOverridesAfterNewConnection();
            }
        }

        public IConnection<int> GetConnection()
        {
            return Connection;
        }

        public override IConnection GetConnectionInterface()
        {
            return Connection;
        }

        public void SetConnection(IConnection<int> connection)
        {
            throw new System.Exception("IConnectionWithOptions<Color> required but IConnection<int> given.");
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
