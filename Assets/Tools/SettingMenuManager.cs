using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenuManager : MonoBehaviour
{
    public GameObject settingMenu;

    // Start is called before the first frame update
    void Start()
    {
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
            settingMenu.SetActive(!settingMenu.activeSelf);
            if (GameObject.Find("Player") != null)
            {
                GameObject.Find("Player").BroadcastMessage("UpdateSettings");
            }
        }
    }
}