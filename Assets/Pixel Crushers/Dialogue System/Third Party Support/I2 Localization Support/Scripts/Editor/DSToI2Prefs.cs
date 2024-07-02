// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using I2.Loc;

namespace PixelCrushers.DialogueSystem.I2Support
{

    [Serializable]
    public class FoldoutInfo
    {
        public bool foldout;
        public FieldSelectionDictionary fieldSelections = new FieldSelectionDictionary();
    }

    /// <summary>
    /// Holds the DS To I2 window's current prefs, which are saved into EditorPrefs
    /// between editor sessions.
    /// </summary>
    [Serializable]
    public class DSToI2Prefs : ISerializationCallbackReceiver
    {
        public enum Category { Actors, Items, Locations, Variables, Conversations, DialogueEntries, LocalizedTextTable, TextTable }

        public const int NumDatabaseCategories = 6;

        public const int NumTotalCategories = 8;

        public List<int> databaseInstanceIDs = new List<int>();

        public List<int> textTableInstanceIDs = new List<int>();

        public int localizedTextTableInstanceID = 0;

        public bool specifyI2Asset = false;

        public int i2AssetInstanceID = 0;

        public int dialogueEntryMinDigits = 1;

        public FoldoutInfo[] categoryFoldouts = new FoldoutInfo[NumTotalCategories];

        public bool translationsToI2 = false;

        public bool specifyLanguageToI2 = false;

        public string specificLanguageToI2 = "en";

        public bool updatePrimaryFields = false;

        public string updatePrimaryFieldsFromLanguage = "en";

        public bool useFieldForDialogueEntryTerms = false;

        public string fieldForDialogueEntryTerms = string.Empty;

        public bool includeConvInfoWithField = false;

        public bool useTextTableNameForI2Term = false;

        public enum DialogueEntryInfo { None, Actor, Text }

        public DialogueEntryInfo dialogueEntryInfo = DialogueEntryInfo.None;

        public I2LanguageIdentifierType languageIdentifier = I2LanguageIdentifierType.LanguageCode;

        public enum AssetIdentifierType { ID, Name }

        public AssetIdentifierType assetIdentifier = AssetIdentifierType.ID;

        public enum Verbose { None, Warnings, Detailed }

        public Verbose verbose = Verbose.None;

        public bool showHelp = false;

        public bool showWarnings { get { return verbose == Verbose.Warnings || verbose == Verbose.Detailed; } }

        public bool showDetails { get { return verbose == Verbose.Detailed; } }

        public DSToI2Prefs()
        {
            for (int i = 0; i < NumTotalCategories; i++)
            {
                categoryFoldouts[i] = new FoldoutInfo();
            }
        }

        private List<DialogueDatabase> m_databases = new List<DialogueDatabase>();
        public List<DialogueDatabase> databases { get { return m_databases; } }

        private List<TextTable> m_textTables = new List<TextTable>();
        public List<TextTable> textTables { get { return m_textTables; } }

        private LocalizedTextTable m_localizedTextTable = null;

        private LanguageSourceAsset m_i2Asset = null;

        public int GetNumDatabases()
        {
            int count = 0;
            for (int i = 0; i < m_databases.Count; i++)
            {
                if (m_databases[i] != null) count++;
            }
            return count;
        }

        public int GetNumTextTables()
        {
            int count = 0;
            for (int i = 0; i < m_textTables.Count; i++)
            {
                if (m_textTables[i] != null) count++;
            }
            return count;
        }

        //public TextTable textTable
        //{
        //    get
        //    {
        //        if (m_textTable == null && textTableInstanceID != 0)
        //        {
        //            m_textTable = EditorUtility.InstanceIDToObject(textTableInstanceID) as TextTable;
        //            if (m_textTable != null) PopulateTextTableFields();
        //        }
        //        return m_textTable;
        //    }
        //    set
        //    {
        //        textTableInstanceID = (value != null) ? value.GetInstanceID() : 0;
        //        if (value != m_textTable)
        //        {
        //            m_textTable = value;
        //            if (m_textTable != null) PopulateTextTableFields();
        //        }
        //    }
        //}

        public LocalizedTextTable localizedTextTable
        {
            get
            {
                if (m_localizedTextTable == null && localizedTextTableInstanceID != 0)
                {
                    m_localizedTextTable = EditorUtility.InstanceIDToObject(localizedTextTableInstanceID) as LocalizedTextTable;
                    if (m_localizedTextTable != null) PopulateLocalizedTextTableFields();
                }
                return m_localizedTextTable;
            }
            set
            {
                localizedTextTableInstanceID = (value != null) ? value.GetInstanceID() : 0;
                if (value != m_localizedTextTable)
                {
                    m_localizedTextTable = value;
                    if (m_localizedTextTable != null) PopulateLocalizedTextTableFields();
                }
            }
        }

        public LanguageSourceAsset i2Asset
        {
            get
            {
                if (m_i2Asset == null && i2AssetInstanceID == 0)
                {
                    m_i2Asset = EditorUtility.InstanceIDToObject(i2AssetInstanceID) as LanguageSourceAsset;
                }
                return m_i2Asset;
            }
            set
            {
                i2AssetInstanceID = (value != null) ? value.GetInstanceID() : 0;
                if (value != m_i2Asset)
                {
                    m_i2Asset = value;
                }
            }
        }

        public void PopulateAllDatabaseFields()
        {
            for (int i = 0; i < NumDatabaseCategories; i++)
            {
                if (categoryFoldouts[i] == null) categoryFoldouts[i] = new FoldoutInfo();
                var fieldSelections = categoryFoldouts[i].fieldSelections;
                fieldSelections.Clear();
            }
            foreach (var database in m_databases)
            {
                if (database == null) return;
                for (int i = 0; i < NumDatabaseCategories; i++)
                {
                    if (categoryFoldouts[i] == null) categoryFoldouts[i] = new FoldoutInfo();
                    var fieldSelections = categoryFoldouts[i].fieldSelections;
                    switch ((Category)i)
                    {
                        case Category.Actors:
                            fieldSelections.PopulateDictionary<Actor>(database.actors);
                            break;
                        case Category.Items:
                            fieldSelections.PopulateDictionary<Item>(database.items);
                            break;
                        case Category.Locations:
                            fieldSelections.PopulateDictionary<Location>(database.locations);
                            break;
                        case Category.Variables:
                            fieldSelections.PopulateDictionary<Variable>(database.variables);
                            break;
                        case Category.Conversations:
                            fieldSelections.PopulateDictionary<Conversation>(database.conversations);
                            break;
                        case Category.DialogueEntries:
                            fieldSelections.PopulateDictionaryWithDialogueEntries(database.conversations);
                            break;
                    }
                }
            }
        }

        public void PopulateAllTextTableFields()
        {
            var index = (int)Category.TextTable;
            if (categoryFoldouts[index] == null) categoryFoldouts[index] = new FoldoutInfo();
            categoryFoldouts[index].fieldSelections.Clear();
            foreach (var textTable in m_textTables)
            {
                if (textTable == null) return;
                var i = (int)Category.TextTable;
                categoryFoldouts[i].fieldSelections.PopulateDictionaryWithTextTable(textTable, true);
            }
        }

        public void PopulateLocalizedTextTableFields()
        {
            if (localizedTextTable == null) return;
            var i = (int)Category.LocalizedTextTable;
            if (categoryFoldouts[i] == null) categoryFoldouts[i] = new FoldoutInfo();
            categoryFoldouts[i].fieldSelections.PopulateDictionaryWithLocalizedTextTable(localizedTextTable);
        }

        public FoldoutInfo GetCategoryFoldout(Category category)
        {
            var index = (int)category;
            if (index >= categoryFoldouts.Length)
            {
                var list = new List<FoldoutInfo>(categoryFoldouts);
                for (int i = categoryFoldouts.Length; i <= index; i++)
                {
                    list.Add(new FoldoutInfo());
                }
                categoryFoldouts = list.ToArray();
            }
            return categoryFoldouts[index];
        }

        public FieldSelectionDictionary GetFieldSelections(Category category)
        {
            return GetCategoryFoldout(category).fieldSelections;
        }

        public void OnBeforeSerialize()
        {
            databaseInstanceIDs.Clear();
            for (int i = 0; i < m_databases.Count; i++)
            {
                var database = m_databases[i];
                databaseInstanceIDs.Add((database == null) ? 0 : database.GetInstanceID());
            }

            textTableInstanceIDs.Clear();
            for (int i = 0; i < m_textTables.Count; i++)
            {
                var textTable = m_textTables[i];
                textTableInstanceIDs.Add((textTable == null) ? 0 : textTable.GetInstanceID());
            }
        }

        public void OnAfterDeserialize() { }

        public void AssignDatabasesFromInstanceIDs()
        {
            m_databases.Clear();
            for (int i = 0; i < databaseInstanceIDs.Count; i++)
            {
                m_databases.Add(EditorUtility.InstanceIDToObject(databaseInstanceIDs[i]) as DialogueDatabase);
            }

            m_textTables.Clear();
            for (int i = 0; i < textTableInstanceIDs.Count; i++)
            {
                m_textTables.Add(EditorUtility.InstanceIDToObject(textTableInstanceIDs[i]) as TextTable);
            }

            if (specifyI2Asset)
            {
                i2Asset = EditorUtility.InstanceIDToObject(i2AssetInstanceID) as LanguageSourceAsset;
            }
        }
    }

}
