using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [System.Serializable]
    public class SettingColor : SettingWithValue<Color>, ISettingWithConnection<Color>
    {
        public const SettingData.DataType DataType = SettingData.DataType.Color;

        public event System.Action<SettingColor> OnSettingColorChanged;
        public event System.Action<Color> OnValueChanged;

        [System.NonSerialized]
        public IConnection<Color> Connection;

        [SerializeField]
        ColorConnectionSO ConnectionObject;

        public override ConnectionSO GetConnectionSO()
        {
            return ConnectionObject;
        }

        public override void SetConnectionSO(ConnectionSO connectionSO)
        {
            ConnectionObject = connectionSO as ColorConnectionSO;
        }

        [System.NonSerialized]
        protected Color _color;

        public override Color GetValue()
        {
            return _color;
        }

        protected bool _valueInitialized = false;

        public override void SetValue(Color color, bool propagateChange = true)
        {
            if (_color == color && _valueInitialized)
                return;
            _valueInitialized = true;

            _color = color;

            if (propagateChange)
            {
                OnChanged();

                if (ApplyImmediately)
                    PushToConnection();
            }
        }

        public SettingColor(SettingData data, List<string> groups = null) : base(data, groups) { }

        public SettingColor(string path, Color value, List<string> groups = null) : base(path, groups)
        {
            SetValue(value);
        }

        protected override void triggerOnSettingChanged()
        {
            base.triggerOnSettingChanged();

            OnSettingColorChanged?.Invoke(this);
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
            SetValue((Color)value, propagateChange);
        }

        public override SettingData.DataType GetDataType()
        {
            return DataType;
        }

        public override SettingData SerializeValueToData()
        {
            var data = new SettingData(ID, DataType);

            // serialize into primitives
            var color = GetValue();
            data.FloatValues = new float[] { color.r, color.g, color.b, color.a };

            return data;
        }

        public override void DeserializeValueFromData(SettingData data)
        {
            if (!checkDataType(data.Type, DataType))
                return;

            // deserialize from primitives
            SetValue(
                new Color(data.FloatValues[0], data.FloatValues[1], data.FloatValues[2], data.FloatValues[3])
                , propagateChange: false);
        }

        protected void extractConnectionFromObject()
        {
            if (Connection == null && ConnectionObject != null)
            {
                if (ConnectionObject is IConnectionSO<IConnection<Color>> typedObject)
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

        public void SetConnection(IConnection<Color> connection)
        {
            Connection = connection;

            if (connection != null)
            {
                InitializeConnection();
            }
        }

        public IConnection<Color> GetConnection()
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
