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
    public GameObject triggerBox;
    public float targetExposure = -2.0f;
    public GameObject key;
    public Volume volume;
    private ColorAdjustments colorAdjustments;
    public string language;
    public string[] conversationName;
    public int ch_sub_speed;
    public int en_sub_speed;
    // Start is called before the first frame update
    void Awake()
    {
        ES3.Save("Sprint", true);
        colorAdjustments = (ColorAdjustments)volume.profile.components.Find(x => x is ColorAdjustments);
        Player.GetComponent<GadgetsTool>().MagneticBoots = false;
        Player.GetComponent<GadgetsTool>().Camera = true;
        if (ThreeDTo2DData.ThreeDScene != null)
        {
            ThreeDTo2DData.ThreeDScene = null;
            Reload();
        }
        else
        {
            triggerBox.SetActive(true);
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
                    enemy.GetComponent<ThreeEnemyBase>().ChangeMaterial();
                }
            }
        }

    }

    IEnumerator GraduallyChangeExposure()
    {

        float elapsedTime = 0.0f;
        float initialExposure = colorAdjustments.postExposure.value;

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
        foreach (Transform enemy in enemies)
        {
            if (enemy.gameObject.active)
            {
                enemy.GetComponent<ThreeEnemyBase>().inPursuit(target);
            }
        }
    }

    void ResetEnemyBackToPatrol(Transform target)
    {
        foreach (Transform enemy in enemies)
        {
            if (enemy.gameObject.active)
            {
                enemy.GetComponent<AIDestinationSetter>().enabled = false;
                enemy.GetComponent<Patrol>().enabled = true;
            }
        }
    }
    public void ColdStart()
    {
        StartCoroutine(GraduallyChangeExposure());
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

        //Spotlights
        StartTheLights();
        spotlightManager.GetComponent<SpotlightManager>().Resume();

        //PlayerStats & EnemyStats
        if (TwoDto3D.ToThreeEnemies.Count == 0)
        {
            //Debug.Log("Here");
            foreach (var key in ThreeDTo2DData.dataDictionary.Keys)
            {
                //Debug.Log(key);
                GameObject obj = GameObject.Find(key);
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
        else
        {
            Player.GetComponent<ThreeDPlayerBase>().gotHitByEnemy();
            /*
            foreach (var key in ThreeDTo2DData.dataDictionary.Keys)
            {
                //Debug.Log(key);
                GameObject obj = GameObject.Find(key);
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
            foreach (string name in TwoDto3D.ToThreeEnemies)
            {
                //Debug.Log(key);
                GameObject obj = GameObject.Find(name);
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
            */
        }

        //Key
        if (!enemies[randomIndex].gameObject.activeSelf)
        {
            key.GetComponent<Door>().Keyed = true;
        }
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
        yield return new WaitForSecondsRealtime(2f);
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

        randomIndex = Random.Range(0, enemies.Count);
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
        int conversationName = DialogueManager.Instance.currentConversationState.subtitle.dialogueEntry.conversationID;
        // Debug log or use the conversation name as needed
        Debug.Log("Conversation ended: " + conversationName.ToString());
    }

    public void Sentence3()
    {
        JudgeLanguage();
        DialogueManager.StartConversation(conversationName[3] + "_" + language);
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
