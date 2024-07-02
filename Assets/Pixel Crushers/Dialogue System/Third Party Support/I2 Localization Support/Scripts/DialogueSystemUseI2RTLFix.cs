using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.I2Support
{

    /// <summary>
    /// If using Right-To-Left (RTL) languages, add this script to the Dialogue Manager to 
    /// make subtitles and response menus apply I2's RTL fix.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/I2 Localization Support/Dialogue System Use I2 RTL Fix")]
    public class DialogueSystemUseI2RTLFix : MonoBehaviour
    {

        [Tooltip("Extra language codes that I2 doesn't recognize as RTL.")]
        public List<string> additionalRTLLanguageCodes = new List<string>(new string[] { "fa" });

        [Tooltip("Use right side alignment for RTL text.")]
        public bool alignRTLToRight = true;

        [Tooltip("Insert new line after this many characters.")]
        public int textLimit = 999;

        /// <summary>
        /// For some reason I2 doesn't treat "fa" as RTL. This method does:
        /// </summary>
        bool IsRTL()
        {
            return I2.Loc.LocalizationManager.IsRight2Left || additionalRTLLanguageCodes.Contains(I2.Loc.LocalizationManager.CurrentLanguageCode);
        }

        /// <summary>
        /// Called when a conversation starts. Sets the subtitle panels to handle RTL text.
        /// </summary>
        void OnConversationStart(Transform actor)
        {
            var ui = DialogueManager.dialogueUI as StandardDialogueUI;
            if (ui != null)
            {
                foreach (var subtitlePanel in ui.conversationUIElements.subtitlePanels)
                {
                    if (subtitlePanel != null)
                    {
                        // Set subtitle alignment:
                        if (alignRTLToRight)
                        {
                            SetTextFieldAlignment(subtitlePanel.subtitleText);
                        }
                        // Set typewriter:
                        var typewriter = subtitlePanel.GetTypewriter();
                        if (typewriter != null)
                        {
                            typewriter.rightToLeft = IsRTL();
                        }
                    }
                }
            }
        }

        private void SetTextFieldAlignment(UITextField textField)
        {
            if (textField.uiText != null)
            {
                var currentAlignment = textField.uiText.alignment;
                switch (currentAlignment)
                {
                    case TextAnchor.LowerLeft:
                    case TextAnchor.LowerRight:
                        textField.uiText.alignment = IsRTL() ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
                        break;
                    case TextAnchor.MiddleLeft:
                    case TextAnchor.MiddleRight:
                        textField.uiText.alignment = IsRTL() ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
                        break;
                    case TextAnchor.UpperLeft:
                    case TextAnchor.UpperRight:
                        textField.uiText.alignment = IsRTL() ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
                        break;
                }
            }
#if TMP_PRESENT
            if (textField.textMeshProUGUI != null)
            {
                var currentAlignment = textField.textMeshProUGUI.alignment;
                switch (currentAlignment)
                {
                    case TMPro.TextAlignmentOptions.BaselineLeft:
                    case TMPro.TextAlignmentOptions.BaselineRight:
                        textField.textMeshProUGUI.alignment = IsRTL() ? TMPro.TextAlignmentOptions.BaselineRight : TMPro.TextAlignmentOptions.BaselineLeft;
                        break;
                    case TMPro.TextAlignmentOptions.BottomLeft:
                    case TMPro.TextAlignmentOptions.BottomRight:
                        textField.textMeshProUGUI.alignment = IsRTL() ? TMPro.TextAlignmentOptions.BottomRight : TMPro.TextAlignmentOptions.BottomLeft;
                        break;
                    case TMPro.TextAlignmentOptions.CaplineLeft:
                    case TMPro.TextAlignmentOptions.CaplineRight:
                        textField.textMeshProUGUI.alignment = IsRTL() ? TMPro.TextAlignmentOptions.CaplineRight : TMPro.TextAlignmentOptions.CaplineLeft;
                        break;
                    case TMPro.TextAlignmentOptions.Midline:
                    case TMPro.TextAlignmentOptions.MidlineLeft:
                        textField.textMeshProUGUI.alignment = IsRTL() ? TMPro.TextAlignmentOptions.MidlineRight : TMPro.TextAlignmentOptions.MidlineLeft;
                        break;
                    case TMPro.TextAlignmentOptions.Left:
                    case TMPro.TextAlignmentOptions.Right:
                        textField.textMeshProUGUI.alignment = IsRTL() ? TMPro.TextAlignmentOptions.Right : TMPro.TextAlignmentOptions.Left;
                        break;
                    case TMPro.TextAlignmentOptions.TopLeft:
                    case TMPro.TextAlignmentOptions.TopRight:
                        textField.textMeshProUGUI.alignment = IsRTL() ? TMPro.TextAlignmentOptions.TopRight : TMPro.TextAlignmentOptions.TopLeft;
                        break;
                }
            }
#endif
        }

        /// <summary>
        /// Called before a subtitle is displayed. Applies RTL fix if necessary.
        /// </summary>
        void OnConversationLine(Subtitle subtitle)
        {
            if (IsRTL())
            {
                subtitle.formattedText.text = I2.Loc.LocalizationManager.ApplyRTLfix(subtitle.formattedText.text, textLimit, false);
            }
        }

        /// <summary>
        /// Called before a response menu is displayed. Applies RTL fix if necessary.
        /// </summary>
        void OnConversationResponseMenu(Response[] responses)
        {
            if (IsRTL())
            {
                for (int i = 0; i < responses.Length; i++)
                {
                    responses[i].formattedText.text = I2.Loc.LocalizationManager.ApplyRTLfix(responses[i].formattedText.text, textLimit, false);
                }
            }
        }
    }
}