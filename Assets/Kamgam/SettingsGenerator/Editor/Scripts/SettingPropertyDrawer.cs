using UnityEditor;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [CustomPropertyDrawer(typeof(ISetting), true)]
    public class SettingProperyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
        }
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Replace label with ID.
            try
            {
                bool isActive = property.FindPropertyRelative("_isActive").boolValue;
                string id = property.FindPropertyRelative("ID").stringValue;
                string text = (isActive ? "[x]  " : "[  ]  ") + id;

                if (string.IsNullOrEmpty(id))
                    text = "(New Setting)";

                label = new GUIContent(text);
            }
            catch
            {
                // keep default label
            }

            EditorGUI.PropertyField(rect, property, label, property.isExpanded);
        }
    }
}
