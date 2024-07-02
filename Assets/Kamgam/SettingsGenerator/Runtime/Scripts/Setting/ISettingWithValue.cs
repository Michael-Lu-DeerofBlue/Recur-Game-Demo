using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    public interface ISettingWithValue<TValue> : ISetting
    {
        TValue GetValue();
        void SetValue(TValue value, bool propagateChange = true);
        void SetDefaultFromConnection(IConnection<TValue> connection);

        void AddChangeListener(System.Action<TValue> onChanged);
        void RemoveChangeListener(System.Action<TValue> onChanged);

        void AddPulledFromConnectionListener(System.Action<TValue> onPulled);
        void RemovePulledFromConnectionListener(System.Action<TValue> onPulled);
    }
}
