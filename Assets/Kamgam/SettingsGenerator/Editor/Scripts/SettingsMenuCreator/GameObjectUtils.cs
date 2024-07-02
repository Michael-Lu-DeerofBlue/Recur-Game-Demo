using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public static class GameObjectUtils
    {
        public static T FindObjectOfType<T>(bool includeInactive = false) where T : UnityEngine.Object
        {
#if UNITY_2023_1_OR_NEWER
            return GameObject.FindFirstObjectByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);
#else
            return GameObject.FindObjectOfType<T>(includeInactive);
#endif
        }
    }
}
