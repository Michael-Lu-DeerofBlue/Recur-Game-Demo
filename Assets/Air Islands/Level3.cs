using Fungus;
using Pathfinding;
using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;
using PixelCrushers.DialogueSystem;
using Unity.Burst.CompilerServices;
using I2.Loc;
using UnityEngine.PlayerLoop;

public class Level3 : LevelController
{
    private AstarPath aStarPath;
    public GameObject playerRef;
    public Transform bridge;
    public GameObject cube;
    public GameObject ladderCube;
    public List<GameObject> enemies;
    public List<Vector3> BridgeCubePositions;
    public List<Vector3> lastBridgeCubePositions = new List<Vector3>();
    public List<Vector3> EndBridgeCubePositions;
    public Camera playerCamera;
    public Camera moveCamera;
    public Camera topDownCamera;
    public float waitDelay;
    public GameObject board;
    public GameObject[] SceneObjects;
    public GameObject[] TranslatedObjects;
    public static bool firstAccess = true;
    public GameObject transitioner;
    public GameObject ladderBridge;
    public string[] conversationName;
    private string language;
    public int ch_sub_speed;
    public int en_sub_speed;
    public static bool firstTimeEnterBoard = true;
    public static bool firstTimeBuidlingBridge = true;
    public Flowchart sfxFlowchart;
    // Start is called before the first frame update
    private void Awake()
    {
        ES3.Save("MoveHP", 5);
        ES3.Save("Sprint", true);
        Dictionary<string, int> iniConsumablesInventory = new Dictionary<string, int>()
    {
        { "MedKit", 2 },
        { "SprayCan", 2 },
        { "Mint", 2 },
        { "PaperCutter", 2 },
        { "FracturedPocketWatch", 2 }
    };
        ES3.Save("Consumables", iniConsumablesInventory);
        ES3.Save("Level", 3);

    }


    void OnConversationEnd(Transform actor)
    {
        int conversationID = DialogueManager.Instance.currentConversationState.subtitle.dialogueEntry.conversationID;
        Debug.Log("Conversation ended: " + conversationID.ToString());
        if (conversationID == 11 || conversationID == 14)
        {
            flowchart.ExecuteBlock("GoToGallery2");
        }
    }

    public void Sentence1() //start convo
    {
        JudgeLanguage();
        string conversation = conversationName[0] + "_" + language;
        DialogueManager.StartConversation(conversation);
    }

    public void Sentence2() //start convo
    {
        JudgeLanguage();
        string conversation = conversationName[1] + "_" + language;
        DialogueManager.StartConversation(conversation);
    }

    public void Sentence3() //start convo
    {
        JudgeLanguage();
        string conversation = conversationName[2] + "_" + language;
        DialogueManager.StartConversation(conversation);
    }

    public void Sentence4() //start convo
    {
        JudgeLanguage();
        string conversation = conversationName[3] + "_" + language;
        DialogueManager.StartConversation(conversation);
    }

    public void Sentence5() //start convo
    {
        JudgeLanguage();
        string conversation = conversationName[4] + "_" + language;
        DialogueManager.StartConversation(conversation);
    }


    public void SpawnBridge()
    {
        if (firstTimeBuidlingBridge)
        {
            firstTimeBuidlingBridge=false;
            Sentence3();
        }
        // Sort the list by the z value of the position
        BridgeCubePositions.Sort((a, b) => a.z.CompareTo(b.z));

        if (lastBridgeCubePositions.Count == 0)
        {
            flowchart.ExecuteBlock("RescanAfter2Seconds");
        }

        // Determine new positions that are not in the lastBridgeCubePositions
        var newPositions = BridgeCubePositions.Except(lastBridgeCubePositions).ToList();

        // Start the coroutine to spawn and animate the bridge for new positions
        StartCoroutine(SpawnBridgeCubes(newPositions));

        // Update lastBridgeCubePositions to the current BridgeCubePositions
        lastBridgeCubePositions = new List<Vector3>(BridgeCubePositions);
    }

    private IEnumerator SpawnBridgeCubes(List<Vector3> positions)
    {
        foreach (var position in positions)
        {
            GameObject lilBridge = Instantiate(cube, position, Quaternion.identity);
            lilBridge.transform.parent = bridge.transform;

            // Initially set the cube below the bridge position
            lilBridge.transform.position = new Vector3(position.x, position.y - 5, position.z);

            // Animate the cube rising up to the bridge position
            lilBridge.transform.DOMoveY(position.y, 0.5f).SetEase(Ease.OutBounce);

            // Wait for a specified delay before spawning the next cube
            yield return new WaitForSeconds(waitDelay);
        }

        UpdateBehavior();
    }

    public void TransitionToBattle()
    {
        foreach (GameObject obj in TranslatedObjects)
        {
            obj.SetActive(false);
        }
        sfxFlowchart.ExecuteBlock("Key Tab");
    }

    public void ReloadBackToBattle()
    {
        BlockStageController[] blockStageControllers = FindObjectsOfType<BlockStageController>();
        foreach (BlockStageController controller in blockStageControllers)
        {
            Destroy(controller.gameObject);
        }
        foreach (GameObject obj in TranslatedObjects)
        {
            obj.SetActive(true);
        }
        if (TwoDto3D.win == true)
        {
            //Debug.Log("here");
            foreach (var key in ThreeDTo2DData.dataDictionary.Keys)
            {
                Debug.Log(key);
                GameObject obj = GameObject.Find(key);
                if (obj != null)
                {
                    //Debug.Log("Here");
                    if (key == "Artemis")
                    {
                        EndAirIsland();
                    }
                    else
                    {
                        obj.SetActive(false);
                    }
                    
                }
            }
        }
        else
        {
            ResetLevel();
        }
        playerRef.GetComponent<PlayerController>().cam.GetComponent<CameraController>().TurnCameraOff();
    }

    void EndAirIsland()
    {
        Sentence5();
    }

    void Start()
    {
        aStarPath = FindObjectOfType<AstarPath>(); // Find the AstarPath component in the scene
    }

    public void EnterBoard()
    {   
        playerCamera.gameObject.SetActive(false);
        moveCamera.gameObject.SetActive(true);
        flowchart.ExecuteBlock("MoveToCamera");
        StartCoroutine(CameraMoveUp());
    }

    private IEnumerator CameraMoveUp()
    {
        // Wait for 0.5 seconds
        yield return new WaitForSeconds(waitDelay);

        // Change the camera's FOV to 115 over 1 second
        float duration = 1f;
        float startFOV = moveCamera.fieldOfView;
        float endFOV = 115f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            moveCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        moveCamera.fieldOfView = endFOV;

    }

    public void DisableSceneObjects()
    {
        foreach (GameObject obj in SceneObjects)
        {
            obj.SetActive(false);
        }
    }

    public void EnableSceneObjects()
    {
        foreach (GameObject obj in SceneObjects)
        {
            obj.SetActive(true);
        }
    }

    public void ExitBoard()
    {
        IconShow[] icons = FindObjectsOfType<IconShow>();
        foreach (IconShow icon in icons)
        {
            icon.Hide();
        }
        EnableSceneObjects();
        playerRef.gameObject.SetActive(true);
        moveCamera.gameObject.SetActive(true);
        topDownCamera.gameObject.SetActive(false);
        flowchart.ExecuteBlock("MoveBackToPlayer");
        StartCoroutine(CameraMoveDown());
    }

    private IEnumerator CameraMoveDown()
    {
        // Wait for 0.5 seconds
        yield return new WaitForSeconds(waitDelay);

        // Change the camera's projection to perspective and FOV to 60 over 1 second

        float duration = 2f;
        float startFOV = moveCamera.fieldOfView;
        float endFOV = 60f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            moveCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        moveCamera.fieldOfView = endFOV;
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
        foreach (GameObject enemy in enemies)
        {
            ES3.DeleteKey(enemy.name + " Position");
            ES3.DeleteKey(enemy.name + " Rotation");
        }
        ES3.DeleteKey("InLevelPlayerPosition");
        ES3.DeleteKey("InLevelPlayerRotation");
        ES3.DeleteKey("EnemyKeyIndex");
        ES3.Save("MoveHP", 2);
    }



    public void UpdateBehavior()
    {
        Rescan();
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<DeerProcessor>() != null)
            {
                enemy.GetComponent<DeerProcessor>().RounteCheck();
            }
            if (enemy.GetComponent<LionProcessor>() != null)
            {
                enemy.GetComponent<LionProcessor>().RounteCheck();
            }
        }
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (board.activeSelf == false)
            {
                EnterBoard();
            }
        }
        if (playerRef.transform.position.y < -5)
        {
            ResetLevel();
        }
        
    }

    public void TerminalEnterBoard()
    {
        if (board.activeSelf == false)
        {
            EnterBoard();
        }
    }

    public void Rescan()
    {
      
        if (aStarPath != null)
        {
            aStarPath.Scan(); // Rescan the whole map to generate a new grid map
            Debug.Log("Map rescanned successfully.");
        }
        else
        {
            Debug.LogError("AstarPath component not found!");
        }
    }

    public Vector3Int GrabNearestMarker()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("L3Marker");
        if (markers.Length == 0)
        {
            Debug.LogWarning("No markers found with tag 'L3Maker'");
            return Vector3Int.zero; // or some default value
        }

        GameObject nearestMarker = null;
        float minDistance = float.MaxValue;

        foreach (GameObject marker in markers)
        {
            float distance = Vector3.Distance(playerRef.transform.position, marker.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestMarker = marker;
            }
        }

        Debug.Log(nearestMarker.gameObject.name);

        if (nearestMarker != null)
        {
            MarkerPos markerPos = nearestMarker.GetComponent<MarkerPos>();
            nearestMarker.GetComponent<IconShow>().Show();
            if (markerPos != null)
            {
                Vector3Int spawnPoint = markerPos.SpanwPoint;
                return new Vector3Int(spawnPoint.x, spawnPoint.y, 0);
            }
            else
            {
                Debug.LogWarning("MarkerPos script not found on the nearest marker");
            }
        }

        return Vector3Int.zero;
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
}
