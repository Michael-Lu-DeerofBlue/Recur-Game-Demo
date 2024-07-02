#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Kamgam.SettingsGenerator
{
    public interface IResetBeforeDomainReload
    {
        void ResetBeforePlayMode();
    }

    /// <summary>
    /// Add this code to the SO deriving from this class to enable resetting values before play mode.
    /// <example>
    /// #if UNITY_EDITOR
    ///     // Domain Reload handling
    ///     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    ///     protected static void startOnResetBeforePlayMode()
    ///     {
    ///         DomainReloadUtils.CallOnResetBeforePlayMode(typeof(YourDerivedSO));
    ///     }
    /// 
    ///     public void ResetBeforePlayMode()
    ///     {
    ///         // Do something in the instance.
    ///     }
    /// #endif
    /// </example>
    /// </summary>
    public static class DomainReloadUtils
    {
        public static void CallOnResetOnAssets<T>() where T : IResetBeforeDomainReload
        {
            CallOnResetOnAssets(typeof(T));
        }

        /// <summary>
        /// Searches all assets for instance matching the given type and calls
        /// ResetBeforePlayMode() on them.
        /// </summary>
        /// <param name="type"></param>
        public static void CallOnResetOnAssets(System.Type type)
        {
            var guids = AssetDatabase.FindAssets("t:" + type.Name);
            if (guids == null)
                return;

            foreach (var guid in guids)
            {
                if (guid == null)
                    continue;

                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path == null)
                    continue;

                var asset = (IResetBeforeDomainReload) AssetDatabase.LoadAssetAtPath(path, type);
                if (asset == null)
                    continue;

                asset.ResetBeforePlayMode();
            }
        }
    }
}
#endif
