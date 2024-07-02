// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.I2Support
{

    /// <summary>
    /// Sets the Dialogue System's language to match I2 Localization's language. 
    /// Add to the Dialogue Manager.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/I2 Localization Support/Dialogue System Use I2 Language")]
    public class DialogueSystemUseI2Language : MonoBehaviour
    {

        public enum AssetIdentifierType { ID, Name }

        public AssetIdentifierType assetsUse = AssetIdentifierType.ID;

        [Tooltip("Specifies whether fields in the Dialogue System are named by language code or language name.")]
        public I2LanguageIdentifierType specifyLanguageBy = I2LanguageIdentifierType.LanguageCode;

        public enum DialogueEntryInfo { None, Actor, Text }

        [Tooltip("Extra info included at end of I2 term.")]
        public DialogueEntryInfo extraEntryInfo = DialogueEntryInfo.None;

        public int dialogueEntryMinDigits = 1;

        public bool useCustomFieldForEntries = false;

        public string customFieldTitle = "Articy Id";

        [Tooltip("On start, set the Dialogue System's current language to match I2 Localization's current language.")]
        public bool useI2LanguageOnStart = true;

        [HelpBox("Use I2 Language At Runtime will bypass the dialogue database and read directly from i2 asset for dialogue text and menu text.", HelpBoxMessageType.Info)]
        [Tooltip("Bypass dialogue database and read directly from i2 asset for dialogue text and menu text.")]
        public bool useI2LanguageAtRuntime = false;

        [Tooltip("If Use I2 Language At Runtime is ticked, update actor Display Names at start. May cause small delay in starting conversation. If unticked, Display Names are updated in Start() and manually by calling UpdateActorDisplayNames.")]
        public bool updateActorDisplayNamesOnConversationStart = false;

        protected virtual void Start()
        {
            I2.Loc.LocalizationManager.SetLanguageAndCode("French", "fr");
            UseCurrentI2Language();
        }

        /// <summary>
        /// Updates the Dialogue System's current language setting to match i2.
        /// </summary>
        public virtual void UseCurrentI2Language()
        {
            var language = (specifyLanguageBy == I2LanguageIdentifierType.LanguageCode) ? I2.Loc.LocalizationManager.CurrentLanguageCode : I2.Loc.LocalizationManager.CurrentLanguage;
            DialogueManager.SetLanguage(language);
            UpdateActorDisplayNames();
        }

        /// <summary>
        /// Update Display Name fields with values in i2.
        /// </summary>
        public virtual void UpdateActorDisplayNames()
        {
            if (!useI2LanguageAtRuntime) return;
            foreach (var actor in DialogueManager.masterDatabase.actors)
            {
                var term = $"Dialogue System/Actor/{actor.Name}/Display Name";
                term = I2.Loc.I2Utils.GetValidTermName(term, true);
                var translation = I2.Loc.LocalizationManager.GetTermTranslation(term, true, 0, true, false, null, I2.Loc.LocalizationManager.CurrentLanguage);
                if (string.IsNullOrEmpty(translation)) continue;
                DialogueLua.SetActorField(actor.Name, "Display Name", translation);
                DialogueLua.SetActorField(actor.Name, $"Display Name {I2.Loc.LocalizationManager.CurrentLanguageCode}", translation);
            }
        }

        protected virtual void OnConversationStart(Transform actor)
        {
            if (updateActorDisplayNamesOnConversationStart) UpdateActorDisplayNames();
        }

        /// <summary>
        /// If useI2LanguageAtRuntime is true, replaces the subtitle's formatted text with
        /// its i2 runtime translation.
        /// </summary>
        protected virtual void OnConversationLine(Subtitle subtitle)
        {
            if (!useI2LanguageAtRuntime || subtitle == null) return;
            var entry = subtitle.dialogueEntry;
            var term = GetDialogueEntryHeader(entry);
            if (!string.IsNullOrEmpty(entry.DialogueText))
            {
                term += "/Dialogue Text";
            }
            else if (!string.IsNullOrEmpty(entry.MenuText))
            {
                term += "/Menu Text";
            }
            else
            {
                return;
            }
            term = I2.Loc.I2Utils.GetValidTermName(term, true);
            var translation = I2.Loc.LocalizationManager.GetTermTranslation(term, true, 0, true, false, null, I2.Loc.LocalizationManager.CurrentLanguage);
            if (!(string.IsNullOrEmpty(translation) || string.Equals(translation, "Null")))
            {
                subtitle.formattedText = FormattedText.Parse(translation);
            }
        }

        /// <summary>
        /// If useI2LanguageAtRuntime is true, replaces the responses' formatted text with
        /// their i2 runtime translations.
        /// </summary>
        protected virtual void OnConversationResponseMenu(Response[] responses)
        {
            if (!useI2LanguageAtRuntime || responses == null) return;
            var emTagForInvalidResponses = DialogueManager.displaySettings.inputSettings.emTagForInvalidResponses;
            var emTagForOldResponses = DialogueManager.displaySettings.inputSettings.emTagForOldResponses;
            var checkEmTags = emTagForInvalidResponses != EmTag.None || emTagForOldResponses != EmTag.None;
            for (int i = 0; i < responses.Length; i++)
            {
                var response = responses[i];
                var entry = response.destinationEntry;
                var term = GetDialogueEntryHeader(entry);
                if (!string.IsNullOrEmpty(entry.MenuText))
                {
                    term += "/Menu Text";
                }
                else if (!string.IsNullOrEmpty(entry.DialogueText))
                {
                    term += "/Dialogue Text";
                }
                else
                {
                    continue;
                }
                term = I2.Loc.I2Utils.GetValidTermName(term, true);
                var translation = I2.Loc.LocalizationManager.GetTermTranslation(term, true, 0, true, false, null, I2.Loc.LocalizationManager.CurrentLanguage);
                if (!(string.IsNullOrEmpty(translation) || string.Equals(translation, "Null")))
                {
                    if (emTagForInvalidResponses != EmTag.None && !response.enabled)
                    {
                        var invalidTagNum = (int)emTagForInvalidResponses;
                        translation = $"[em{invalidTagNum}]{translation}[/em{invalidTagNum}]";
                    }
                    if (emTagForOldResponses != EmTag.None && DialogueLua.GetSimStatus(response.destinationEntry) == DialogueLua.WasDisplayed)
                    {
                        var oldTagNum = (int)emTagForOldResponses;
                        translation = $"[em{oldTagNum}]{translation}[/em{oldTagNum}]";
                    }
                    response.formattedText = FormattedText.Parse(translation);
                }
            }
        }

        protected string GetDialogueEntryHeader(DialogueEntry entry)
        {
            if (entry == null) return string.Empty;
            if (useCustomFieldForEntries)
            {
                var fieldValue = Field.LookupValue(entry.fields, customFieldTitle);
                if (string.IsNullOrEmpty(fieldValue))
                {
                    Debug.LogWarning($"Dialogue System: Entry {entry.conversationID}:{entry.id} '{entry.DialogueText}' doesn't have a field '{customFieldTitle}'");
                }
                else
                {
                    return $"Dialogue System/{fieldValue}";
                }
            }
            var header = "Dialogue System/Conversation/";
            header += (assetsUse == AssetIdentifierType.ID)
                ? entry.conversationID.ToString("D" + dialogueEntryMinDigits)
                : SanitizedConversationTitle(entry.conversationID);
            //---Was, prior to BAN edit: header +=  "/Entry/" + entry.id.ToString("D" + dialogueEntryMinDigits);
            var id = entry.id.ToString("D" + dialogueEntryMinDigits);
            if (useCustomFieldForEntries)
            {
                var fieldValue = Field.LookupValue(entry.fields, customFieldTitle);
                if (string.IsNullOrEmpty(fieldValue))
                {
                    Debug.LogWarning($"Dialogue System: Entry {entry.conversationID}:{entry.id} '{entry.DialogueText}' doesn't have a field '{customFieldTitle}'");
                }
                else
                {
                    id = fieldValue;
                }
            }
            header += "/Entry/" + id;
            switch (extraEntryInfo)
            {
                case DialogueEntryInfo.Actor:
                    var actor = DialogueManager.masterDatabase.GetActor(entry.ActorID);
                    header += "/" + ((actor != null) ? actor.Name : string.Empty);
                    break;
                case DialogueEntryInfo.Text:
                    header += "/" + entry.DialogueText;
                    break;
            }
            return header;
        }

        protected string SanitizedConversationTitle(int conversationID)
        {
            var conversation = DialogueManager.masterDatabase.GetConversation(conversationID);
            return (conversation != null) ? SanitizeTerm(conversation.Title) : conversationID.ToString("D" + dialogueEntryMinDigits);
        }

        protected string SanitizeTerm(string s)
        {
            return string.IsNullOrEmpty(s) ? s : I2.Loc.I2Utils.GetValidTermName(s.Replace("/", ".").Replace("\\", ".").Replace("[", ".").Replace("]", "."));
        }

    }
}