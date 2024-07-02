using UnityEditor;
using UnityEngine;

namespace Kamgam.UGUIComponentsForSettings
{
    [CustomEditor(typeof(ColorPickerButtonUGUI))]
    public class ColorPickerButtonUGUIEditor : Editor
    {
        public ColorPickerButtonUGUI button;

        public void OnEnable()
        {
            button = target as ColorPickerButtonUGUI;
        }

        override public void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(button.ColorImage.color != button.Color)
            {
                button.ColorImage.color = button.Color;
                markAsChangedIfEditing();
            }
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
            PrefabUtility.RecordPrefabInstancePropertyModifications(button.ColorImage);
        }
    }
}
