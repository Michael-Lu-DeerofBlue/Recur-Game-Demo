// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using UnityEditor;

namespace Kamgam.SettingsGenerator
{
    [CustomEditor(typeof(SettingResolverForVisualElement), editorForChildClasses: true)]
    public class SettingResolverForVisualElementEditor : SettingResolverEditor
    {
        SerializedProperty _classNameProp;

        public SettingResolver uiResolver;

        public override void OnEnable()
        {
            _classNameProp = serializedObject.FindProperty("BindingClass");
            _handledSerializedProperties.Add(_classNameProp.propertyPath);

            uiResolver = target as SettingResolverForVisualElement;

            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Provider field
            EditorGUILayout.PropertyField(_classNameProp);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
#endif
