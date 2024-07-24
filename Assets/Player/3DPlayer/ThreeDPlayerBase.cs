using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class ThreeDPlayerBase : MonoBehaviour
{
    public int HP;
    public GameObject UIHandler;
    public GameObject levelController;
    public Flowchart flowchart;
    // Start is called before the first frame update
    void Start()
    {
        UIHandler = GameObject.Find("PlayerUI");
        UIHandler.GetComponent<PlayerToUI>().UpdateHP(HP);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void gotHitByEnemy()
    {
        PlayEffectHit();
        HP--;
        ES3.Save("MoveHP", HP);
        if (GameObject.Find("GoggleCanvas") != null)
        {
            GameObject.Find("GoggleCanvas").GetComponent<Player3DUI>().DecreaseHP(HP);
        }
        if (GameObject.FindAnyObjectByType<Level3>() != null)
        {
            Terminal[] terminals = FindObjectsOfType<Terminal>();

            // Iterate through each Terminal and call the Break method
            foreach (Terminal terminal in terminals)
            {
                terminal.Break();
            }
        }
        //UIHandler.GetComponent<PlayerToUI>().UpdateHP(HP);
        if (HP <= 0)
        {
            ThreeDTo2DData.ThreeDScene = null;
            GameObject.Find("GoggleCanvas").GetComponent<Player3DUI>().DecreaseHP(HP);
            if (GameObject.FindAnyObjectByType<Level2>() != null)
            {
                levelController.GetComponent<Level2>().ResetLevel();
            }
            else if (GameObject.FindAnyObjectByType<Level3>() != null)
            {
                levelController.GetComponent<Level3>().ResetLevel();
            }
            PlayEffectDie();
        }
    }

    void PlayEffectHit()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart
            flowchart.ExecuteBlock("PlayerHit");
        }
    }

    void PlayEffectDie()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart
            flowchart.ExecuteBlock("PlayerDie");
        }
    }
}
