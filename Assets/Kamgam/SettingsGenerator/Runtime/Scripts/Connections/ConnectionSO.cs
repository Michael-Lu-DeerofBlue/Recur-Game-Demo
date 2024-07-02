using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    // This base class exists to trick the UnityEditor into
    // filtering the selection options.

    // It also handles resetting objects before play mode
    // if Domain-Reload is disabled (via IResetBeforeDomainReload).

    public abstract class ConnectionSO : ScriptableObject
#if UNITY_EDITOR
        , IResetBeforeDomainReload
#endif
    {
        public abstract void DestroyConnection();

        public abstract SettingData.DataType GetDataType();

#if UNITY_EDITOR
        // Domain Reload handling
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void onResetBeforePlayMode()
        {
            DomainReloadUtils.CallOnResetOnAssets(typeof(ConnectionSO));
        }

        public void ResetBeforePlayMode()
        {
            DestroyConnection();
        }
#endif
    }
}
