// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using Kamgam.LocalizationForSettings.UIElements;
using UnityEditor;
using UnityEngine;

namespace Kamgam.LocalizationForSettings
{
    [CustomEditor(typeof(LocalizeVisualElement), editorForChildClasses: true)]
    public class LocalizeVisualElementEditor : LocalizeBaseEditor
    {
        protected LocalizeVisualElement _localizer;
        protected SerializedProperty _classNameProp;

        public override void OnEnable()
        {
            _localizer = target as LocalizeVisualElement;
            _classNameProp = serializedObject.FindProperty("BindingClass");

            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Binding class
            EditorGUILayout.PropertyField(_classNameProp);
            var color = GUI.color;
            var newColor = color;
            newColor.a -= 0.3f;
            GUI.color = newColor;
            EditorGUILayout.LabelField("Bound Element Type: " + _localizer.GetBindingClassElement().GetType().Name);
            EditorGUILayout.LabelField("Localize Comp Type: " + _localizer.GetElementType().Name);
            GUI.color = color;
            if (_localizer.VisualElement == null || _localizer.GetBindingClassElement().GetType() != _localizer.GetElementType())
            {
                EditorGUILayout.HelpBox(
                    "PLEASE NOTICE that the type of the element referenced by the BindingClass is of type '"+ _localizer.GetBindingClassElement().GetType().Name + "' " +
                    "and does not match the localize component type '"+ _localizer.GetElementType().Name + "'.\n\n" +
                    "It is assumed you have referenced the root of a custom control (aka Template or 'Prefab'). " +
                    "To get a matching element the VERY FIRST element of type '"+ _localizer.GetElementType().Name + "' down the visual tree hierarchy will be used." +
                    "\n\nThis is useful to localize labels in simple controls. However, localizing MULTIPLE elements within a custom control from outside like this is not supported." +
                    " If you need to localize multiple elements then you should add the localization to the custom control logic instead."
                    , MessageType.Info, true);
            }

            // Save changes
            if (serializedObject.hasModifiedProperties)
            {
                markAsChangedIfEditing();
            }
            serializedObject.ApplyModifiedProperties();

            // Draw normal ui
            base.OnInspectorGUI();
        }
    }
}
#endif
