using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Boolean setting. This represents a value that is either true or false.<br />
    /// Use this for checkboxes or toggles. Basically anything that can be either on or off.
    /// </summary>
    [System.Serializable]
    public class SettingBool : SettingWithValue<bool>, ISettingWithConnection<bool>
    {
        public const SettingData.DataType DataType = SettingData.DataType.Bool;

        public event System.Action<SettingBool> OnSettingBoolChanged;
        public event System.Action<bool> OnValueChanged;

        [System.NonSerialized]
        public IConnection<bool> Connection;

        [SerializeField]
        BoolConnectionSO ConnectionObject;

        public override ConnectionSO GetConnectionSO()
        {
            return ConnectionObject;
        }

        public override void SetConnectionSO(ConnectionSO connectionSO)
        {
            ConnectionObject = connectionSO as BoolConnectionSO;
        }

        [System.NonSerialized]
        protected bool _value;

        public override bool GetValue()
        {
            return _value;
        }

        protected bool _valueInitialized = false;

        public override void SetValue(bool value, bool propagateChange = true)
        {
            if (_value == value && _valueInitialized)
                return;
            _valueInitialized = true;

            _value = value;

            if (propagateChange)
            {
                OnChanged();

                if (ApplyImmediately)
                    PushToConnection();
            }
        }

        public SettingBool(SettingData data, List<string> groups = null) : base(data, groups) { }

        public SettingBool(string path, bool value, List<string> groups = null) : base(path, groups)
        {
            SetValue(value);
        }

        protected override void triggerOnSettingChanged()
        {
            base.triggerOnSettingChanged();

            OnSettingBoolChanged?.Invoke(this);
            OnValueChanged?.Invoke(GetValue());
        }

        public override void ResetToDefault()
        {
            SetValue(_defaultValue);

            if (HasConnection() && ApplyImmediately)
                PushToConnection();
        }

        public override object GetValueAsObject()
        {
            return GetValue();
        }

        public override void SetValueFromObject(object value, bool propagateChange = true)
        {
            SetValue((bool)value, propagateChange);
        }

        public override SettingData.DataType GetDataType()
        {
            return DataType;
        }

        public override SettingData SerializeValueToData()
        {
            var data = new SettingData(ID, SettingData.DataType.Bool);
            
            // serialize into primitives
            data.IntValues = new int[] { GetValue() ? 1 : 0 };

            return data;
        }

        public override void DeserializeValueFromData(SettingData data)
        {
            if (!checkDataType(data.Type, DataType))
                return;

            // deserialize from primitives
            SetValue(data.IntValues[0] == 1 ? true : false, propagateChange: false);
        }

        protected void extractConnectionFromObject()
        {
            if (Connection == null && ConnectionObject != null)
            {
                if (ConnectionObject is IConnectionSO<IConnection<bool>> typedObject)
                {
                    Connection = typedObject.GetConnection();
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
                SetValue(Connection.Get(), propagateChange: false);
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

        public void SetConnection(IConnection<bool> connection)
        {
            Connection = connection;

            if (connection != null)
            {
                InitializeConnection(); 
            }
        }

        public IConnection<bool> GetConnection()
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
