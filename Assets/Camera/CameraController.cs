using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera playerCamera;
    public float zoomSpeed = 10f;
    public float minZoom = 20f;
    public float maxZoom = 60f;
    public float maxDistance = 100f;
    public bool isCameraMode = false;
    public LayerMask targetLayer;
    public LayerMask wallLayer;
    public GameObject UIIndicator;
    public GameObject LevelController;
    public int gridResolution = 10;
    private Dictionary<string, bool> detectedEnemies = new Dictionary<string, bool>();
    public Image googleFrame; // Reference to the UI Image component
    public float duration = 0.2f; // Duration of the fade
    public Color whiteColor = new Color(1, 1, 1, 1); // Default to white with alpha 0
    public Color transColor = new Color(1, 1, 1, 0); // Default to white with alpha 1
    void Update()
    {
        // Toggle camera mode
        if (Input.GetKeyDown(KeyCode.E) && GetComponentInParent<GadgetsTool>().Camera)
        {
            if (isCameraMode) { playerCamera.fieldOfView = 60; }
            isCameraMode = !isCameraMode;
            if (isCameraMode)
            {
                //on
                StartCoroutine(Fade(transColor, whiteColor));
            }
            else
            {
                //off
                StartCoroutine(Fade(whiteColor, transColor));
            }
        }

        // Zoom functionality
        /*
        if (isCameraMode)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            playerCamera.fieldOfView -= scrollInput * zoomSpeed;
            playerCamera.fieldOfView = Mathf.Clamp(playerCamera.fieldOfView, minZoom, maxZoom);
        }
        */
        if (Input.GetMouseButtonDown(0) && isCameraMode)
        {
            DetectTargetsInFrame();
        }
    }

    public IEnumerator Fade(Color fromColor, Color toColor)
    {
        float elapsedTime = 0f;
        googleFrame.color = fromColor; // Set the initial color

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            googleFrame.color = Color.Lerp(fromColor, toColor, elapsedTime / duration); // Interpolate color
            yield return null; // Wait for the next frame
        }

        // Ensure the final color is set
        googleFrame.color = toColor;
    }

    void DetectTargetsInFrame()
    {
        int count = 0;
        detectedEnemies.Clear();
        HashSet<Transform> detectedTargets = new HashSet<Transform>();

        for (int x = 0; x < gridResolution; x++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                float u = (float)x / (gridResolution - 1);
                float v = (float)y / (gridResolution - 1);
                Ray ray = playerCamera.ViewportPointToRay(new Vector3(u, v, 0));

                RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance, targetLayer | wallLayer);
                foreach (RaycastHit hit in hits)
                {
                    if ((targetLayer & (1 << hit.collider.gameObject.layer)) != 0)
                    {
                        if (!IsObstructed(ray.origin, hit.point))
                        {
                            if (detectedTargets.Count < 4)
                            {
                                detectedTargets.Add(hit.transform);
                            }
                            
                        }
                    }
                }
            }
        }

        count = detectedTargets.Count;
        
        foreach (var target in detectedTargets)
        {
            //Debug.Log(target.name);
            string targetName = target.gameObject.transform.parent.name;
            //Debug.Log(target.transform.parent.rotation);
            bool isFacingAway = IsFacingAwayFromPlayer(target.parent.transform);
            detectedEnemies[targetName] = isFacingAway;
        }
        ThreeDTo2DData.dataDictionary.Clear();
        ThreeDTo2DData.dataDictionary = detectedEnemies;
        /*
       Debug.Log("Number of target objects in frame: " + count + " Time: " + Time.time);
       foreach (var enemy in detectedEnemies)
       {
           Debug.Log($"Enemy: {enemy.Key}, Detected: {enemy.Value}" + " Time: " + Time.time);
       }
       */
        if (count > 0)
        {
            LevelController.GetComponent<LevelController>().GoToBattle();
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName == "galleries")
            {
                LevelController.GetComponent<GalleryLevel>().SceneChange();
            }
        }
        
    }

    bool IsFacingAwayFromPlayer(Transform enemyTransform)
    {
        Vector3 directionToPlayer = (playerCamera.transform.position - enemyTransform.position).normalized;
        float dotProduct = Vector3.Dot(enemyTransform.forward, directionToPlayer);
        return dotProduct < 0; // Returns true if the enemy is facing away from the player
    }

    bool IsObstructed(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        return Physics.Raycast(start, direction, distance, wallLayer);
    }
}
