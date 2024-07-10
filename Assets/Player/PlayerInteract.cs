using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask mask;
    private InputManager inputManager;

    void Start()
    {
        cam = GetComponent<PlayerController>().cam;
        if (cam == null)
        {
            Debug.LogError("Camera component is missing on PlayerController.");
        }

        inputManager = GetComponent<InputManager>();
        if (inputManager == null)
        {
            Debug.LogError("InputManager component is missing.");
        }
    }

    void Update()
    {
        if (cam == null || inputManager == null)
        {
            return; // Exit if required components are not assigned
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        //Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                Debug.Log(interactable.promptMessage);
                if (inputManager.OnFoot.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
