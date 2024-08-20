using I2.Loc;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GalleryLevel : LevelController
{
    public GameObject Player;
    public Transform[] enemies;
    public Transform point;
    public static bool deadalus;

    public string language;
    public string[] conversationName;
    public int ch_sub_speed;
    public int en_sub_speed;
    private Queue<string> conversationQueue = new Queue<string>();
    private bool isConversationRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        if (ThreeDTo2DData.ThreeDScene != null)
        {
            ThreeDTo2DData.ThreeDScene = null;
            Reload();
        }
    }

    void Reload()
    {
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
        foreach (Transform enemy in enemies)
        {
            enemy.position = ES3.Load<Vector3>(enemy.name + " Position");
            enemy.rotation = ES3.Load<Quaternion>(enemy.name + " Rotation");
        }
        if (TwoDto3D.win == true)
        {
            foreach (var key in ThreeDTo2DData.dataDictionary.Keys)
            {
                GameObject obj = GameObject.Find(key);
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToJungle()
    {
        flowchart.ExecuteBlock("GoToJungle");
    }
    public void GoToDeadalus()
    {
        ThreeDTo2DData.dataDictionary = new Dictionary<string, bool>() { { "Deadalus", false } };
        deadalus = true;
        GoToBattle();
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
    public void Sentence2() //must possess key
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

    public void GoToVertical()
    {
        Player.GetComponent<PlayerController>().enabled = false;
        Player.GetComponent<WToMoveUp>().enabled = true;
        Player.GetComponent<Rigidbody>().useGravity = false;
        //increase the y of the player by 100 over time
        flowchart.ExecuteBlock("GoToVertical");
        StartCoroutine(CameraMoveUp());
        StartCoroutine(MovePlayerUp());
    }

    private IEnumerator MovePlayerUp()
    {
        yield return new WaitForSeconds(2f);
        float duration = 5.0f; // Duration in seconds
        float targetHeight = Player.transform.position.y + 100;
        Vector3 startPosition = Player.transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, targetHeight, startPosition.z);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            Player.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Player.transform.position = targetPosition;
    }

    private IEnumerator CameraMoveUp()
    {
        // Wait for 0.5 seconds
        yield return new WaitForSeconds(0.6f);
        Player.transform.position = point.position;
    }

    public void SceneChange()
    {
        Save();
    }

    public void Save()
    {
        Debug.Log("hee");
        Player.GetComponent<PlayerController>().Save();

        //Enemy
        foreach (Transform enemy in enemies)
        {
            ES3.Save(enemy.name + " Position", enemy.position);
            ES3.Save(enemy.name + " Rotation", enemy.rotation);
        }
    }
}
