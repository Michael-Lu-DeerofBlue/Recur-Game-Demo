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
    public Flowchart gameFlowchart;
    private SingletonFlowchart bgmFlowchart;
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
    public GameObject anotherWhiteScreen;
    public GameObject openingSettingMenu;
    public GameObject gameplaySettingMenu;
    public bool convoThreeDone;
    public bool convoFourthDone;
    public GameObject goggleCanvas;
    public GameObject EscKey;
    public TextMeshProUGUI hint;
    public Dictionary<string, int> ConsumablesInventory = new Dictionary<string, int>()
    {
        { "MedKit", 2 },
        { "SprayCan", 2 },
        { "Mint", 2 },
        { "PaperCutter", 2 },
        { "FracturedPocketWatch", 2 }
    };

    public Dictionary<string, int> StickersInventory = new Dictionary<string, int>()
    {
        { "Critical", 2 },
        { "Pierce", 2 },
        { "Sober", 2 },
        { "Swordmaster", 2 },
        { "Gunslinger", 2 }
    };

    public List<StickerInfo.StickerData> stickerData = new List<StickerInfo.StickerData>();
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
        
        bgmFlowchart = SingletonFlowchart.Instance; //access the singleton bgm flowchart in the scene
        if (bgmFlowchart == null)
        {
            Debug.LogWarning("BGM flowchart not found!");
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
        if (convoThreeDone)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                hint.text = "";
                fourthTrigger.SetActive(true);
                convoThreeDone = false;
            }
        }
        if (convoFourthDone)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                hint.text = "";
            }
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
        ES3.Save("Consumables", ConsumablesInventory);
        ES3.Save("Stickers", StickersInventory);
        ES3.Save("StickerData", stickerData);
        openingMenu.SetActive(false);
        gameFlowchart.ExecuteBlock("CameraRotate");
        gameFlowchart.ExecuteBlock("CameraDrop");
        bgmFlowchart.ExecuteBlock("MainMusicLoop"); //Start to play the main bgm
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
        gameFlowchart.ExecuteBlock("CameraRotate");
        gameFlowchart.ExecuteBlock("CameraDrop");
        gameFlowchart.ExecuteBlock("ContinueWhiteScreen");
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
        gameFlowchart.ExecuteBlock("StunAndMove");
        ES3.Save("First Combat", true);
        ThreeDTo2DData.ThreeDScene = "CentralMeditationRoom";
        Player.GetComponent<PlayerController>().Save();
        Debug.Log("playing tutorial music (sentence 2)");
        bgmFlowchart.ExecuteBlock("TutorialMusicLoop");
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
        perseus = true;
        anotherWhiteScreen.SetActive(false);
        EscKey.SetActive(true);
        goggleCanvas.SetActive(true);
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
        gameFlowchart.ExecuteBlock("NoWhiteScreen");
        Debug.Log("playing music again on reload");
        bgmFlowchart.ExecuteBlock("MainMusicLoop");
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
        // Access the current conversation's name
        int conversationName = DialogueManager.Instance.currentConversationState.subtitle.dialogueEntry.conversationID;
        // Debug log or use the conversation name as needed
        //Debug.Log("Conversation ended: " + conversationName.ToString());

        if (!conversationTracker[0])
        {
            //Debug.Log("Conversation ende");
            conversationTracker[0] = true;
            airWall.SetActive(false);
            gameFlowchart.ExecuteBlock("FenceDown");
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
            whiteScreen.SetActive(true);
            ES3.Save("Gadgets", true);
            convoThreeDone = true;
            JudgeLanguage();
            string hintText = "用TAB打开背包";
            if (language == "en") {
                hintText = "use Tab to open your pilot hatch";
            }
            hint.text = hintText;
        }
        else if (!conversationTracker[4])
        {
            conversationTracker[4] = true;
            Player.GetComponent<GadgetsTool>().Camera = true;
            JudgeLanguage();
            string hintText = "按E进入拍摄模式，对敌人按左键以拍摄并进入战斗";
            if (language == "en")
            {
                hintText = "use E to enter camera mode, left click to photo and combat the enemy";
            }
            hint.text = hintText;
        }
    }


    void JudgeLanguage()
    {
        switch (LocalizationManager.CurrentLanguage)
        {
            case "English":
                language = "en";
                //set subtitle speed
                DialogueManager.displaySettings.subtitleSettings.subtitleCharsPerSecond = 1500;
                break;
            case "Chinese (Simplified)":
                language = "cn";
                //set subtitle speed
                DialogueManager.displaySettings.subtitleSettings.subtitleCharsPerSecond = 700;
                break;
            // Add more cases for other languages if needed
            default:
                language = "en";
                break;
        }
    }

    public void CursorUnlock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
