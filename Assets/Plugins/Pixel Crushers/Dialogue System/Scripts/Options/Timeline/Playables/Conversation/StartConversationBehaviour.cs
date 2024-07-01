// Recompile at 7/1/2024 8:27:26 AM
#if USE_TIMELINE
#if UNITY_2017_1_OR_NEWER
// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Playables;
using System;

namespace PixelCrushers.DialogueSystem
{

    [Serializable]
    public class StartConversationBehaviour : PlayableBehaviour
    {

        [Tooltip("(Optional) The other participant.")]
        public Transform conversant;

        [Tooltip("The conversation to start.")]
        [ConversationPopup(true)]
        public string conversation;

        [Tooltip("Jump to a specific dialogue entry instead of starting from the conversation's START node.")]
        public bool jumpToSpecificEntry;

        [Tooltip("Dialogue entry to jump to.")]
        public int entryID;

        [Tooltip("Stop any active conversations before starting this one.")]
        public bool exclusive = false;

        [Tooltip("(Optional) Assign if you want to override dialogue UI for this conversation.")]
        public AbstractDialogueUI overrideDialogueUI;

        public string GetEditorDialogueText()
        {
            var dialogueText = PreviewUI.GetDialogueText(conversation, jumpToSpecificEntry ? entryID : -1);
            return "'" + dialogueText + "'";
        }

    }
}
#endif
#endif
