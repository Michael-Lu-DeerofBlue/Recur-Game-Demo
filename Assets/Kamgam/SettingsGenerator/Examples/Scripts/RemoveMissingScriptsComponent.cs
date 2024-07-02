#if UNITY_EDITOR
using UnityEngine.EventSystems;
using UnityEditor;
#endif

using UnityEngine;

namespace Kamgam.BikeAndCharacter25D.Examples
{
    /// <summary>
    /// Removes any component that has a missing script.
    /// </summary>
    [ExecuteAlways]
    public class RemoveMissingScriptsComponent : MonoBehaviour
    {
#if UNITY_EDITOR
        public void Awake()
        {
            Fix();
        }

        public void Fix()
        {
            if (!EditorApplication.isPlaying)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(this.gameObject);
                EditorUtility.SetDirty(this.gameObject);
            }
        }
#endif
    }
}