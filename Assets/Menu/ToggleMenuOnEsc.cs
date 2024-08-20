using UnityEngine;
using UnityEngine.SceneManagement;

public class ToggleMenuOnEsc : MonoBehaviour
{
    public GameObject targetObject; // Reference to the menu GameObject
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
        Debug.Log("Sent Esc Message");
        mainUIManager.OnEsc();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Hit Tab");
            if (MainUIManager.gameStateString == "pauseMenu" || MainUIManager.gameStateString == "gaming")
            {
                sendEscMessage();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log("Hit Esc");
            if (currentSceneName == "CentralMeditationRoom")
            {
                bool inOp = GameObject.Find("Level Script").GetComponent<Level1>().inOpenin;
                if (inOp)
                {
                    mainUIManager.SettingForceQuit();
                }
                else
                {
                    sendEscMessage();
                }
            }
            else
            {
                sendEscMessage();
            }
        }

    } 
}
    

