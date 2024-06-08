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

    private bool isCameraMode = false;
    public GameObject UIIndicator;

    void Update()
    {
        // Toggle camera mode
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isCameraMode) { playerCamera.fieldOfView = 60; }
            isCameraMode = !isCameraMode;
            UIIndicator.SetActive(isCameraMode);
        }

        // Zoom functionality
        if (isCameraMode)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            playerCamera.fieldOfView -= scrollInput * zoomSpeed;
            playerCamera.fieldOfView = Mathf.Clamp(playerCamera.fieldOfView, minZoom, maxZoom);
        }

        if (Input.GetMouseButtonDown(0) && isCameraMode)
        {
            DetectTargetsInFrame();
        }
    }

    void DetectTargetsInFrame()
    {
        int targetCount = 0;
        
        // Alternative: Detect all objects within the camera's view frustum
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");

        foreach (GameObject target in targets)
        {
            if (target.GetComponent<Collider>() && GeometryUtility.TestPlanesAABB(planes, target.GetComponent<Collider>().bounds))
            {
                // Raycast from the camera to the target to ensure it is not occluded
                Vector3 direction = target.transform.position - playerCamera.transform.position;
                Ray ray = new Ray(playerCamera.transform.position, direction);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.CompareTag("Target"))
                    {
                        targetCount++;
                    }
                }
            }
        }

        Debug.Log("Number of target objects in frame: " + targetCount);
    }
}
