using UnityEditor;
using UnityEngine;

namespace Kamgam.LocalizationForSettings
{
    [CustomEditor(typeof(LocalizationProvider))]
    public class LocalizationProviderEditor : Editor
    {
        public LocalizationProvider provider;

        protected SerializedProperty _translationsProp;
        protected SerializedProperty _autoDetectProp;
        protected SerializedProperty _defaultLanguageProp;

        protected int _selectedTranslationIndex = -1;

        protected Vector2 _localizationsScrollPos;
        protected Vector2 _translationScrollPos;

        public void OnEnable()
        {
            provider = target as LocalizationProvider;
            var loc = provider.GetLocalization();
            loc.Sort();
            _translationsProp = serializedObject.FindProperty("_localization._translations");
            _autoDetectProp = serializedObject.FindProperty("_localization.AutoDetectLanguage");
            _defaultLanguageProp = serializedObject.FindProperty("_localization.DefaultLanguage");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            bool autoDetect = _autoDetectProp.boolValue;
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying)
            {
                if (provider.GetLocalization().GetLanguageCount() > 0)
                {
                    int selected = provider.GetLocalization().GetLanguageIndex();
                    string[] options = provider.GetLocalization().GetLanguages().ToArray();
                    selected = EditorGUILayout.Popup("Current Language:", selected, options);
                    provider.GetLocalization().SetLanguageIndex(selected);
                }
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            if (autoDetect != _autoDetectProp.boolValue)
            {
                if (!_autoDetectProp.boolValue)
                {
                    provider.GetLocalization().SetLanguage(_defaultLanguageProp.stringValue);
                }
                else
                {
                    provider.GetLocalization().DetectLanguage(true);
                }
            }

            GUILayout.Space(5);
            string lang = provider.GetLocalization().GetLanguage();
            IMGUIUtils.DrawLabel("Current Language: " + (string.IsNullOrEmpty(lang) ? "Undefined" : lang));

            GUILayout.Space(15);
            IMGUIUtils.DrawLabel("Translations");

            var localization = provider.GetLocalization();
            int languageCount = localization.GetLanguageCount();
            int translationCount = localization.GetTranslationCount();

            if (provider.GetLocalization().GetLanguageCount() > 0)
            {
                // Terms table
                GUILayout.BeginVertical(EditorStyles.helpBox);
                float scrollViewContentHeight = (translationCount + 1) * (EditorGUIUtility.singleLineHeight + 4);
                float scrollViewHeight = Mathf.Min( 300, scrollViewContentHeight);
                _localizationsScrollPos = GUILayout.BeginScrollView(_localizationsScrollPos, GUILayout.Height(scrollViewHeight));

                // header
                GUILayout.BeginHorizontal();
                IMGUIUtils.DrawLabel("Term", bold: true, wordwrap: false);
                GUILayout.Space(40);
                GUILayout.Space(10); // Add additional space to reduce the likelihood of hidding the delete button instead of the scrollbar.

                GUILayout.EndHorizontal();

                // content
                int deleteTranslationIndex = -1;
                for (int i = 0; i < translationCount; i++)
                {
                    var translation = localization.GetTranslationAt(i);
                    if (translation == null)
                        continue;

                    GUILayout.BeginHorizontal();

                    // Edit Button
                    bool isBeingEdited = _selectedTranslationIndex == i;
                    GUI.enabled = !isBeingEdited;
                    var style = new GUIStyle(GUI.skin.button);
                    style.alignment = TextAnchor.MiddleLeft;
                    if (GUILayout.Button(new GUIContent(translation.GetTerm(), "Edit this term."), style))
                    {
                        _selectedTranslationIndex = i;
                    }
                    GUI.enabled = true;

                    // Delete Button
                    if (GUILayout.Button(new GUIContent("X", "Delete this term."), GUILayout.Width(24)))
                    {
                        Undo.RegisterCompleteObjectUndo(provider, "Removed term from localization.");
                        deleteTranslationIndex = i;
                    }

                    GUILayout.Space(10); // Add additional space to reduce the likelihood of hidding the delete button instead of the scrollbar.

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();

                bool ignoreSelectionThisFrame = false;

                GUILayout.EndVertical();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add Term", GUILayout.MaxWidth(100)))
                {
                    string newTerm = "NewTerm";
                    int index = 0;
                    while(localization.HasTerm(newTerm) && index < 30)
                    {
                        if (newTerm.EndsWith(")"))
                        {
                            newTerm = newTerm.Replace("(" + (index - 1) + ")", "(" + index + ")");
                        }
                        else
                        {
                            newTerm = newTerm + " (1)";
                        }
                        index++;
                    }
                    _selectedTranslationIndex = localization.CreateOrUpdateTranslation(newTerm, localization.GetLanguage(), "");
                    _localizationsScrollPos.y = scrollViewContentHeight + 20;
                    ignoreSelectionThisFrame = true;
                }
                GUILayout.EndHorizontal();

                // show selected translation
                if (!ignoreSelectionThisFrame && _selectedTranslationIndex >= 0 && _selectedTranslationIndex < localization.GetTranslationCount())
                {
                    var translation = localization.GetTranslationAt(_selectedTranslationIndex);
                    if (translation != null)
                    {
                        GUILayout.Space(10);

                        IMGUIUtils.DrawLabel("Selected Term:", bold: true);

                        // Term
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);

                            GUILayout.BeginHorizontal();

                            IMGUIUtils.DrawLabel("Term:", bold: true, wordwrap: false, options: GUILayout.Width(80));

                            string term = translation.GetTerm();
                            term = GUILayout.TextField(term);

                            // update in serialized object
                            _translationsProp.GetArrayElementAtIndex(_selectedTranslationIndex)
                                .FindPropertyRelative("_term")
                                .stringValue = term;

                            GUILayout.EndHorizontal();
                        }

                        // Translation per language
                        GUILayout.BeginScrollView(_translationScrollPos);
                        for (int i = 0; i < languageCount; i++)
                        {
                            var language = localization.GetLanguageAt(i);
                            GUILayout.BeginHorizontal();
                            IMGUIUtils.DrawLabel(language + ":", bold: false, wordwrap: false, options: GUILayout.Width(80));
                            string text = translation.GetText(i);
                            text = GUILayout.TextField(text);
                            translation.SetText(i, text);

                            // update in serialized object
                            var textsProp = _translationsProp.GetArrayElementAtIndex(_selectedTranslationIndex)
                                            .FindPropertyRelative("_texts");
                            if (i < textsProp.arraySize)
                            { 
                                textsProp.GetArrayElementAtIndex(i).stringValue = text;
                            }

                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndScrollView();

                        GUILayout.EndVertical();

                    }
                }

                // delete translation
                if (deleteTranslationIndex >= 0)
                {
                    var del = localization.GetTranslationAt(deleteTranslationIndex);
                    localization.DeleteTranslation(del.GetTerm());

                    if (deleteTranslationIndex > _selectedTranslationIndex)
                    {
                        _selectedTranslationIndex--;
                    }
                    else if(deleteTranslationIndex == _selectedTranslationIndex)
                    {
                        _selectedTranslationIndex = -1;
                    }
                }
            }

            // Sync changes to serialized object
            serializedObject.ApplyModifiedProperties();
        }
    }
}
