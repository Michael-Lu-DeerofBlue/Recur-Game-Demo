using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.ChatMapper;
using I2.Loc;
using TMPro;
using Fungus;

public class HintController : MonoBehaviour
{
    public List<GameObject> gameObjects;
    public int currentIndex = 0;
    public int lineCleared = 0;
    public string language;
    public string conversationName;
    public TextMeshPro countText;
    public Flowchart flowchart;
    void Start()
    {
        // Ensure all game objects are disabled except the first one
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].SetActive(i == 0);
        }
    }

    private void Update()
    {
        countText.text = lineCleared.ToString() + "/5";
    }

    public void ExitOut()
    {
        JudgeLanguage();
        DialogueManager.StartConversation(conversationName + "_" + language);
        flowchart.ExecuteBlock("WhiteScreen");
    }

    void JudgeLanguage()
    {
        switch (LocalizationManager.CurrentLanguage)
        {
            case "English":
                language = "en";
                //set subtitle speed
                DialogueManager.displaySettings.subtitleSettings.subtitleCharsPerSecond = 10;
                break;
            case "Chinese (Simplified)":
                language = "cn";
                //set subtitle speed
                DialogueManager.displaySettings.subtitleSettings.subtitleCharsPerSecond = 4;
                break;
            // Add more cases for other languages if needed
            default:
                language = "en";
                break;
        }
    }


    public void SwitchTip()
    {
        if (gameObjects == null || gameObjects.Count == 0)
            return;
        //Debug.Log(gameObjects.Count);
        if (currentIndex < gameObjects.Count - 1)
        {
            // Disable the current game object
            gameObjects[currentIndex].SetActive(false);

            // Increment the index and wrap around if needed
            currentIndex = (currentIndex + 1);
            if (currentIndex == 7)
            {
                countText.gameObject.SetActive(true);
            }

            // Enable the next game object
            gameObjects[currentIndex].SetActive(true);
        }
        else
        {
            foreach (GameObject go in gameObjects) { go.SetActive(false); }
        }
        
    }
}
