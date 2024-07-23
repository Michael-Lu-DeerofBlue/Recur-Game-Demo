using Pathfinding;
using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3 : LevelController
{
    private AstarPath aStarPath;
    public List<Transform> playerMarkers;
    public GameObject playerRef;
    public Transform bridge;
    public GameObject cube;
    public List<GameObject> enemies;
    public List<Vector3> BridgeCubePositions;
    public Camera playerCamera;
    public Camera moveCamera;
    public Camera topDownCamera;
    public float waitDelay;
    public GameObject board;
    public GameObject[] SceneObjects;
    public static bool firstAccess = true;
    // Start is called before the first frame update
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
        board.GetComponent<Board>().SaveTileMap();
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

    public void SpawnBridge()
    {
        foreach (var position in BridgeCubePositions)
        {
            GameObject lilBridge = Instantiate(cube, position, Quaternion.identity);
            lilBridge.transform.parent = bridge.transform;
        }
        UpdateBehavior();
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

    public Vector3 GrabNearestMarkerLocation()
    {
        Transform nearestMarker = null;
        float minDistance = float.MaxValue;

        foreach (Transform marker in playerMarkers)
        {
            float distance = Vector3.Distance(playerRef.transform.position, marker.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestMarker = marker;
            }
        }

        if (nearestMarker != null)
        {
            return nearestMarker.position;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
