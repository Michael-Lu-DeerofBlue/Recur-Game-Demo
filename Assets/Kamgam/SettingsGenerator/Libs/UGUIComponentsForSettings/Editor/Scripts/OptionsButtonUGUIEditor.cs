using UnityEditor;
using UnityEngine;

namespace Kamgam.UGUIComponentsForSettings
{
    [CustomEditor(typeof(OptionsButtonUGUI))]
    public class OptionsButtonUGUIEditor : Editor
    {
        public OptionsButtonUGUI button;

        public void OnEnable()
        {
            button = target as OptionsButtonUGUI;
            button.UpdateText();
        }

        override public void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var oldValue = button.SelectedIndex;
            button.SelectedIndex = Mathf.Clamp( EditorGUILayout.IntField("Value:", button.SelectedIndex), 0, button.NumOfOptions-1);
            if (oldValue != button.SelectedIndex)
            {
                if (!EditorApplication.isPlaying)
                    EditorApplication.QueuePlayerLoopUpdate();
            }

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Prev"))
            {
                button.Prev();
                markAsChangedIfEditing();
            }

            if (GUILayout.Button("Next"))
            {
                button.Next();
                markAsChangedIfEditing();
            }

            EditorGUILayout.EndHorizontal();
        }

        protected void markAsChangedIfEditing()
        {
            if (EditorApplication.isPlaying)
                return;

            // Schedule an update to the scene view will rerender (otherwise
            // the change would not be visible unless clicked into the scene view).
            EditorApplication.QueuePlayerLoopUpdate();

            // Make sure the scene can be saved
            EditorUtility.SetDirty(button);

            // Make sure the Prefab recognizes the changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(button);
            PrefabUtility.RecordPrefabInstancePropertyModifications(button.TextTf);
        }
    }
}
