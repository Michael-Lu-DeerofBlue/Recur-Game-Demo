using UnityEditor;
using UnityEngine;

namespace Kamgam.UGUIComponentsForSettings
{
    [CustomEditor(typeof(InputKeyUGUI))]
    public class InputKeyUGUIEditor : Editor
    {
        public InputKeyUGUI control;

        bool listeningForKeyPress = false;

        public void OnEnable()
        {
            control = target as InputKeyUGUI;
            control.UpdateKeyName();
        }

        override public void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(listeningForKeyPress ? "Stop listening" : "Listen For Key In Editor"))
            {
                if (!EditorApplication.isPlaying)
                {
                    listeningForKeyPress = !listeningForKeyPress;
                    if (!listeningForKeyPress)
                        control.SetActive(false);

                    markAsChangedIfEditing();
                }
                
            }
            EditorGUILayout.EndHorizontal();

            if (listeningForKeyPress)
            {
                control.SetActive(true);
                GUILayout.Label("Listening: PRESS NOW");

                if (Event.current.isKey && Event.current.type == EventType.KeyDown)
                {
                    var keyCode = InputUtils.KeyCodeToUniversalKeyCode(Event.current.keyCode);
                    if (keyCode != UniversalKeyCode.None && keyCode != UniversalKeyCode.Unknown)
                    {
                        control.Key = InputUtils.KeyCodeToUniversalKeyCode(Event.current.keyCode);
                    }

                    control.SetActive(false);
                    listeningForKeyPress = false;
                }

                markAsChangedIfEditing();
            }

            base.OnInspectorGUI();
        }

        protected void markAsChangedIfEditing()
        {
            if (EditorApplication.isPlaying)
                return;

            // Schedule an update to the scene view will rerender (otherwise
            // the change would not be visible unless clicked into the scene view).
            EditorApplication.QueuePlayerLoopUpdate();

            // Make sure the scene can be saved
            EditorUtility.SetDirty(control);

            // Make sure the Prefab recognizes the changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(control);
            PrefabUtility.RecordPrefabInstancePropertyModifications(control.TextTf);
            PrefabUtility.RecordPrefabInstancePropertyModifications(control.Active);
            PrefabUtility.RecordPrefabInstancePropertyModifications(control.Normal);
        }
    }
}
