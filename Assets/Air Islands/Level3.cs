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

    // Start is called before the first frame update
    void Start()
    {
        aStarPath = FindObjectOfType<AstarPath>(); // Find the AstarPath component in the scene
    }

    public void EnterBoard()
    {
        flowchart.ExecuteBlock("MoveToCanmera");
    }

    public void ExitBoard()
    {
        flowchart.ExecuteBlock("MoveBackToPlayer");
    }

    public void SpawnBridge()
    {
        foreach (var position in BridgeCubePositions)
        {
            Instantiate(cube, position, Quaternion.identity);
        }
        //UpdateBehavior();
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
    void Update()
    {

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
