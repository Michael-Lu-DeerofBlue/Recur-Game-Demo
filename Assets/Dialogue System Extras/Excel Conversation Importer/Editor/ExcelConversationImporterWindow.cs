using System;
using UnityEngine;
using UnityEditor;
using UnityQuickSheet;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This window imports Excel files into linear conversations.
    /// Each conversation should be in a separate worksheet. The worksheet's name will be
    /// the conversation's title.
    /// In each worksheet, the first column is the actor. The second column is the dialogue text.
    /// 
    /// NOTE: Requires https://github.com/kimsama/Unity-QuickSheet
    /// </summary>
    public class ExcelConversationImporterWindow : EditorWindow
    {

        [MenuItem("Tools/Pixel Crushers/Dialogue System/Import/Excel", false)]
        public static void Init()
        {
            GetWindow<ExcelConversationImporterWindow>(false, "DS Excel");
        }

        private ExcelConversationImporterWindowPrefs prefs;
        private Template template;

        private void OnEnable()
        {
            template = TemplateTools.LoadFromEditorPrefs();
            prefs = ExcelConversationImporterWindowPrefs.Load();
        }

        private void OnDisable()
        {
            prefs.Save();
        }

        private void OnGUI()
        {
            DrawMainExcelFilepath();
            DrawDatabase();
            DrawButtons();
        }

        private void DrawMainExcelFilepath()
        {
            EditorGUILayout.HelpBox("Excel file should have one conversation per worksheet. First column is actor, second column is dialogue text.", MessageType.None);
            EditorGUILayout.BeginHorizontal();
            prefs.excelFilepath = EditorGUILayout.TextField(new GUIContent("Excel File", "Main Excel file containing conversations."), prefs.excelFilepath);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(22)))
            {
                prefs.excelFilepath =
                    EditorUtility.OpenFilePanel("Select Main Excel File",
                                                EditorWindowTools.GetDirectoryName(prefs.excelFilepath),
                                                "xlsx");
                GUIUtility.keyboardControl = 0;
            }
            EditorGUILayout.EndHorizontal();
            prefs.secondRowIsConversationTitle = EditorGUILayout.Toggle(new GUIContent("Second Row Is Title", "Second row, second column (B2) of each worksheet is conversation's title."), prefs.secondRowIsConversationTitle);
        }

        private void DrawDatabase()
        {
            prefs.database = EditorGUILayout.ObjectField(new GUIContent("Dialogue Database", "Dialogue database to update with content from Excel file."), prefs.database, typeof(DialogueDatabase), false) as DialogueDatabase;
        }

        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(prefs.database == null || string.IsNullOrEmpty(prefs.excelFilepath));
            if (GUILayout.Button("Import Excel File", GUILayout.Width(128))) ImportExcelFile();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        private void ImportExcelFile()
        {
            try
            {
                EditorUtility.DisplayCancelableProgressBar("Excel Conversation Importer", "Importing Excel file " + prefs.excelFilepath, 0);
                ExcelQuery query = new ExcelQuery(prefs.excelFilepath, "");
                if (query == null )
                {
                    Debug.LogError("Can't load " + prefs.excelFilepath + "!");
                    return;
                }
                string[] sheetNames = query.GetSheetNames();
                for (int i = 0; i < sheetNames.Length; i++)
                {
                    var title = sheetNames[i];
                    var progress = (float)i / (float)sheetNames.Length;
                    if (EditorUtility.DisplayCancelableProgressBar("Excel Conversation Importer", "Importing conversation " + title, progress)) break;
                    ImportSheet(title, new ExcelQuery(prefs.excelFilepath, title));
                }
                Debug.Log("Dialogue System: Imported " + sheetNames.Length + " worksheets from " + prefs.excelFilepath);
            }
            finally
            {
                EditorUtility.SetDirty(prefs.database);
                EditorUtility.ClearProgressBar();
            }
        }

        private void ImportSheet(string title, ExcelQuery query)
        {
            var rows = query.Deserialize<ExcelConversationData>().ToArray();

            // Check if first row is conversation title:
            if (prefs.secondRowIsConversationTitle && rows.Length > 0)
            {
                title = rows[0].Text;
            }
            int firstTextRow = prefs.secondRowIsConversationTitle ? 1 : 0;

            // Prepare conversation:
            var conversation = prefs.database.GetConversation(title);
            DialogueEntry lastEntry;
            if (conversation != null)
            {
                conversation.dialogueEntries.Clear();
            }
            else
            {
                // Create new conversation:
                conversation = template.CreateConversation(template.GetNextConversationID(prefs.database), title);
                prefs.database.conversations.Add(conversation);
            }
            lastEntry = template.CreateDialogueEntry(template.GetNextDialogueEntryID(conversation), conversation.id, "START");
            conversation.dialogueEntries.Add(lastEntry);
            lastEntry.canvasRect.x = lastEntry.canvasRect.y = 0;

            for (int i = firstTextRow; i < rows.Length; i++)
            {
                var row = rows[i];
                var actorName = row.Actor;
                var dialogueText = row.Text;

                var actor = !string.IsNullOrEmpty(actorName) ? prefs.database.GetActor(actorName) : null;
                if (actor == null)
                {
                    actor = template.CreateActor(template.GetNextActorID(prefs.database), actorName, false);
                    prefs.database.actors.Add(actor);
                }

                var entry = template.CreateDialogueEntry(template.GetNextDialogueEntryID(conversation), conversation.id, string.Empty);
                conversation.dialogueEntries.Add(entry);
                entry.ActorID = actor.id;
                entry.DialogueText = dialogueText;
                lastEntry.outgoingLinks.Add(new Link(conversation.id, lastEntry.id, conversation.id, entry.id));
                lastEntry = entry;
            }
        }

    }

    [Serializable]
    public class ExcelConversationImporterWindowPrefs : ISerializationCallbackReceiver
    {
        public string excelFilepath;
        public string databaseGuid;
        public bool secondRowIsConversationTitle = false;

        private DialogueDatabase m_database = null;
        public DialogueDatabase database
        {
            get
            {
                if (m_database == null && !string.IsNullOrEmpty(databaseGuid))
                {
                    m_database = AssetDatabase.LoadAssetAtPath<DialogueDatabase>(AssetDatabase.GUIDToAssetPath(databaseGuid));
                }
                return m_database;
            }
            set
            {
                m_database = value;
            }
        }

        private const string PrefsKey = "PixelCrushers.DialogueSystem.ExcelImporter";

        public static ExcelConversationImporterWindowPrefs Load()
        {
            var prefs = EditorPrefs.HasKey(PrefsKey) ? JsonUtility.FromJson<ExcelConversationImporterWindowPrefs>(EditorPrefs.GetString(PrefsKey)) : null;
            if (prefs == null) prefs = new ExcelConversationImporterWindowPrefs();
            return prefs;
        }

        public void Save()
        {
            EditorPrefs.SetString(PrefsKey, JsonUtility.ToJson(this));
        }

        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
            databaseGuid = (database != null) ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(database)) : string.Empty;
        }
    }
}
