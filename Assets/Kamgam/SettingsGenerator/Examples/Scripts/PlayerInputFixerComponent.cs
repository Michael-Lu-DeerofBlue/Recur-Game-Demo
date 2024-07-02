#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEditor;
#endif

using UnityEngine;

namespace Kamgam.SettingsGenerator.Examples
{
    /// <summary>
    /// Replaces the StandaloneInputModule with a InputSystemUIInputModule 
    /// if the new input system is used.
    /// </summary>
    [ExecuteAlways]
    public class PlayerInputFixerComponent : MonoBehaviour
    {
#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
        public void Awake()
        {
            Fix();
        }

        public void Fix()
        {
            if (!EditorApplication.isPlaying)
            {
                var module = gameObject.GetComponent<UnityEngine.InputSystem.PlayerInput>();
                if (module == null)
                {
                    Debug.Log("PlayerInputFixerComponent: FIX: Adding.");
                    var input = gameObject.AddComponent<UnityEngine.InputSystem.PlayerInput>();

                    if (input.actions == null)
                    {
                        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:InputActionAsset");
                        if (guids.Length > 0)
                        {
                            foreach (var guid in guids)
                            {
                                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                                if (path.Contains("InputSystemBinding"))
                                {
                                    input.actions = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(path);
                                    UnityEditor.EditorUtility.SetDirty(this.gameObject);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Revert()
        {
            if (!EditorApplication.isPlaying)
            {
                var module = gameObject.GetComponent<UnityEngine.InputSystem.PlayerInput>();
                if (module != null)
                {
                    Debug.Log("PlayerInputFixerComponent: REVERT: Removing.");
                    DestroyImmediate(module);
                }
            }
        }
#endif
    }

#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
    [CustomEditor(typeof(PlayerInputFixerComponent))]
    public class PlayerInputFixerComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fix"))
            {
                (target as PlayerInputFixerComponent).Fix();
                EditorUtility.SetDirty(
                    (target as PlayerInputFixerComponent).gameObject
                    );
            }

            if (GUILayout.Button("Revert"))
            {
                (target as PlayerInputFixerComponent).Revert();
                EditorUtility.SetDirty(
                    (target as PlayerInputFixerComponent).gameObject
                    );
            }
        }
    }
#endif
}