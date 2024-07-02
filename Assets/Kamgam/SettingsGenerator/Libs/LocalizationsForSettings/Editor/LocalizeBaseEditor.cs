using UnityEditor;
using UnityEngine;

namespace Kamgam.LocalizationForSettings
{
    [CustomEditor(typeof(LocalizeBase))]
    public abstract class LocalizeBaseEditor : Editor
    {
        public LocalizeBase localizer;

        protected SerializedProperty _providerProp;
        protected SerializedProperty _termProp;
        protected SerializedProperty _updateFromTextProp;
        protected SerializedProperty _formatProp;

        protected string _lastText;

        public virtual void OnEnable()
        {
            localizer = target as LocalizeBase;
            _providerProp = serializedObject.FindProperty("LocalizationProvider");
            _termProp = serializedObject.FindProperty("Term");
            _updateFromTextProp = serializedObject.FindProperty("UpdateTermFromText");
            _formatProp = serializedObject.FindProperty("Format");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_providerProp);

            if (localizer.UpdateTermFromText
                && localizer.GetText() != null
                && localizer.GetText() != _lastText
                && localizer.LocalizationProvider.HasLocalization()
                && localizer.LocalizationProvider.GetLocalization().HasTerm(localizer.GetText().Trim()))
            {
                _termProp.stringValue = localizer.GetText().Trim();
            }
            if (localizer.GetText() != null)
                _lastText = localizer.GetText();

            // Draw term field in red is term is not found.
            bool foundTerm = LocalizationProvider.IsUsable(localizer.LocalizationProvider) && localizer.LocalizationProvider.Get(localizer.Term) != null;
            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = foundTerm ? Color.green : Color.red;
            EditorGUILayout.PropertyField(_termProp, new GUIContent("Term", "The term used for localization"));
            if (!foundTerm)
            {
                EditorGUILayout.HelpBox(new GUIContent("The term '" + _termProp.stringValue + "' has NOT been found in the localization table.\n" +
                    "It might be dynamic (then that's okay).\n" +
                    "But you might also have a typo in it (please check)."));
            }
            GUI.backgroundColor = bgColor;


            bool updateFromText = _updateFromTextProp.boolValue;
            EditorGUILayout.PropertyField(_updateFromTextProp);
            // Check if text machtes a term only if the checkbox was just enabled.
            if (_updateFromTextProp.boolValue && _updateFromTextProp.boolValue != updateFromText)
            {
                _lastText = null;
            }

            EditorGUILayout.PropertyField(_formatProp);

            if (serializedObject.hasModifiedProperties)
            {
                markAsChangedIfEditing();
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected void markAsChangedIfEditing()
        {
            if (EditorApplication.isPlaying)
                return;

            // Schedule an update to the scene view will rerender (otherwise
            // the change would not be visible unless clicked into the scene view).
            EditorApplication.QueuePlayerLoopUpdate();

            // Make sure the scene can be saved
            EditorUtility.SetDirty(localizer);

            // Make sure the Prefab recognizes the changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(localizer);
        }
    }
}
