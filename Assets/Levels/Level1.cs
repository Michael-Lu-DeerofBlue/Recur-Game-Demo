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
    public string[] conversationName;
    public string language;
    public GameObject airWall;
    public GameObject firstTrigger;
    public GameObject secondTrigger;
    public GameObject fourthTrigger;
    public List<bool> conversationTracker;
    public GameObject whiteScreen;
    public GameObject openingSettingMenu;
    public GameObject gameplaySettingMenu;

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
                ES3.DeleteKey("First Combat");
                secondTrigger.SetActive(false);
                Reload();
            }
        }
        Player.GetComponent<GadgetsTool>().MagneticBoots = false;
        Player.GetComponent<GadgetsTool>().Camera = false;
    }

    // Update is called once per frame
    public override void GoToBattle()
    {
        base.GoToBattle();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameplaySettingMenu.SetActive(false);
        }
    }

    public void NewGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ES3.Save("First Time", false);
        ES3.Save("MoveHP", 2);
        float number = 100;
        ES3.Save("Gadgets", false);
        ES3.Save("CombatHP", number);
        ES3.Save("Longsword", false);
        ES3.Save("Sprint", false);
        ES3.Save("Flashlight", false);
        openingMenu.SetActive(false);
        flowchart.ExecuteBlock("CameraRotate");
        flowchart.ExecuteBlock("CameraDrop");
        
    }

    public void OpenSetting()
    {
        gameplaySettingMenu.SetActive(true);
    }

    public void switchCamera()
    {
        fallingCamera.SetActive(false);
        Player.SetActive(true);
    }

    public void Continue()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ES3.Save("First Combat", true);
        ThreeDTo2DData.ThreeDScene = "CentralMeditationRoom";
        Player.GetComponent<PlayerController>().Save();
    }

    public void Sentence3()
    {
        JudgeLanguage();
        conversationTracker[0] = true;
        conversationTracker[1] = true;
        conversationTracker[2] = true;
        DialogueManager.StartConversation(conversationName[3] + "_" + language);
    }
    public void Sentence4()
    {
        JudgeLanguage();
        DialogueManager.StartConversation(conversationName[4] + "_" + language);
        
    }

    public void Reload() //Player, Enemy, SpotLights
    {
        openingMenu.SetActive(false);
        fallingCamera.SetActive(false);
        Player.SetActive(true);
        whiteScreen.SetActive(false);
        firstTrigger.SetActive(false);
        secondTrigger.SetActive(false);
        airWall.SetActive(false);
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
        flowchart.ExecuteBlock("NoWhiteScreen");
        Sentence3();
    }

        void OnEnable()
    {
        // Subscribe to the conversation end event
        DialogueManager.Instance.conversationEnded += OnConversationEnd;
    }

    void OnDisable()
    {
        // Unsubscribe from the conversation end event
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.conversationEnded -= OnConversationEnd;
        }
        
    }

    void OnConversationEnd(Transform actor)
    {
        if (!conversationTracker[0])
        {
            conversationTracker[0] = true;
            airWall.SetActive(false);
            flowchart.ExecuteBlock("FenceDown");
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
        else if (!conversationTracker[3])
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            conversationTracker[3] = true;
            fourthTrigger.SetActive(true);
            whiteScreen.SetActive(true);
            ES3.Save("Gadgets", true);
        }
        else if (!conversationTracker[4])
        {
            conversationTracker[4] = true;
            Player.GetComponent<GadgetsTool>().Camera = true; 
            
        }
    }


    void JudgeLanguage()
    {
        switch (LocalizationManager.CurrentLanguage)
        {
            case "English":
                language = "en";
                //set subtitle speed
                DialogueManager.displaySettings.subtitleSettings.subtitleCharsPerSecond = 20;
                break;
            case "Chinese (Simplified)":
                language = "cn";
                //set subtitle speed
                DialogueManager.displaySettings.subtitleSettings.subtitleCharsPerSecond = 10;
                break;
            // Add more cases for other languages if needed
            default:
                language = "en";
                break;
        }
    }
}
