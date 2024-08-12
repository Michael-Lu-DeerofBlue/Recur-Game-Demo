using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.ChatMapper;
using I2.Loc;
using TMPro;
using Fungus;
using UnityEngine.SceneManagement;

public class HintController : MonoBehaviour
{
    public List<GameObject> gameObjects;
    public int currentIndex = 0;
    public int lineCleared = 0;
    public string language;
    public string conversationName;
    public TextMeshPro countText;
    public Flowchart gameFlowchart;
    public Flowchart BGMFlowchart;
    public GameObject perTutorialManager;
    void Start()
    {
        // Ensure all game objects are disabled except the first one
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].SetActive(i == 0);
        }
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "BattleLevel - tutorial")
        {
            //Handle To Tutorial
        }
    }
        private void Update()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName == "BattleLevel - tutorial")
            {
                countText.text = lineCleared.ToString() + "/3";
            }
        }

        public void ExitOut()
        {
            JudgeLanguage();
            DialogueManager.StartConversation(conversationName + "_" + language);
            BGMFlowchart.ExecuteBlock("FadeToCalm"); //fade music to calm ver.
            gameFlowchart.ExecuteBlock("WhiteScreen");
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

        public void WaitAndSwitch()
        {
            gameFlowchart.ExecuteBlock("WaitAndSwitch");
        }


        public void SwitchTip()
        {
            if (gameObjects == null || gameObjects.Count == 0)
                return;
            //Debug.Log(gameObjects.Count);
            string currentSceneName = SceneManager.GetActiveScene().name;
            //Per tutorial
            if (currentSceneName == "BattleLevel - per - tutorial")
            {
                if (currentIndex < gameObjects.Count - 1)
                {
                    // Disable the current game object
                    gameObjects[currentIndex].SetActive(false);

                    // Increment the index and wrap around if needed
                    currentIndex = (currentIndex + 1);
                    if (currentIndex == 2)
                    {
                        perTutorialManager.GetComponent<PerseusTutorial>().Resume();
                        gameFlowchart.ExecuteBlock("WaitAndPause");
                    }
                    // Enable the next game object
                    gameObjects[currentIndex].SetActive(true);
                }
                else
                {
                    foreach (GameObject go in gameObjects) { go.SetActive(false); }
                }
            }



            //Tutorial
            if (currentSceneName == "BattleLevel - tutorial")
            {
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

        //Board
        if (currentSceneName == "Air Island Jungle")
        {
            if (currentIndex < gameObjects.Count - 1)
            {
                // Disable the current game object
                gameObjects[currentIndex].SetActive(false);

                // Increment the index and wrap around if needed
                currentIndex = (currentIndex + 1);
               
                // Enable the next game object
                gameObjects[currentIndex].SetActive(true);
            }
            else
            {
                foreach (GameObject go in gameObjects) { go.SetActive(false); }
            }
        }


    }
}
