// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.I2Support
{

    /// <summary>
    /// Editor window to copy fields between a Dialogue System dialogue database
    /// and/or localized text table and I2 Localization.
    /// </summary>
    public class DSToI2Window : EditorWindow
    {

        #region Menu Item

        [MenuItem("Tools/Pixel Crushers/Dialogue System/Third Party/I2 Localization/DS To I2", false)]
        public static void ShowWindow()
        {
            GetWindow<DSToI2Window>(false, "DS To I2").minSize = new Vector2(340, 160);
        }

        #endregion

        #region Fields & Properties

        private const string DSI2Category = "Dialogue System/";

        private const string DSToI2PrefsKey = "PixelCrushers.DialogueSystem.DSToI2Prefs";

        [SerializeField]
        private DSToI2Prefs m_prefs = new DSToI2Prefs();
        private DSToI2Prefs prefs
        {
            get { return m_prefs; }
            set { m_prefs = value; }
        }

        [SerializeField]
        private Vector2 m_scrollPosition = Vector2.zero;
        private Vector2 scrollPosition
        {
            get { return m_scrollPosition; }
            set { m_scrollPosition = value; }
        }

        private ReorderableList m_databaseList;
        private ReorderableList m_textTableList;

        private List<string> languages;
        private List<string> languageCodes;
        private int languageCount;

        #endregion

        #region Initialization

        private void OnEnable()
        {
            LoadPrefs();
            InitDatabasesReorderableList();
            InitTextTablesReorderableList();
        }

        private void OnDisable()
        {
            SavePrefs();
        }

        private void LoadPrefs()
        {
            if (EditorPrefs.HasKey(DSToI2PrefsKey))
            {
                prefs = JsonUtility.FromJson<DSToI2Prefs>(EditorPrefs.GetString(DSToI2PrefsKey));
                prefs.AssignDatabasesFromInstanceIDs();
            }
        }

        private void SavePrefs()
        {
            EditorPrefs.SetString(DSToI2PrefsKey, JsonUtility.ToJson(prefs));
        }

        private void CacheLanguageList()
        {
            languages = I2.Loc.LocalizationManager.GetAllLanguages();
            languageCodes = I2.Loc.LocalizationManager.GetAllLanguagesCode();
            languageCount = Mathf.Min(languageCodes.Count, languages.Count);
        }

        #endregion

        #region GUI

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawDatabaseList();
            DrawTextTableList();
            prefs.localizedTextTable = EditorGUILayout.ObjectField(new GUIContent("Localized Text Table", "Optional legacy Localized Text Table to localize."), prefs.localizedTextTable, typeof(LocalizedTextTable), false) as LocalizedTextTable;

            prefs.specifyI2Asset = EditorGUILayout.ToggleLeft(new GUIContent("Specify I2 Asset", "Specify I2 asset. If unticked, use default I2Languages."), prefs.specifyI2Asset);
            if (prefs.specifyI2Asset)
            {
                EditorWindowTools.StartIndentedSection();
                prefs.i2Asset = EditorGUILayout.ObjectField(new GUIContent("I2 Asset", "I2 asset to sync with Dialogue System."), prefs.i2Asset, typeof(I2.Loc.LanguageSourceAsset), false) as I2.Loc.LanguageSourceAsset;
                EditorWindowTools.EndIndentedSection();
            }
            EditorGUILayout.EndVertical();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            try
            {
                // Database foldouts:
                EditorGUI.BeginDisabledGroup(prefs.GetNumDatabases() == 0);
                for (int i = 0; i < DSToI2Prefs.NumDatabaseCategories; i++)
                {
                    DrawFoldout((DSToI2Prefs.Category)i, prefs.categoryFoldouts[i]);
                }
                EditorGUI.EndDisabledGroup();

                // Text table foldouts:
                EditorGUI.BeginDisabledGroup(prefs.GetNumTextTables() == 0);
                DrawFoldout(DSToI2Prefs.Category.TextTable, prefs.GetCategoryFoldout(DSToI2Prefs.Category.TextTable));
                EditorGUI.EndDisabledGroup();

                // Localized text table foldouts:
                EditorGUI.BeginDisabledGroup(prefs.localizedTextTable == null);
                DrawFoldout(DSToI2Prefs.Category.LocalizedTextTable, prefs.GetCategoryFoldout(DSToI2Prefs.Category.LocalizedTextTable));
                EditorGUI.EndDisabledGroup();
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            prefs.assetIdentifier = (DSToI2Prefs.AssetIdentifierType)EditorGUILayout.EnumPopup(new GUIContent("Assets Use", "Name localized fields using asset ID number or Name/Title."), prefs.assetIdentifier);
            prefs.languageIdentifier = (I2LanguageIdentifierType)EditorGUILayout.EnumPopup(new GUIContent("Fields Use", "Name localized fields using language code or language name."), prefs.languageIdentifier);
            prefs.useFieldForDialogueEntryTerms = EditorGUILayout.Toggle(new GUIContent("Custom Field for Entries", "Use a specified custom field for dialogue entry terms."), prefs.useFieldForDialogueEntryTerms);
            if (prefs.useFieldForDialogueEntryTerms)
            {
                prefs.fieldForDialogueEntryTerms = EditorGUILayout.TextField(new GUIContent("   Field Title", "Use this field title for dialogue entry terms."), prefs.fieldForDialogueEntryTerms);
                prefs.includeConvInfoWithField = EditorGUILayout.Toggle(new GUIContent("   Include Conv Info", "Include conversation ID/title in term; if unticked, term only includes field value."), prefs.includeConvInfoWithField);
            }
            else
            {
                prefs.dialogueEntryInfo = (DSToI2Prefs.DialogueEntryInfo)EditorGUILayout.EnumPopup(new GUIContent("Extra Entry Info", "Add this extra info to dialogue entry terms."), prefs.dialogueEntryInfo);
                prefs.dialogueEntryMinDigits = EditorGUILayout.IntField(new GUIContent("Entry Min Digits", "Pad entry IDs with zeroes to at least this many digits. Improves sorting on dialogue entry terms."), prefs.dialogueEntryMinDigits);
            }
            prefs.useTextTableNameForI2Term = EditorGUILayout.Toggle(new GUIContent("Text Table Name As Term", "Use text table's name in term instead of 'Dialogue System'."), prefs.useTextTableNameForI2Term);
            prefs.translationsToI2 = EditorGUILayout.Toggle(new GUIContent("Translations To I2", "'To I2' button also copies translations of selected fields to I2. CAREFUL: This may overwrite translations in I2."), prefs.translationsToI2);
            if (prefs.translationsToI2)
            {
                prefs.specifyLanguageToI2 = EditorGUILayout.Toggle(new GUIContent("   Specify Language", "'To I2' button also copies translations only of specified language to I2. CAREFUL: This may overwrite translations in I2."), prefs.specifyLanguageToI2);
                if (prefs.specifyLanguageToI2)
                {
                    prefs.specificLanguageToI2 = EditorGUILayout.TextField(new GUIContent("   Language To I2", "Copy only this localized field to I2 when copying translations to I2."), prefs.specificLanguageToI2);
                }
            }
            prefs.updatePrimaryFields = EditorGUILayout.Toggle(new GUIContent("Update Primary Fields", "Update primary field (e.g., Dialogue Text) with version in specified language."), prefs.updatePrimaryFields);
            if (prefs.updatePrimaryFields)
            {
                EditorWindowTools.StartIndentedSection();
                prefs.updatePrimaryFieldsFromLanguage = EditorGUILayout.TextField(new GUIContent("From Language", "Update primary field (e.g., Dialogue Text) with version in this language (e.g., 'en')."), prefs.updatePrimaryFieldsFromLanguage);
                EditorWindowTools.EndIndentedSection();
            }
            prefs.verbose = (DSToI2Prefs.Verbose)EditorGUILayout.EnumPopup(new GUIContent("Verbose", "Amount of detail to log to Console window."), prefs.verbose);
            prefs.showHelp = EditorGUILayout.Toggle("Show Help", prefs.showHelp);
            if (prefs.showHelp)
            {
                EditorGUILayout.HelpBox("1. Assign dialogue database and/or text table(s).\n" +
                    "2. Expand foldouts. Tick fields you want to localize.\n" +
                    "3. Click To I2.\n" +
                    "4. Translate in I2 (eg, Languages tab, click Translate).\n" +
                    "5. Click From I2.", MessageType.None);
            }
            var disallow = prefs.GetNumDatabases() == 0 && prefs.localizedTextTable == null && prefs.GetNumTextTables() == 0;
            if (prefs.specifyI2Asset && prefs.i2Asset == null) disallow = true;
            EditorGUI.BeginDisabledGroup(disallow);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Refresh", "Refresh list of fields. Click this if you added new fields to your dialogue database.")))
            {
                prefs.PopulateAllDatabaseFields();
                prefs.PopulateAllTextTableFields();
                prefs.PopulateLocalizedTextTableFields();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button(new GUIContent("Clear I2", "Remove Dialogue System fields from I2Languages prefab.")))
            {
                if (EditorUtility.DisplayDialog("Clear I2", "Clear all Dialogue System fields from the I2Languages prefab?", "OK", "Cancel"))
                {
                    ClearI2();
                    GUIUtility.ExitGUI();
                }
            }
            if (GUILayout.Button(new GUIContent("To I2", "Add ticked fields to I2Languages prefab.")))
            {
                CopyToI2();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button(new GUIContent("Inspect I2", "Add ticked fields to I2Languages prefab.")))
            {
                InspectI2();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button(new GUIContent("From I2", "Update fields with the values in I2Languages prefab.")))
            {
                RetrieveFromI2();
                GUIUtility.ExitGUI();
            }
            //// Will enable when I2 adds support:
            //if (GUILayout.Button(new GUIContent("Translate All", "Add fields to I2Languages, tell I2 to translate using Google Translate, and update fields from I2Languages.")))
            //{
            //    TranslateAll();
            //}
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        private void InitDatabasesReorderableList()
        {
            m_databaseList = new ReorderableList(prefs.databases, typeof(DialogueDatabase), true, true, true, true);
            m_databaseList.drawHeaderCallback += OnDrawDatabasesListHeader;
            m_databaseList.drawElementCallback += OnDrawDatabasesListElement;
            m_databaseList.onAddCallback += OnAddDatabasesList;
        }

        private void DrawDatabaseList()
        {
            EditorGUI.BeginChangeCheck();
            m_databaseList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                prefs.PopulateAllDatabaseFields();
            }
        }

        private void OnDrawDatabasesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Databases");
        }

        private void OnDrawDatabasesListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < prefs.databases.Count)) return;
            prefs.databases[index] = EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                GUIContent.none, prefs.databases[index], typeof(DialogueDatabase), false) as DialogueDatabase;
        }

        private void OnAddDatabasesList(ReorderableList list)
        {
            prefs.databases.Add(null);
        }

        private void InitTextTablesReorderableList()
        {
            m_textTableList = new ReorderableList(prefs.textTables, typeof(TextTable), true, true, true, true);
            m_textTableList.drawHeaderCallback += OnDrawTextTablesListHeader;
            m_textTableList.drawElementCallback += OnDrawTextTablesListElement;
            m_textTableList.onAddCallback += OnAddTextTablesList;
        }

        private void DrawTextTableList()
        {
            EditorGUI.BeginChangeCheck();
            m_textTableList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                prefs.PopulateAllTextTableFields();
            }
            if (prefs.textTables.Count > 1) EditorGUILayout.LabelField("Any new fields in i2 will add to first text table.");
        }

        private void OnDrawTextTablesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Text Tables");
        }

        private void OnDrawTextTablesListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < prefs.textTables.Count)) return;
            prefs.textTables[index] = EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                GUIContent.none, prefs.textTables[index], typeof(TextTable), false) as TextTable;
        }

        private void OnAddTextTablesList(ReorderableList list)
        {
            prefs.textTables.Add(null);
        }

        private void DrawFoldout(DSToI2Prefs.Category category, FoldoutInfo foldoutInfo)
        {
            var label = (category == DSToI2Prefs.Category.Items) ? "Items/Quests" : category.ToString();
            foldoutInfo.foldout = EditorGUILayout.Foldout(foldoutInfo.foldout, label);
            if (foldoutInfo.foldout)
            {
                EditorGUI.indentLevel++;

                // All/None buttons:
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(string.Empty, GUILayout.Width(12));
                if (GUILayout.Button("All", GUILayout.Width(64)))
                {
                    foldoutInfo.fieldSelections.SetAllSelections(true);
                }
                if (GUILayout.Button("None", GUILayout.Width(64)))
                {
                    foldoutInfo.fieldSelections.SetAllSelections(false);
                }
                EditorGUILayout.EndHorizontal();

                // All fields:
                var titles = foldoutInfo.fieldSelections.titles;
                var includes = foldoutInfo.fieldSelections.includes;
                var fieldTypes = foldoutInfo.fieldSelections.fieldTypes;
                for (int i = 0; i < titles.Count; i++)
                {
                    if (!(0 <= i && i < includes.Count)) continue;
                    EditorGUI.BeginDisabledGroup(fieldTypes != null && i < fieldTypes.Count && !IsTextFieldType(fieldTypes[i]));
                    includes[i] = EditorGUILayout.ToggleLeft(titles[i], includes[i]);
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.indentLevel--;
            }
        }

        private void InspectI2()
        {
            if (prefs.specifyI2Asset && prefs.i2Asset != null)
            {
                Selection.activeObject = prefs.i2Asset;
            }
            else
            {
                var languageSource = FindObjectOfType<I2.Loc.LanguageSource>();
                if (languageSource != null)
                {
                    Selection.activeObject = languageSource;
                }
                else
                {
                    var languageSourceAsset = Resources.Load("I2Languages");
                    if (languageSourceAsset != null)
                    {
                        Selection.activeObject = languageSourceAsset;
                    }
                    else
                    {
                        Debug.LogWarning("Can't find I2Languages.");
                    }
                }
            }
        }

        #endregion

        #region To I2

        private void CopyToI2()
        {
            CacheLanguageList();

            foreach (var database in prefs.databases)
            {
                CopyDatabaseToI2(database);
            }

            // Text table:
            foreach (var textTable in prefs.textTables)
            {
                if (textTable == null) continue;
                try
                {
                    EditorUtility.DisplayProgressBar("Copying to I2", "Copying text table '" + textTable.name + "' fields...", 1);
                    AddTextTableFields(textTable);
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            // Localized text table:
            if (prefs.localizedTextTable != null)
            {
                try
                {
                    EditorUtility.DisplayProgressBar("Copying to I2", "Copying localized text table fields...", 1);
                    AddLocalizedTextTableFields();
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            Debug.Log("Copied terms from Dialogue System to " + ((prefs.specifyI2Asset && prefs.i2Asset) ? prefs.i2Asset.name : "I2Languages") + ".");

            SaveI2();
            RepaintI2();
        }

        private void CopyDatabaseToI2(DialogueDatabase database)
        {
            // Dialogue database:
            if (database == null) return;
            try
            {
                var total = 4 + database.conversations.Count;
                EditorUtility.DisplayProgressBar("Copying to I2", "Copying actor fields...", (1 / total));
                foreach (var actor in database.actors)
                {
                    AddFields("Actor/" + GetAssetIdentifier(actor.id, actor.Name), actor.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.Actors), false);
                }
                EditorUtility.DisplayProgressBar("Copying to I2", "Copying item/quest fields...", (2 / total));
                foreach (var item in database.items)
                {
                    AddFields("Item-Quest/" + GetAssetIdentifier(item.id, item.Name), item.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.Items), false);
                }
                EditorUtility.DisplayProgressBar("Copying to I2", "Copying location fields...", (3 / total));
                foreach (var location in database.locations)
                {
                    AddFields("Location/" + GetAssetIdentifier(location.id, location.Name), location.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.Locations), false);
                }
                EditorUtility.DisplayProgressBar("Copying to I2", "Copying variable fields...", (4 / total));
                foreach (var variable in database.variables)
                {
                    AddFields("Variable/" + GetAssetIdentifier(variable.id, variable.Name), variable.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.Variables), false);
                }
                float current = 4;
                foreach (var conversation in database.conversations)
                {
                    current += 1;
                    EditorUtility.DisplayProgressBar("Copying to I2", "Copying conversation '" + conversation.Title + "'.", (current / total));
                    AddFields("Conversation/" + GetAssetIdentifier(conversation.id, conversation.Title), conversation.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.Conversations), false);
                    var sanitizedConversationTitle = SanitizeTerm(conversation.Title);
                    foreach (var entry in conversation.dialogueEntries)
                    {
                        var extraInfo = string.Empty;
                        switch (prefs.dialogueEntryInfo)
                        {
                            case DSToI2Prefs.DialogueEntryInfo.Actor:
                                var actor = database.GetActor(entry.ActorID);
                                extraInfo = "/" + ((actor != null) ? actor.Name : string.Empty);
                                break;
                            case DSToI2Prefs.DialogueEntryInfo.Text:
                                extraInfo = "/" + entry.DialogueText;
                                break;
                        }
                        var dialogueEntryHeader = GetDialogueEntryHeader(entry, sanitizedConversationTitle, conversation.id, entry.id, extraInfo);
                        AddFields(dialogueEntryHeader, entry.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.DialogueEntries), true);
                        var description = Field.LookupValue(entry.fields, DialogueSystemFields.Description);
                        if (!string.IsNullOrEmpty(description))
                        {
                            var sanitizedTitle = SanitizeTerm("Dialogue Text");
                            var term = DSI2Category + dialogueEntryHeader + "/" + sanitizedTitle;
                            term = I2.Loc.I2Utils.GetValidTermName(term, true);
                            var data = GetTermData(term);
                            if (data != null)
                            {
                                data.Description = description;
                            }
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private bool IsTextFieldType(FieldType fieldType)
        {
            return fieldType == FieldType.Text || fieldType == FieldType.Localization;
        }

        private string GetAssetIdentifier(int assetID, string assetName)
        {
            return (prefs.assetIdentifier == DSToI2Prefs.AssetIdentifierType.ID) ? assetID.ToString() : SanitizeTerm(assetName);
        }

        private string GetDialogueEntryHeader(DialogueEntry entry, string sanitizedConversationTitle, int conversationID, int entryID, string extraInfo)
        {
            var conversationPart = (prefs.assetIdentifier == DSToI2Prefs.AssetIdentifierType.ID)
                ? conversationID.ToString("D" + prefs.dialogueEntryMinDigits)
                : sanitizedConversationTitle;
            if (prefs.useFieldForDialogueEntryTerms && !string.IsNullOrEmpty(prefs.fieldForDialogueEntryTerms))
            {
                var fieldValue = Field.LookupValue(entry.fields, prefs.fieldForDialogueEntryTerms);
                if (string.IsNullOrEmpty(fieldValue))
                {
                    if (prefs.showWarnings && entry.id != 0) Debug.LogWarning("Dialogue System: Entry " + conversationID + ":" + entryID + " '" + entry.DialogueText + "' doesn't have a field '" + prefs.fieldForDialogueEntryTerms + "'");
                }
                {
                    if (prefs.includeConvInfoWithField)
                    {
                        return "Conversation/" + conversationPart +
                        "/Entry/" + fieldValue + extraInfo;
                    }
                    else
                    {
                        return fieldValue;
                    }
                }
            }
            else
            {
                return "Conversation/" + conversationPart +
                    "/Entry/" + entryID.ToString("D" + prefs.dialogueEntryMinDigits) + extraInfo;
            }
        }

        private void AddTextTableFields(TextTable textTable)
        {
            var fieldSelections = prefs.GetFieldSelections(DSToI2Prefs.Category.TextTable);
            foreach (var kvp in textTable.fields)
            {
                var fieldID = kvp.Key;
                var field = kvp.Value;
                if (!(fieldSelections.ShouldInclude(field.fieldName))) continue;
                var value = (field.texts.Count > 0 && !string.IsNullOrEmpty(field.texts[0])) ? field.texts[0] : field.fieldName;
                var sanitizedFieldName = SanitizeTerm(field.fieldName);
                AddField("TextTable", sanitizedFieldName, value, GetTextTableCategory(textTable));
                if (prefs.translationsToI2)
                {
                    var term = GetTextTableCategory(textTable) + "TextTable" + "/" + sanitizedFieldName;
                    term = I2.Loc.I2Utils.GetValidTermName(term, true);
                    for (int j = 0; j < languageCount; j++)
                    {
                        var languageForLocalizedFieldTitle = (prefs.languageIdentifier == I2LanguageIdentifierType.LanguageCode) ? languageCodes[j] : languages[j];
                        if (prefs.specifyLanguageToI2 && languageForLocalizedFieldTitle != prefs.specificLanguageToI2) continue;
                        var languageID = textTable.GetLanguageID(languageForLocalizedFieldTitle);
                        if (languageID == 0)
                        { // If TextTable uses different scheme than DialogueDatabase, try the other one:
                            languageForLocalizedFieldTitle = (prefs.languageIdentifier != I2LanguageIdentifierType.LanguageCode) ? languageCodes[j] : languages[j];
                            languageID = textTable.GetLanguageID(languageForLocalizedFieldTitle);
                        }
                        if (languageID != 0)
                        {
                            AddTranslation(term, languageForLocalizedFieldTitle, j, field.GetTextForLanguage(languageID));
                        }
                    }
                }
            }
        }

        private string GetTextTableCategory(TextTable textTable)
        {
            return prefs.useTextTableNameForI2Term ? (textTable.name.Replace("/", ".") + "/")
                : DSI2Category;
        }

        private void AddLocalizedTextTableFields()
        {
            var fieldSelections = prefs.GetFieldSelections(DSToI2Prefs.Category.LocalizedTextTable);
            for (int i = 0; i < prefs.localizedTextTable.fields.Count; i++)
            {
                var field = prefs.localizedTextTable.fields[i];
                if (!(fieldSelections.ShouldInclude(field.name))) continue;
                var value = (field.values.Count > 0 && !string.IsNullOrEmpty(field.values[0])) ? field.values[0] : field.name;
                AddField("LocalizedTextTable", SanitizeTerm(field.name), value);
            }
        }

        private void AddFields(string header, List<Field> fields, FieldSelectionDictionary fieldSelections, bool isDialogueEntry)
        {
            AddFields(header, fields, fieldSelections);
            if (prefs.translationsToI2)
            {
                for (int i = 0; i < fields.Count; i++)
                {
                    var field = fields[i];
                    if (IsTextFieldType(field.type))
                    {
                        if (!(fieldSelections.ShouldInclude(field.title))) continue;
                        var sanitizedTitle = SanitizeTerm(field.title);
                        var isDialogueText = isDialogueEntry && string.Equals(sanitizedTitle, "Dialogue Text");
                        var term = DSI2Category + header + "/" + SanitizeTerm(field.title);
                        term = I2.Loc.I2Utils.GetValidTermName(term, true);
                        for (int j = 0; j < languageCount; j++)
                        {
                            var languageForLocalizedFieldTitle = (prefs.languageIdentifier == I2LanguageIdentifierType.LanguageCode) ? languageCodes[j] : languages[j];
                            if (prefs.specifyLanguageToI2 && languageForLocalizedFieldTitle != prefs.specificLanguageToI2) continue;
                            var locTitle = isDialogueText ? languageForLocalizedFieldTitle : (sanitizedTitle + " " + languageForLocalizedFieldTitle);
                            var locField = Field.Lookup(fields, locTitle);
                            if (locField != null)
                            {
                                AddTranslation(term, languageForLocalizedFieldTitle, j, locField.value);
                            }
                        }
                    }
                }
            }
        }

        private void AddFields(string header, List<Field> fields, FieldSelectionDictionary fieldSelections)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                if (IsTextFieldType(field.type))
                {
                    if (!(fieldSelections.ShouldInclude(field.title))) continue;
                    AddField(header, SanitizeTerm(field.title), field.value);
                }
            }
        }

        private void AddField(string header, string title, string value, string category = null)
        {
            if (string.IsNullOrEmpty(value)) return;
            var term = ((category == null) ? DSI2Category : category) + header + "/" + title;
            term = I2.Loc.I2Utils.GetValidTermName(term, true);
            if (prefs.showDetails) Debug.Log("Dialogue System: Adding term " + term);
            if (prefs.i2Asset != null)
            {
                UpdateTerm(prefs.i2Asset.SourceData, term, value);
            }
            else
            {
                foreach (var source in I2.Loc.LocalizationManager.Sources)
                {
                    UpdateTerm(source, term, value);
                }
            }
        }

        private void AddTranslation(string term, string language, int languageIndex, string value)
        {
            if (string.IsNullOrEmpty(term) || string.IsNullOrEmpty(value) || string.IsNullOrEmpty(language)) return;
            var data = GetTermData(term);
            if (data != null && (0 <= languageIndex && languageIndex < data.Languages.Length))
            {
                data.Languages[languageIndex] = value;
                if (prefs.showDetails) Debug.Log("Dialogue System: Adding translation " + term + " [" + language + "]");
            }
        }

        private I2.Loc.TermData GetTermData(string term)
        {
            if (prefs.specifyI2Asset && prefs.i2Asset != null)
            {
                return prefs.i2Asset.SourceData.GetTermData(term);
            }
            else
            {
                return I2.Loc.LocalizationManager.GetTermData(term);
            }
        }

        private void UpdateTerm(I2.Loc.LanguageSourceData source, string term, string value)
        {
            var termData = source.AddTerm(term, I2.Loc.eTermType.Text, false);
            if (termData.Languages.Length > 0) termData.Languages[0] = value;
        }

        private void ClearI2()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Clearing I2", "Clearing Dialogue System fields from I2Languages.", 0.01f);
                var terms = I2.Loc.LocalizationManager.GetTermsList();
                float numTerms = terms.Count;
                for (int i = 0; i < terms.Count; i++)
                {
                    var term = terms[i];
                    if (term.StartsWith(DSI2Category) || term.Contains("/TextTable/"))
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Clearing I2", term, (float)i / numTerms)) break;
                        if (prefs.showDetails) Debug.Log("Dialogue System: Removing term " + term);
                        if (prefs.i2Asset != null)
                        {
                            prefs.i2Asset.SourceData.RemoveTerm(term);
                        }
                        else
                        {
                            I2.Loc.LocalizationManager.Sources.ForEach(source => source.RemoveTerm(term));
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                Debug.Log("Removed all Dialogue System terms from I2.");
            }
            SaveI2();
            RepaintI2();
        }

        private void SaveI2()
        {
            if (prefs.i2Asset != null)
            {
                EditorUtility.SetDirty(prefs.i2Asset);
            }
            else
            {
                I2.Loc.LocalizationManager.Sources.ForEach(source => EditorUtility.SetDirty(source.ownerObject));
            }
            AssetDatabase.SaveAssets();
        }

        #endregion

        #region RetrieveFromI2

        private void RetrieveFromI2()
        {
            CacheLanguageList();

            foreach (var database in prefs.databases)
            {
                RetrieveDatabaseFromI2(database);
            }

            // Text table:
            var existingTextTableFields = new HashSet<string>();
            TextTable firstTextTable = null;
            foreach (var textTable in prefs.textTables)
            {
                if (textTable == null) continue;
                if (firstTextTable == null) firstTextTable = textTable;
                try
                {
                    EditorUtility.DisplayProgressBar("Retrieving from I2", "Retrieving text table '" + textTable.name + "' fields...", 1);
                    RetrieveTextTableFields(textTable, existingTextTableFields);
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }
            if (firstTextTable != null)
            {
                RetrieveNewTextTableFields(firstTextTable, existingTextTableFields);
            }

            // Localized text table:
            if (prefs.localizedTextTable != null)
            {
                try
                {
                    EditorUtility.DisplayProgressBar("Retrieving from I2", "Retrieving localized text table fields...", 1);
                    RetrieveLocalizedTextTableFields();
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            Debug.Log("Copied terms from " + ((prefs.specifyI2Asset && prefs.i2Asset) ? prefs.i2Asset.name : "I2Languages") + " to Dialogue System.");

            AssetDatabase.SaveAssets();
            RepaintDialogueSystem();
        }

        private void RetrieveDatabaseFromI2(DialogueDatabase database)
        {
            if (database == null) return;
            try
            {
                var total = 4 + database.conversations.Count;
                EditorUtility.DisplayProgressBar("Retrieving from I2", "Retrieving actor fields...", (1 / total));
                foreach (var actor in database.actors)
                {
                    RetrieveFields("Actor/" + GetAssetIdentifier(actor.id, actor.Name), actor.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.Actors));
                }
                EditorUtility.DisplayProgressBar("Retrieving from I2", "Retrieving item/quest fields...", (2 / total));
                foreach (var item in database.items)
                {
                    RetrieveFields("Item-Quest/" + GetAssetIdentifier(item.id, item.Name), item.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.Items));
                }
                EditorUtility.DisplayProgressBar("Retrieving from I2", "Retrieving location fields...", (3 / total));
                foreach (var location in database.locations)
                {
                    RetrieveFields("Location/" + GetAssetIdentifier(location.id, location.Name), location.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.Locations));
                }
                EditorUtility.DisplayProgressBar("Retrieving from I2", "Retrieving variable fields...", (4 / total));
                foreach (var variable in database.variables)
                {
                    RetrieveFields("Variable/" + GetAssetIdentifier(variable.id, variable.Name), variable.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.Variables));
                }
                float current = 4;
                foreach (var conversation in database.conversations)
                {
                    current += 1;
                    EditorUtility.DisplayProgressBar("Retrieving from I2", "Retrieving conversation '" + conversation.Title + "'.", (current / total));
                    RetrieveFields("Conversation/" + GetAssetIdentifier(conversation.id, conversation.Title), conversation.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.Conversations));
                    var sanitizedConversationTitle = SanitizeTerm(conversation.Title);
                    foreach (var entry in conversation.dialogueEntries)
                    {
                        var extraInfo = string.Empty;
                        switch (prefs.dialogueEntryInfo)
                        {
                            case DSToI2Prefs.DialogueEntryInfo.Actor:
                                var actor = database.GetActor(entry.ActorID);
                                extraInfo = "/" + ((actor != null) ? actor.Name : string.Empty);
                                break;
                            case DSToI2Prefs.DialogueEntryInfo.Text:
                                extraInfo = "/" + entry.DialogueText;
                                break;
                        }
                        RetrieveFields(GetDialogueEntryHeader(entry, sanitizedConversationTitle, conversation.id, entry.id, extraInfo), entry.fields, prefs.GetFieldSelections(DSToI2Prefs.Category.DialogueEntries));
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.SetDirty(database);
            }
        }

        private void RetrieveFields(string header, List<Field> fields, FieldSelectionDictionary fieldSelections)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                if (IsTextFieldType(field.type))
                {
                    if (!(fieldSelections.ShouldInclude(field.title))) continue;
                    if (string.IsNullOrEmpty(field.value)) continue;
                    var sanitizedTitle = SanitizeTerm(field.title);
                    var term = DSI2Category + header + "/" + sanitizedTitle;
                    term = I2.Loc.I2Utils.GetValidTermName(term, true);
                    var data = GetTermData(term);
                    if (data == null)
                    {
                        if (prefs.showWarnings) Debug.LogWarning("Dialogue System: I2Languages doesn't contain data for " + term);
                        continue;
                    }
                    for (int j = 0; j < languageCount; j++)
                    {
                        var languageForLocalizedFieldTitle = (prefs.languageIdentifier == I2LanguageIdentifierType.LanguageCode) ? languageCodes[j] : languages[j];
                        var isDialogueText = string.Equals(sanitizedTitle, "Dialogue Text");
                        var locTitle = isDialogueText ? languageForLocalizedFieldTitle : (sanitizedTitle + " " + languageForLocalizedFieldTitle);
                        var locField = Field.Lookup(fields, locTitle);
                        if (locField == null)
                        {
                            locField = new Field(locTitle, string.Empty, FieldType.Localization);
                            fields.Add(locField);
                        }
                        if (!(0 <= j && j < data.Languages.Length)) continue;
                        locField.value = ReplaceNewlineCodes(data.Languages[j]);
                        if (prefs.showDetails) Debug.Log("Dialogue System: " + header + " " + locTitle + " = " + data.Languages[j]);

                        if (prefs.updatePrimaryFields && prefs.updatePrimaryFieldsFromLanguage == languageForLocalizedFieldTitle)
                        {
                            field.value = locField.value;
                            if (prefs.dialogueEntryInfo == DSToI2Prefs.DialogueEntryInfo.Text)
                            {
                                var newTerm = DSI2Category + header + "/" + sanitizedTitle;
                                newTerm = I2.Loc.I2Utils.GetValidTermName(newTerm, true);
                                data.Term = newTerm;
                            }
                        }
                    }
                }
            }
        }

        private void RetrieveTextTableFields(TextTable textTable, HashSet<string> allExistingTextTableFields)
        {
            var languages = I2.Loc.LocalizationManager.GetAllLanguages();
            var languageCodes = I2.Loc.LocalizationManager.GetAllLanguagesCode();

            // Set up text table's languages:
            textTable.languages.Clear();
            textTable.languages.Add("Default", 0);
            var count = (prefs.languageIdentifier == I2LanguageIdentifierType.LanguageCode) ? languageCodes.Count : languages.Count;
            for (int i = 0; i < count; i++)
            {
                var languageForTextTable = (prefs.languageIdentifier == I2LanguageIdentifierType.LanguageCode) ? languageCodes[i] : languages[i];
                textTable.languages.Add(languageForTextTable, i + 1);
            }

            var termPrefix = GetTextTableCategory(textTable) + "TextTable/";
            var existingFieldNames = new HashSet<string>();

            // Retrieve field values:
            var fieldSelections = prefs.GetFieldSelections(DSToI2Prefs.Category.TextTable);
            foreach (var kvp in textTable.fields)
            {
                var fieldID = kvp.Key;
                var field = kvp.Value;
                if (!(fieldSelections.ShouldInclude(field.fieldName))) continue;
                if (string.IsNullOrEmpty(field.fieldName)) continue;
                existingFieldNames.Add(field.fieldName);
                allExistingTextTableFields.Add(field.fieldName);
                if (field.texts.Count == 0)
                {
                    field.texts.Add(0, field.fieldName); // Default.
                }
                var term = termPrefix + SanitizeTerm(field.fieldName);
                term = I2.Loc.I2Utils.GetValidTermName(term, true);
                var data = GetTermData(term);
                if (data == null)
                {
                    if (prefs.showWarnings) Debug.LogWarning("Dialogue System: I2Languages doesn't contain data for " + term);
                    continue;
                }
                for (int j = 0; j < count; j++)
                {
                    var languageforFieldName = (prefs.languageIdentifier == I2LanguageIdentifierType.LanguageCode) ? languageCodes[j] : languages[j];
                    var valueIndex = j + 1;
                    if (valueIndex >= field.texts.Count) field.texts.Add(j + 1, string.Empty);
                    field.texts[valueIndex] = ReplaceNewlineCodes(data.Languages[j]);
                    if (prefs.showDetails) Debug.Log("Dialogue System: " + term + " " + languageforFieldName + " = " + data.Languages[j]);
                }
            }

            if (prefs.useTextTableNameForI2Term)
            {
                // Add new new fields defined in i2 but not yet in text table:
                var terms = I2.Loc.LocalizationManager.GetTermsList();
                foreach (var term in terms)
                {
                    if (term == null || !term.StartsWith(termPrefix)) continue;
                    var fieldName = term.Substring(termPrefix.Length);
                    if (!existingFieldNames.Contains(fieldName))
                    {
                        var data = GetTermData(term);
                        textTable.AddField(fieldName);
                        for (int j = 0; j < count; j++)
                        {
                            var languageforFieldName = (prefs.languageIdentifier == I2LanguageIdentifierType.LanguageCode) ? languageCodes[j] : languages[j];
                            textTable.SetFieldTextForLanguage(fieldName, languageforFieldName, data.Languages[j]);
                        }
                        allExistingTextTableFields.Add(fieldName);
                    }
                }
            }

            EditorUtility.SetDirty(textTable);

            if (TextTableEditorWindow.instance != null)
            {
                Selection.activeObject = null;
                Selection.activeObject = textTable;
                TextTableEditorWindow.instance.Repaint();
            }
        }

        private void RetrieveNewTextTableFields(TextTable textTable, HashSet<string> existingFieldNames)
        {
            if (prefs.useTextTableNameForI2Term) return;

            // Add new new fields defined in i2 but not yet in any text tables:
            var count = (prefs.languageIdentifier == I2LanguageIdentifierType.LanguageCode) ? languageCodes.Count : languages.Count;
            var termPrefix = GetTextTableCategory(textTable) + "TextTable/";
            var terms = I2.Loc.LocalizationManager.GetTermsList();
            foreach (var term in terms)
            {
                if (term == null || !term.StartsWith(termPrefix)) continue;
                var fieldName = term.Substring(termPrefix.Length);
                if (!existingFieldNames.Contains(fieldName))
                {
                    var data = GetTermData(term);
                    textTable.AddField(fieldName);
                    for (int j = 0; j < count; j++)
                    {
                        var languageforFieldName = (prefs.languageIdentifier == I2LanguageIdentifierType.LanguageCode) ? languageCodes[j] : languages[j];
                        textTable.SetFieldTextForLanguage(fieldName, languageforFieldName, data.Languages[j]);
                    }
                }
            }
        }

        private void RetrieveLocalizedTextTableFields()
        {
            var languages = I2.Loc.LocalizationManager.GetAllLanguages(); //.Sources[0].GetLanguages();
            var languageCodes = I2.Loc.LocalizationManager.GetAllLanguagesCode(); //.Sources[0].GetLanguagesCode();

            // Set up localized text table's languages:
            prefs.localizedTextTable.languages.Clear();
            prefs.localizedTextTable.languages.Add("Default");
            var count = (prefs.languageIdentifier == I2LanguageIdentifierType.LanguageCode) ? languageCodes.Count : languages.Count;
            for (int i = 0; i < count; i++)
            {
                prefs.localizedTextTable.languages.Add(languages[i]);
            }

            // Retrieve field values:
            var fieldSelections = prefs.GetFieldSelections(DSToI2Prefs.Category.LocalizedTextTable);
            for (int i = 0; i < prefs.localizedTextTable.fields.Count; i++)
            {
                var field = prefs.localizedTextTable.fields[i];
                if (!(fieldSelections.ShouldInclude(field.name))) continue;
                if (string.IsNullOrEmpty(field.name)) continue;
                if (field.values.Count == 0)
                {
                    field.values.Add(field.name);
                }
                var term = DSI2Category + "LocalizedTextTable/" + SanitizeTerm(field.name);
                term = I2.Loc.I2Utils.GetValidTermName(term, true);
                var data = GetTermData(term);
                if (data == null)
                {
                    if (prefs.showWarnings) Debug.LogWarning("Dialogue System: I2Languages doesn't contain data for " + term);
                    continue;
                }
                for (int j = 0; j < count; j++)
                {
                    var languageForLocalizedFieldTitle = (prefs.languageIdentifier == I2LanguageIdentifierType.LanguageCode) ? languageCodes[j] : languages[j];
                    var valueIndex = j + 1;
                    if (valueIndex >= field.values.Count) field.values.Add(string.Empty);
                    field.values[valueIndex] = ReplaceNewlineCodes(data.Languages[j]);
                    if (prefs.showDetails) Debug.Log("Dialogue System: " + term + " " + languageForLocalizedFieldTitle + " = " + data.Languages[j]);
                }
            }
            EditorUtility.SetDirty(prefs.localizedTextTable);
        }

        private string ReplaceNewlineCodes(string s)
        {
            return string.IsNullOrEmpty(s) ? s : s.Replace("\\n", "\n");
        }

        //private string ReplaceForwardSlashes(string s)
        //{
        //    return s.Replace("/", "%2F").Replace("[%2Fi2nt]", "[/i2nt]");
        //}

        private string SanitizeTerm(string s)
        {
            return string.IsNullOrEmpty(s) ? s : I2.Loc.I2Utils.GetValidTermName(s.Replace("/", ".").Replace("\\", ".").Replace("[", ".").Replace("]", "."));
        }

        #endregion

        #region Translate All

        //// Will add when supported.
        //private void TranslateAll()
        //{
        //    if (!I2.Loc.GoogleTranslation.CanTranslate())
        //    {
        //        EditorUtility.DisplayDialog("Google Translate Error", "I2's WebService is not set correctly or needs to be reinstalled. Please correct this and then click Translate All again.", "OK");
        //        return;
        //    }
        //    var languages = I2.Loc.LocalizationManager.Sources[0].GetLanguages();
        //    for (int i = 0; i < languages.Count; i++)
        //    {
        //        var language = languages[i];
        //        typeof(I2.Loc.LocalizationEditor).GetMethod("TranslateAllToLanguage",
        //                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).
        //                    Invoke(obj: null, parameters: new object[] { language });
        //    }
        //}

        #endregion

        #region Repaints

        private void RepaintI2()
        {
            typeof(I2.Loc.LocalizationEditor).GetMethod("ParseTerms",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).
                Invoke(obj: null, parameters: new object[] { true, false, false });
        }

        private void RepaintDialogueSystem()
        {
            RepaintDialogueEditorWindow();
            RepaintEditor<LocalizedTextTableEditor>();
            RepaintEditor<TextTableEditor>();
        }

        private void RepaintDialogueEditorWindow()
        {
            var instance = DialogueEditor.DialogueEditorWindow.instance;
            if (instance != null)
            {
                instance.Reset();
                instance.Repaint();
            }
        }

        private void RepaintEditor<T>() where T : Editor
        {
            var editors = Resources.FindObjectsOfTypeAll<T>();
            for (int i = 0; i < editors.Length; i++)
            {
                editors[i].Repaint();
            }
        }

        #endregion

    }
}
