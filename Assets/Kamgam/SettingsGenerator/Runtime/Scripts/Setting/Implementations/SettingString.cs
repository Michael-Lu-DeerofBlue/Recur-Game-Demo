using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [System.Serializable]
    public class SettingString : SettingWithValue<string>, ISettingWithConnection<string>
    {
        public const SettingData.DataType DataType = SettingData.DataType.String;

        public event System.Action<SettingString> OnSettingStringChanged;
        public event System.Action<string> OnValueChanged;

        [System.NonSerialized]
        public IConnection<string> Connection;

        [SerializeField]
        StringConnectionSO ConnectionObject;

        public override ConnectionSO GetConnectionSO()
        {
            return ConnectionObject;
        }

        public override void SetConnectionSO(ConnectionSO connectionSO)
        {
            ConnectionObject = connectionSO as StringConnectionSO;
        }

        [System.NonSerialized]
        protected string _value;

        public override string GetValue()
        {
            return _value;
        }

        protected bool _valueInitialized;

        public override void SetValue(string value, bool propagateChange = true)
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


        public SettingString(SettingData data, List<string> groups = null) : base(data, groups) { }

        public SettingString(string path, string value, List<string> groups = null) : base(path, groups)
        {
            SetValue(value);
        }

        protected override void triggerOnSettingChanged()
        {
            base.triggerOnSettingChanged();

            OnSettingStringChanged?.Invoke(this);
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
            SetValue((string)value, propagateChange);
        }

        public override SettingData.DataType GetDataType()
        {
            return DataType;
        }

        public override SettingData SerializeValueToData()
        {
            var data = new SettingData(ID, DataType);

            // serialize into primitives
            data.StringValues = new string[] { GetValue() };

            return data;
        }

        public override void DeserializeValueFromData(SettingData data)
        {
            if (!checkDataType(data.Type, DataType))
                return;

            if (data.StringValues[0].Length > 65000)
            {
                Debug.LogError("SG SettingString: String is too long and will be truncated.");
                data.StringValues[0] = data.StringValues[0].Substring(0, 65000);
            }

            // deserialize from primitives
            SetValue(data.StringValues[0], propagateChange: false);
        }

        protected void extractConnectionFromObject()
        {
            if (Connection == null && ConnectionObject != null)
            {
                if (ConnectionObject is IConnectionSO<IConnection<string>> typedObject)
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


        public void SetConnection(IConnection<string> connection)
        {
            Connection = connection;

            if (connection != null)
            {
                InitializeConnection();
            }
        }

        public IConnection<string> GetConnection()
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
