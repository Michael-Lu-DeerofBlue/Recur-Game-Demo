// Copyright (c) Pixel Crushers. All rights reserved.

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    public static class DialogueDatabaseEditorTools
    {

        public static bool debug { get; set; } = false;

        public static void ReorderIDsInConversationsDepthFirst(DialogueDatabase database)
        {
            foreach (var conversation in database.conversations)
            {
                ReorderIDsInConversationDepthFirst(database, conversation);
            }
        }

        public static void ReorderIDsInConversationDepthFirst(DialogueDatabase database, Conversation conversation)
        {
            if (conversation == null) return;
            try
            {
                EditorUtility.DisplayProgressBar("Reordering IDs", conversation.Title, 0);

                // Determine new order:
                var newIDs = new Dictionary<int, int>();
                int nextID = 0;
                DetermineNewEntryID(database, conversation, conversation.GetFirstDialogueEntry(), newIDs, ref nextID);

                // Include orphans:
                foreach (var entry in conversation.dialogueEntries)
                {
                    if (newIDs.ContainsKey(entry.id)) continue;
                    newIDs.Add(entry.id, nextID);
                    nextID++;
                }

                if (debug)
                {
                    var s = conversation.Title + " new IDs:\n";
                    foreach (var kvp in newIDs)
                    {
                        s += kvp.Key + " --> " + kvp.Value + "\n";
                    }
                    Debug.Log(s);
                }

                // Change IDs:
                int tempOffset = 100000;
                foreach (var kvp in newIDs)
                {
                    ChangeEntryIDEverywhere(database, conversation.id, kvp.Key, kvp.Value + tempOffset);
                }
                foreach (var kvp in newIDs)
                {
                    ChangeEntryIDEverywhere(database, conversation.id, kvp.Value + tempOffset, kvp.Value);
                }

                // Sort entries:
                conversation.dialogueEntries.Sort((x, y) => x.id.CompareTo(y.id));

            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void DetermineNewEntryID(
            DialogueDatabase database, 
            Conversation conversation, 
            DialogueEntry entry,
            Dictionary<int, int> newIDs, ref int nextID)
        {
            if (conversation == null || entry == null || entry.conversationID != conversation.id) return;
            newIDs.Add(entry.id, nextID);
            nextID++;
            for (int i = 0; i < entry.outgoingLinks.Count; i++)
            {
                var child = database.GetDialogueEntry(entry.outgoingLinks[i]);
                if (child == null) continue;
                if (newIDs.ContainsKey(child.id)) continue;
                DetermineNewEntryID(database, conversation, child, newIDs, ref nextID);
            }
        }

        private static void ChangeEntryIDEverywhere(DialogueDatabase database, int conversationID, int oldID, int newID)
        {
            for (int c = 0; c < database.conversations.Count; c++)
            {
                var conversation = database.conversations[c];
                for (int e = 0; e < conversation.dialogueEntries.Count; e++)
                {
                    var entry = conversation.dialogueEntries[e];
                    if (conversation.id == conversationID && entry.id == oldID)
                    {
                        entry.id = newID;
                    }
                    for (int i = 0; i < entry.outgoingLinks.Count; i++)
                    {
                        var link = entry.outgoingLinks[i];
                        if (link.originConversationID == conversationID && link.originDialogueID == oldID) link.originDialogueID = newID;
                        if (link.destinationConversationID == conversationID && link.destinationDialogueID == oldID) link.destinationDialogueID = newID;
                    }
                }
            }
        }

    }

}
