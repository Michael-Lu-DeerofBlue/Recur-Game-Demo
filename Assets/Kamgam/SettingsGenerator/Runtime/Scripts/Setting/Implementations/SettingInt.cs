using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [System.Serializable]
    public class SettingInt : SettingWithValue<int>, ISettingWithConnection<int>
    {
        public const SettingData.DataType DataType = SettingData.DataType.Int;

        public event System.Action<SettingInt> OnSettingIntChanged;
        public event System.Action<int> OnValueChanged;

        [System.NonSerialized]
        public IConnection<int> Connection;

        [SerializeField]
        IntConnectionSO ConnectionObject;

        public override ConnectionSO GetConnectionSO()
        {
            return ConnectionObject;
        }

        public override void SetConnectionSO(ConnectionSO connectionSO)
        {
            ConnectionObject = connectionSO as IntConnectionSO;
        }

        [System.NonSerialized]
        protected int _value;

        public override int GetValue()
        {
            return _value;
        }

        protected bool _valueInitialized;

        public override void SetValue(int value, bool propagateChange = true)
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

        public SettingInt(SettingData data, List<string> groups = null) : base(data, groups) { }

        public SettingInt(string path, int value, List<string> groups = null) : base(path, groups)
        {
            SetValue(value);
        }

        protected override void triggerOnSettingChanged()
        {
            base.triggerOnSettingChanged();

            OnSettingIntChanged?.Invoke(this);
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
            SetValue((int)value, propagateChange);
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
                if (ConnectionObject is IConnectionSO<IConnection<int>> typedObject)
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

        public void SetConnection(IConnection<int> connection)
        {
            Connection = connection;

            if (connection != null)
            {
                InitializeConnection();
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

        public override void OnQualityChanged(int qualityLevel)
        {
            if (HasConnection())
            {
                Connection.OnQualityChanged(qualityLevel);
            }
        }
    }
}
