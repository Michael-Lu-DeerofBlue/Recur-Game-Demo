using UnityEngine;

public class ToggleMenuOnEsc : MonoBehaviour
{
    public GameObject targetObject; // Reference to the menu GameObject
    public MonoBehaviour cursorScript;
    public MonoBehaviour PlayerController; // Reference to the player control script
    public MonoBehaviour CameraController; // Reference to the camera control script
    public MainUIManager mainUIManager;

    private bool isPaused = false; // To track the paused state

    void Start()
    {
        mainUIManager = FindAnyObjectByType<MainUIManager>();
        // Ensure all references are set
        if (targetObject == null)
        {
            Debug.LogError("Target object is not assigned.");
        }

        if (PlayerController == null)
        {
            Debug.LogError("Player control script is not assigned.");
        }

        if (CameraController == null)
        {
            Debug.LogError("Camera control script is not assigned.");
        }
    }

    public void sendEscMessage()
    {
        mainUIManager.OnEsc();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sendEscMessage();

            if (cursorScript != null)
            {
                cursorScript.enabled = !isPaused;
            }

            if (PlayerController != null)
            {
                PlayerController.enabled = !isPaused;
            }

            if (CameraController != null)
            {
                CameraController.enabled = !isPaused;
            }

            // Optionally, lock/unlock the cursor
            if (isPaused)
            {
                //Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    } 
}
    

