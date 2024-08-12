using IndieMarc.EnemyVision;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Fungus;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using I2.Loc;
using PixelCrushers.DialogueSystem;
using Unity.Burst.CompilerServices;
using System;
using TMPro;

public class Level2 : LevelController
{
    public GameObject Player;
    public List<GameObject> activeSpotlights;
    public List<Transform> enemies;
    public GameObject spotlightManager;
    public bool playerShined;
    public int randomIndex;
    public float duration = 5.0f; // Duration over which the light will decrease
    public GameObject LevelController;
    public GameObject firstTrigger;
    public GameObject secondTrigger;
    public GameObject thirdTrigger;
    public float targetExposure = -2.0f;
    public GameObject key;
    public Volume volume;
    private ColorAdjustments colorAdjustments;
    public string language;
    public string[] conversationName;
    public int ch_sub_speed;
    public int en_sub_speed;
    private Queue<string> conversationQueue = new Queue<string>();
    private bool isConversationRunning = false;
    public int pursuitSpeed;
    public float patrolSpeed;
    public bool inPursuitCalled;
    public string[] hints;
    public int hintsCurrentIndex;
    public TextMeshProUGUI hint;
    public bool reloaded;
    public GameObject fakeEnemy;
    void Awake()
    {
        ES3.Save("MoveHP", 2);
        ES3.Save("Sprint", true);
        colorAdjustments = (ColorAdjustments)volume.profile.components.Find(x => x is ColorAdjustments);
        Player.GetComponent<GadgetsTool>().MagneticBoots = false;
        if (ThreeDTo2DData.ThreeDScene != null)
        {
            ThreeDTo2DData.ThreeDScene = null;
            Reload();
        }
        else
        {
            firstTrigger.SetActive(true);
            secondTrigger.SetActive(true);
            SetRandomEnemyHasKey();
        }
    }

    private void Update()
    {
        activeSpotlights = spotlightManager.GetComponent<SpotlightManager>().activeSpotlights;
        playerShined = false;
        foreach (GameObject spotlight in activeSpotlights)
        {
            playerShined = spotlight.GetComponent<SpotlightDetection>().PlayerInSpotLight();
            //Debug.Log(playerShined);
            if (playerShined)
            {
                Scene2E();
                ResetEnemyTarget(Player.transform);
                break;
            }
        }
        foreach (GameObject spotlight in activeSpotlights)
        {
            foreach (Transform enemy in enemies)
            {
                bool enemyShined = spotlight.GetComponent<SpotlightDetection>().EnemyInSpotLight(enemy);
                if (enemyShined)
                {
                    Scene2F(); //play dialogue
                    enemy.GetComponent<ThreeEnemyBase>().ChangeMaterial();
                }
            }
        }

    }

    private void Start()
    {
        JudgeLanguage();
        flowchart.ExecuteBlock("StartConvo"); //initial conversation
      
    }

    IEnumerator GraduallyChangeExposure(float initialExposure)
    {

        float elapsedTime = 0.0f;
        Debug.Log(initialExposure);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpFactor = elapsedTime / duration;

            // Gradually change the post exposure
            colorAdjustments.postExposure.value = Mathf.Lerp(initialExposure, targetExposure, lerpFactor);

            yield return null;
        }

        // Ensure the final exposure is set
        colorAdjustments.postExposure.value = targetExposure;
    }

    public void ResetEnemyTarget(Transform target)
    {
        if (!inPursuitCalled)
        {
            inPursuitCalled = true;
            foreach (Transform enemy in enemies)
            {
                int random = UnityEngine.Random.Range(1, 10);
                if (random <= 4)
                {
                    if (enemy.gameObject.activeSelf)
                    {
                        enemy.GetComponent<ThreeEnemyBase>().inPursuit(target);
                        enemy.GetComponent<AIPath>().maxSpeed = pursuitSpeed;
                    }
                }

            }
            StartCoroutine(SetBooleanFalseAfterDelay(5f));
        }
       
    }

    public void ResetAllEnemyTarget(Transform target)
    {
        foreach (Transform enemy in enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                enemy.GetComponent<ThreeEnemyBase>().inBell(target);
                enemy.GetComponent<AIPath>().maxSpeed = pursuitSpeed;
            }
        }

    }

    public void GoToGallery1()
    {
        flowchart.ExecuteBlock("GoToGallery");
    }

    public void BackToNormal()
    {
        foreach(Transform enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
        }
        spotlightManager.GetComponent<SpotlightManager>().TurnOffAllSpotlight();
        fakeEnemy.SetActive(true);
        targetExposure = 0.73f;
        StartCoroutine(GraduallyChangeExposure(-2.5f));
    }

    public void ColdStart()
    {
        StartCoroutine(GraduallyChangeExposure(0.73f));
        Player.GetComponent<PlayerInteract>().canInteract = true;
        Player.GetComponent<GadgetsTool>().Camera = true;
        flowchart.ExecuteBlock("ColdStart");
    }

    public void StartTheLights()
    {
        spotlightManager.GetComponent<SpotlightManager>().enabled = true;
    }

    public void StartTheEnemy()
    {
        foreach (Transform enemy in enemies)
        {
            enemy.GetComponent<ThreeEnemyBase>().enabled = true;
            enemy.GetComponent<Patrol>().enabled = true;
        }
    }

    public void StartTheSight()
    {
        foreach (Transform enemy in enemies)
        {
            enemy.GetComponent<EnemyFOV>().enabled = true;
        }
    }


    // Update is called once per frame
    public override void GoToBattle()
    {
        Save();
        base.GoToBattle();
    }

    public void Reload() //Player, Enemy, SpotLights
    {
        reloaded = true;
        secondTrigger.SetActive(true); //key reminder
        //Exposure
        colorAdjustments.postExposure.value = targetExposure;

        //Player
        if (ES3.KeyExists("InLevelPlayerPosition"))
        {
            Player.transform.position = ES3.Load<Vector3>("InLevelPlayerPosition");
        }
        if (ES3.KeyExists("InLevelPlayerRotation"))
        {
            Player.transform.rotation = ES3.Load<Quaternion>("InLevelPlayerRotation");
        }

        //Enemy
        randomIndex = ES3.Load<int>("EnemyKeyIndex");
        StartTheEnemy();
        StartTheSight();
        foreach (Transform enemy in enemies)
        {
            enemy.position = ES3.Load<Vector3>(enemy.name + " Position");
            enemy.rotation = ES3.Load<Quaternion>(enemy.name + " Rotation");
        }
        ThreeEnemyBase enemyScript = enemies[randomIndex].GetComponent<ThreeEnemyBase>();
        enemyScript.hasKey = true;
        PlayEffectKey();

        //Spotlights
        StartTheLights();
        spotlightManager.GetComponent<SpotlightManager>().Resume();

        //PlayerStats & EnemyStats
        if (TwoDto3D.win  == true)
        {
            
            foreach (var key in ThreeDTo2DData.dataDictionary.Keys)
            {
                Debug.Log(key);
                GameObject obj = GameObject.Find(key);
                if (obj != null)
                {Debug.Log("Here");
                    obj.SetActive(false);
                }
            }
        }
        else
        {
            ResetLevel();
        }

        //Key
        if (!enemies[randomIndex].gameObject.activeSelf)
        {
            key.GetComponent<Door>().Keyed = true;
        }

        //Player Interact
        Player.GetComponent<PlayerInteract>().canInteract = true;

        //close door
        flowchart.ExecuteBlock("CloseDoor");
    }

    public void ResetLevel()
    {
        ClearInGameSaveData();
        StartCoroutine(PauseAndReloadScene());
    }
    IEnumerator PauseAndReloadScene()
    {
        // Pause the game by setting the time scale to 0
        Time.timeScale = 0f;

        // Wait for 5 real-time seconds
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ClearInGameSaveData()
    {
        foreach (Transform enemy in enemies)
        {
            ES3.DeleteKey(enemy.name + " Position");
            ES3.DeleteKey(enemy.name + " Rotation");
        }
        ES3.DeleteKey("InLevelPlayerPosition");
        ES3.DeleteKey("InLevelPlayerRotation");
        ES3.DeleteKey("EnemyKeyIndex");
        ES3.Save("MoveHP",2);
    }

    public void Save()
    {
        //Player
        Player.GetComponent<PlayerController>().Save();

        //Enemy
        foreach (Transform enemy  in enemies)
        {
            ES3.Save(enemy.name + " Position", enemy.position);
            ES3.Save(enemy.name + " Rotation", enemy.rotation);
        }
        ES3.Save("EnemyKeyIndex", randomIndex);

        //Spotlights
        spotlightManager.GetComponent<SpotlightManager>().Pause();
    }

    void SetRandomEnemyHasKey()
    {
        if (enemies.Count == 0)
        {
            Debug.LogWarning("No enemies in the list.");
            return;
        }

        randomIndex = UnityEngine.Random.Range(0, enemies.Count);
        ThreeEnemyBase enemyScript = enemies[randomIndex].GetComponent<ThreeEnemyBase>();

        if (enemyScript != null)
        {
            enemyScript.hasKey = true;
            Debug.Log("Enemy at index " + randomIndex + " now has the key.");
        }
        else
        {
            Debug.LogError("Enemy at index " + randomIndex + " does not have an Enemy script.");
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
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.conversationEnded -= OnConversationEnd;
        }

    }

    void OnConversationEnd(Transform actor)
    {
        isConversationRunning = false;
        if (conversationQueue.Count > 0)
        {
            StartConversation(conversationQueue.Dequeue()); //remove entry from the queue so the next one can play
        }

        int conversationID = DialogueManager.Instance.currentConversationState.subtitle.dialogueEntry.conversationID;
        Debug.Log("Conversation ended: " + conversationID.ToString());
        
        if ( conversationID == 1 || conversationID == 3)
        {
            hint.text = hints[0];
            if (language == "en")
            {
                hint.text = "Use Shift to Sprint";
            }
            flowchart.ExecuteBlock("SwapHints");
        }
        else if (conversationID == 2 || conversationID == 4)
        {
            secondTrigger.SetActive(false);
        }
        else if (conversationID == 5 || conversationID == 6)
        {
            firstTrigger.SetActive(false);
        }
        else if (conversationID == 7 || conversationID == 8)
        {
            thirdTrigger.SetActive(false);
        }
        else if (conversationID == 13 || conversationID == 14)
        {
            Scene2H();
        }
    }

    public void Scene2A() //start convo
    {
        if (!reloaded)
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
       
    }

    public void Scene2B() //must possess key
    {
        JudgeLanguage();
        string conversation = conversationName[1] + "_" + language;
        if (isConversationRunning)
        {
            conversationQueue.Enqueue(conversation);
        }
        else
        {
            StartConversation(conversation);
        }
    }

    public void Scene2C() //approach gate
    {
        if (!reloaded)
        {
            JudgeLanguage();
            string conversation = conversationName[2] + "_" + language;
            if (isConversationRunning)
            {
                conversationQueue.Enqueue(conversation);
            }
            else
            {
                StartConversation(conversation);
            }
        }
    }

    public void Scene2D() //engage brides
    {
        JudgeLanguage();
        string conversation = conversationName[3] + "_" + language;
        if (isConversationRunning)
        {
            conversationQueue.Enqueue(conversation);
        }
        else
        {
            StartConversation(conversation);
        }
    }

    public void Scene2E() //player spotlight
    {
        JudgeLanguage();
        string conversation = conversationName[4] + "_" + language;
        StartConversation(conversation);
    }

    public void Scene2F() //enemy spotlight
    {
        JudgeLanguage();
        string conversation = conversationName[5] + "_" + language;
        StartConversation(conversation);
    }

    public void Scene2G() 
    {
        JudgeLanguage();
        string conversation = conversationName[6] + "_" + language;
        if (isConversationRunning)
        {
            conversationQueue.Enqueue(conversation);
        }
        else
        {
            StartConversation(conversation);
        }
    }

    public void Scene2H() //area complete (NOT CALLED YET)
    {
        JudgeLanguage();
        string conversation = conversationName[7] + "_" + language;
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

    IEnumerator SetBooleanFalseAfterDelay(float delay)
    {
        // Wait for the specified amount of time
        yield return new WaitForSeconds(delay);

        // Set the boolean to false
        inPursuitCalled = false;
    }

    public void SwapHint()
    {
        hintsCurrentIndex++;
        hint.text = hints[hintsCurrentIndex];
        if (language == "en" && hintsCurrentIndex == 1)
        {
            hint.text = "Taking Photos from the back weakens the enemies";
        }
        if (language == "en" && hintsCurrentIndex == 2)
        {
            hint.text = "The amount of enemies in the frame determines the amount of enemies you combat";
        }
    }

    public void StartHint()
    {
       
    }

    void PlayEffectKey()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart
            flowchart.ExecuteBlock("GetKey");
        }
    }
}
