using UnityEditor;
using UnityEngine;

namespace Kamgam.UGUIComponentsForSettings
{
    [CustomEditor(typeof(SliderUGUI))]
    public class SliderUGUIEditor : Editor
    {
        public SliderUGUI slider;

        public void OnEnable()
        {
            slider = target as SliderUGUI;
        }

        override public void OnInspectorGUI()
        {
            if (slider == null)
            {
                base.OnInspectorGUI();
                return;
            }

            var oldText = slider.Text;
            slider.Text = EditorGUILayout.TextField("Text:", slider.Text);
            if(oldText != slider.Text)
            {
                markAsChangedIfEditing();
            }

            var oldValue = slider.Value;
            slider.Value = EditorGUILayout.Slider("Value:", slider.Value, slider.MinValue, slider.MaxValue);
            if (oldValue != slider.Value)
            {
                markAsChangedIfEditing();
            }

            if (slider.WholeNumbers)
            {
                slider.MinValue = Mathf.Round(slider.MinValue);
                slider.MaxValue = Mathf.Round(slider.MaxValue);
            }

            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Decrease (-)"))
            {
                slider.Decrease();
                markAsChangedIfEditing();
            }

            if (GUILayout.Button("Increase (+)"))
            {
                slider.Increase();
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
            if (slider != null)
            {
                EditorUtility.SetDirty(slider);

                if (slider.TextTf != null)
                    EditorUtility.SetDirty(slider.TextTf);

                if (slider.Slider != null)
                    EditorUtility.SetDirty(slider.Slider);
            }

            // Make sure the Prefab recognizes the changes
            if (slider != null)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(slider);

                if (slider.TextTf != null)
                    PrefabUtility.RecordPrefabInstancePropertyModifications(slider.TextTf);

                if (slider.Slider != null)
                    PrefabUtility.RecordPrefabInstancePropertyModifications(slider.Slider);
            }
        }
    }
}
