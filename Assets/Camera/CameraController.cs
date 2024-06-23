using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera playerCamera;
    public float zoomSpeed = 10f;
    public float minZoom = 20f;
    public float maxZoom = 60f;
    public float maxDistance = 100f;
    private bool isCameraMode = false;
    public LayerMask targetLayer;
    public LayerMask wallLayer;
    public GameObject UIIndicator;
    public int gridResolution = 10;
    void Update()
    {
        // Toggle camera mode
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isCameraMode) { playerCamera.fieldOfView = 60; }
            isCameraMode = !isCameraMode;
            UIIndicator.GetComponent<PlayerToUI>().CameraUI(isCameraMode.ToString());
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

    void DetectTargetsInFrame()
    {
        int count = 0;
        HashSet<Collider> detectedTargets = new HashSet<Collider>();

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
                            detectedTargets.Add(hit.collider);
                        }
                    }
                }
            }
        }

        count = detectedTargets.Count;

        Debug.Log("Number of target objects in frame: " + count + " Time: " + Time.time);
    }

    bool IsObstructed(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        return Physics.Raycast(start, direction, distance, wallLayer);
    }
}
