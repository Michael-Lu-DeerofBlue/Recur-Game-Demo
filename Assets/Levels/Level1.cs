using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class Level1 : LevelController
{
    public GameObject Player;
    public Button continueButton;
    public GameObject openingMenu;
    public GameObject fallingCamera;
    // Start is called before the first frame update
    void Awake()
    {
        if (!ES3.KeyExists("First Time"))
        {
            continueButton.interactable = false;
        }
        Player.GetComponent<GadgetsTool>().MagneticBoots = false;
        Player.GetComponent<GadgetsTool>().Camera = true;
    }

    // Update is called once per frame
    public override void GoToBattle()
    {
        base.GoToBattle();
    }

    public void NewGame()
    {
        ES3.Save("First Time", false);
        openingMenu.SetActive(false);
        flowchart.ExecuteBlock("CameraRotate");
        flowchart.ExecuteBlock("CameraDrop");
    }

    public void switchCamera()
    {
        fallingCamera.SetActive(false);
        Player.SetActive(true);
    }

    public void Continue()
    {
        openingMenu.SetActive(false);
        flowchart.ExecuteBlock("CameraRotate");
        flowchart.ExecuteBlock("CameraDrop");
        flowchart.ExecuteBlock("ContinueWhiteScreen");
    }

}
