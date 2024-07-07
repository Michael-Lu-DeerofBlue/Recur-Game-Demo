// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using PixelCrushers.DialogueSystem;

// public class DialogueSetLanguage : MonoBehaviour {
//     public string newLanguage;
//     void Start()
//     {
//         DialogueManager.instance.SetLanguage(newLanguage);
//         UpdateCharacterNames();
//         UpdateSubtitleText();
//         DialogueManager.UpdateResponses();
//     }

//     public void UpdateSubtitleText()
//     {
//         var subtitle = DialogueManager.currentConversationState.subtitle;
//         subtitle.formattedText.text = FormattedText.Parse(subtitle.dialogueEntry.currentDialogueText);
//         DialogueManager.standardUISubtitlePanel.ShowSubtitle(subtitle);
//         // (^ Alternatively you could just set the subtitle panel's subtitleText.text.)
//     }

//     public void UpdateCharacterNames()
//     {
//         var conversation = DialogueManager.masterDatabase.GetConversation(DialogueManager.currentConversationState.subtitle.dialogueEntry.conversationID);
//         var actorIDs = new HashSet<int>();
//         conversation.dialogueEntries.ForEach(entry => actorIDs.Add(entry.ActorID));
//         foreach (var actorID in actorIDs)
//         {
//             var characterInfo = DialogueManager.conversationModel.GetCharacterInfo(actorID);
//             characterInfo.Name = PixelCrushers.DialogueSystem.CharacterInfo.GetLocalizedDisplayNameInDatabase(characterInfo.nameInDatabase);
//         }
//     }
// }