using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDPlayerBase : MonoBehaviour
{
    public int HP;
    public GameObject UIHandler;
    public GameObject levelController;
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
        //Hit by enemy SFX
        HP--;
        UIHandler.GetComponent<PlayerToUI>().UpdateHP(HP);
        if (HP <= 0)
        {
            ThreeDTo2DData.ThreeDScene = null;
            levelController.GetComponent<Level2>().ResetLevel();
            //Player Die SFX
        }
    }
}
