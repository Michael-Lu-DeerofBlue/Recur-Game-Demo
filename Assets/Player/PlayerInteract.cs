using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask mask;
    private Camera cam;
    private PlayerToUI playerToUI;
    private InputManager inputManager;
    public bool canInteract;
    void Start()
    {
        cam = GetComponent<PlayerController>().cam;
        if (cam == null)
        {
            Debug.LogError("Camera component is missing on PlayerController.");
        }

        playerToUI = playerUI.GetComponent<PlayerToUI>();
        if (playerToUI == null)
        {
            Debug.LogError("PlayerToUI component is missing on PlayerUI.");
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

        playerToUI.UpdateText(string.Empty);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        //Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;
        if (canInteract)
        {
            if (Physics.Raycast(ray, out hitInfo, distance, mask))
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    playerToUI.UpdateText(interactable.promptMessage);
                    if (inputManager.OnFoot.Interact.triggered)
                    {
                        interactable.BaseInteract();
                    }
                }
            }
        }
       
    }
}
