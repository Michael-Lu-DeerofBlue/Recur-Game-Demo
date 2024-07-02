using UnityEditor;

namespace Kamgam.LocalizationForSettings
{
    [CustomEditor(typeof(LocalizeTMPro))]
    public class LocalizeTMProEditor : LocalizeBaseEditor
    {
        protected SerializedProperty _textfieldProp;

        public override void OnEnable()
        {
            _textfieldProp = serializedObject.FindProperty("Textfield");

            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_textfieldProp);

            if (serializedObject.hasModifiedProperties)
            {
                markAsChangedIfEditing();
            }

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
