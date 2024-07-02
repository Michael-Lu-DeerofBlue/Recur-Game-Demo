using UnityEditor;
using UnityEngine;

namespace Kamgam.UGUIComponentsForSettings
{
    [CustomEditor(typeof(TabButtonUGUI))]
    public class TabButtonUGUIEditor : Editor
    {
        public TabButtonUGUI button;

        public void OnEnable()
        {
            button = target as TabButtonUGUI;
        }

        override public void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var oldText = button.Text;
            button.Text = EditorGUILayout.TextField("Text:", button.Text);
            if(oldText != button.Text)
            {
                // Schedule an update to the scene view will rerender (otherwise
                // the change would not be visible unless clicked into the scene view).
                EditorApplication.QueuePlayerLoopUpdate();
            }

            if (GUILayout.Button("Toggle"))
            {
                button.SetActive(!button.IsActive);
            }
        }
    }
}
