using UnityEditor;

namespace PixelCrushers.DialogueSystem.I2Support
{

    [CustomEditor(typeof(DialogueSystemUseI2Language))]
    public class DialogueSystemUseI2LanguageEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DialogueSystemUseI2Language.assetsUse)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DialogueSystemUseI2Language.specifyLanguageBy)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DialogueSystemUseI2Language.extraEntryInfo)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DialogueSystemUseI2Language.dialogueEntryMinDigits)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DialogueSystemUseI2Language.useCustomFieldForEntries)));
            if (serializedObject.FindProperty(nameof(DialogueSystemUseI2Language.useCustomFieldForEntries)).boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DialogueSystemUseI2Language.customFieldTitle)));
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DialogueSystemUseI2Language.useI2LanguageOnStart)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DialogueSystemUseI2Language.useI2LanguageAtRuntime)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DialogueSystemUseI2Language.updateActorDisplayNamesOnConversationStart)));
            serializedObject.ApplyModifiedProperties();

        }
    }
}