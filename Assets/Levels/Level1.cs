using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.ChatMapper;
using I2.Loc;
using TMPro;
using UnityEngine.Rendering.Universal;

public class Level1 : LevelController
{
    public GameObject Player;
    public Button continueButton;
    public GameObject openingMenu;
    public GameObject fallingCamera;
    public GameObject FirstConvoTrigger;
    public string[] conversationName;
    public string language;
    public GameObject airWall;
    public GameObject firstTrigger;
    public GameObject secondTrigger;
    public List<bool> conversationTracker;

    // Start is called before the first frame update
    void Awake()
    {
        if (!ES3.KeyExists("First Time"))
        {
            continueButton.interactable = false;
        }
        if (ES3.KeyExists("First Combat"))
        {
            if (ES3.Load<bool>("First Combat"))
            {
                secondTrigger.SetActive(false);
                ES3.DeleteKey("First Combat");
                Reload();
                openingMenu.SetActive(false);
                fallingCamera.SetActive(false);
                Player.SetActive(true);
            }
        }
        Player.GetComponent<GadgetsTool>().MagneticBoots = false;
        Player.GetComponent<GadgetsTool>().Camera = true;
    }

    // Update is called once per frame
    public override void GoToBattle()
    {
        base.GoToBattle();
    }

    public void NewGame()
    {
        ES3.Save("First Time", false);
        openingMenu.SetActive(false);
        flowchart.ExecuteBlock("CameraRotate");
        flowchart.ExecuteBlock("CameraDrop");
    }

    public void switchCamera()
    {
        fallingCamera.SetActive(false);
        Player.SetActive(true);
    }

    public void Continue()
    {
        openingMenu.SetActive(false);
        flowchart.ExecuteBlock("CameraRotate");
        flowchart.ExecuteBlock("CameraDrop");
        flowchart.ExecuteBlock("ContinueWhiteScreen");
    }

    public void StartPrologue()
    {
        JudgeLanguage();
        DialogueManager.StartConversation(conversationName[0] + "_" + language);
    }

    public void Sentence1()
    {
        JudgeLanguage();
        DialogueManager.StartConversation(conversationName[1] + "_" + language);
    }

    public void Sentence2()
    {
        JudgeLanguage();
        DialogueManager.StartConversation(conversationName[2] + "_" + language);
        Player.GetComponent<PlayerController>().movementSpeed = 0;
        flowchart.ExecuteBlock("StunAndMove");
        ES3.Save("First Combat", true);
        ThreeDTo2DData.ThreeDScene = "CentralMeditationRoom";
        Player.GetComponent<PlayerController>().Save();
    }

    public void Reload() //Player, Enemy, SpotLights
    {
        ES3.DeleteKey("First Combat");
        //Player
        if (ES3.KeyExists("InLevelPlayerPosition"))
        {
            Player.transform.position = ES3.Load<Vector3>("InLevelPlayerPosition");
        }
        if (ES3.KeyExists("InLevelPlayerRotation"))
        {
            Player.transform.rotation = ES3.Load<Quaternion>("InLevelPlayerRotation");
        }
    }

        void OnEnable()
    {
        // Subscribe to the conversation end event
        DialogueManager.Instance.conversationEnded += OnConversationEnd;
    }

    void OnDisable()
    {
        // Unsubscribe from the conversation end event
        DialogueManager.Instance.conversationEnded -= OnConversationEnd;
    }

    void OnConversationEnd(Transform actor)
    {
        if (!conversationTracker[0])
        {
            conversationTracker[0] = true;
            airWall.SetActive(false);
        }
        else if (!conversationTracker[1])
        {
            conversationTracker[1] = true;
            firstTrigger.SetActive(false);
            secondTrigger.SetActive(true);
        }
        else if (!conversationTracker[2])
        {
            secondTrigger.SetActive(false);
        }
    }


    void JudgeLanguage()
    {
        switch (LocalizationManager.CurrentLanguage)
        {
            case "English":
                language = "en";
                DialogueManager.displaySettings.subtitleSettings.subtitleCharsPerSecond = 200;
                break;
            case "Chinese (Simplified)":
                language = "cn";
                DialogueManager.displaySettings.subtitleSettings.subtitleCharsPerSecond = 100;
                break;
            // Add more cases for other languages if needed
            default:
                language = "en";
                break;
        }
    }
}
