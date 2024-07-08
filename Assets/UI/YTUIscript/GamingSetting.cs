using Fungus;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Kamgam.UGUIComponentsForSettings.SelectIfNull;
public class GamingSetting : MonoBehaviour
{
    public Button[] Buttons;
    public float alphaThreshold = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Button button in Buttons)
        {
            // Set alphaHitTestMinimumThreshold for each button's image
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.alphaHitTestMinimumThreshold = alphaThreshold;
            }
            SetChildImagesAlpha(button, 0f);

            // Add EventTrigger to handle hover events
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { OnHoverEnter(button); });
            trigger.triggers.Add(entryEnter);

            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { OnHoverExit(button); });
            trigger.triggers.Add(entryExit);
        }
    }

    void OnHoverEnter(Button button)
    {
        SetChildImagesAlpha(button, 1f); 
    }

    void OnHoverExit(Button button)
    {
        SetChildImagesAlpha(button, 0f); 
    }

    void SetChildImagesAlpha(Button button, float alpha)
    {
        Image[] images = button.GetComponentsInChildren<Image>(true);
        foreach (Image img in images)
        {
            if (img != button.GetComponent<Image>()) 
            {
                Color color = img.color;
                color.a = alpha;
                img.color = color;
            }
        }
    }
}