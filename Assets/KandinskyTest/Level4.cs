using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using I2.Loc;
using PixelCrushers.DialogueSystem;

public class Level4 : MonoBehaviour
{
    public Flowchart flowchart;
    public GameObject Player;
    public Transform reference;
    public string language;
    public string[] conversationName;
    public int ch_sub_speed;
    public int en_sub_speed;
    private Queue<string> conversationQueue = new Queue<string>();
    private bool isConversationRunning = false;
    private void Awake()
    {
        Player.transform.position = reference.position;
    }
    // Start is called before the first frame update
    public void ThankYou()
    {
        flowchart.ExecuteBlock("End");
    }

    public void Sentence1() //must possess key
    {
        JudgeLanguage();
        string conversation = conversationName[0] + "_" + language;
        if (isConversationRunning)
        {
            conversationQueue.Enqueue(conversation);
        }
        else
        {
            StartConversation(conversation);
        }
    }

    private void StartConversation(string conversation)
    {
        isConversationRunning = true;
        //Debug.Log("Starting conversation: " + conversation);
        DialogueManager.StartConversation(conversation);
    }

    void JudgeLanguage()
    {
        switch (LocalizationManager.CurrentLanguage)
        {
            case "English":
                language = "en";
                //set subtitle speed
                DialogueManager.displaySettings.subtitleSettings.subtitleCharsPerSecond = en_sub_speed;
                break;
            case "Chinese (Simplified)":
                language = "cn";
                //set subtitle speed
                DialogueManager.displaySettings.subtitleSettings.subtitleCharsPerSecond = ch_sub_speed;
                break;
            // Add more cases for other languages if needed
            default:
                language = "en";
                break;
        }
    }
}
