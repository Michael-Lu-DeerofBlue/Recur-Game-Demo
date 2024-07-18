using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class AllowButtonAfterDelay : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Get or add a CanvasGroup component
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        // Make the button invisible and non-interactable
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Make the button interactable and visible after 1 second
        Invoke(nameof(AllowButton), 1f);
    }

    private void OnConversationLine(Subtitle subtitle)
    {
        // Make the button invisible and non-interactable
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Make the button interactable and visible after 1 second
        Invoke(nameof(AllowButton), 1f);
    }

    void AllowButton()
    {
        // Make the button visible and interactable
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
