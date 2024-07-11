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
        UIHandler.GetComponent<PlayerToUI>().UpdateHP(HP);
        if (HP <= 0)
        {
            ThreeDTo2DData.ThreeDScene = null;
            levelController.GetComponent<Level2>().ResetLevel();
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
