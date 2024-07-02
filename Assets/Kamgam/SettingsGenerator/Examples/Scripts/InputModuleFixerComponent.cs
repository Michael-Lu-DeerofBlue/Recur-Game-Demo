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
    public class InputModuleFixerComponent : MonoBehaviour
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
                var standaloneModule = gameObject.GetComponent<StandaloneInputModule>();
                if (standaloneModule != null)
                {
                    Debug.Log("InputModuleFixerComponent: FIX: Replacing StandaloneInputModule with InputSystemUIInputModule.");
                    DestroyImmediate(standaloneModule);
                    gameObject.AddComponent<InputSystemUIInputModule>();
                }
            }
        }

        public void Revert()
        {
            if (!EditorApplication.isPlaying)
            {
                var module = gameObject.GetComponent<InputSystemUIInputModule>();
                if (module != null)
                {
                    Debug.Log("InputModuleFixerComponent: REVERT: Replacing InputSystemUIInputModule with StandaloneInputModule.");
                    DestroyImmediate(module);
                    gameObject.AddComponent<StandaloneInputModule>();
                }
            }
        }
#endif
    }

#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
    [CustomEditor(typeof(InputModuleFixerComponent))]
    public class InputModuleFixerComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var fixer = (target as InputModuleFixerComponent);

            if (GUILayout.Button("Fix"))
            {
                fixer.Fix();
                EditorUtility.SetDirty(fixer.gameObject);
            }

            if (GUILayout.Button("Revert"))
            {
                fixer.Revert();
                EditorUtility.SetDirty(fixer.gameObject);
            }
        }
    }
#endif
}