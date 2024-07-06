using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenuManager : MonoBehaviour
{
    public GameObject settingMenu;
    public bool active;
    // Start is called before the first frame update
    void Start()
    {
        active = false;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingMenu();
        }
    }

    public void ToggleSettingMenu()
    {
        if (settingMenu != null)
        {
            active = !active;
            if (active){
                if (DialogueManager.isConversationActive)
                {
                    DialogueManager.Pause();
                }
                Time.timeScale = 0; 
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else { 
                Time.timeScale = 1; 
                if (GameObject.Find("Player") != null)
                {
                    GameObject.Find("Player").BroadcastMessage("UpdateSettings");
                }
                if (DialogueManager.isConversationActive)
                {
                    DialogueManager.Unpause();
                }
            }
            settingMenu.SetActive(active);
            
        }
    }
}