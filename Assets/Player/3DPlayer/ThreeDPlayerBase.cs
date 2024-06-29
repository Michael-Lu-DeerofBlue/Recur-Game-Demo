using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDPlayerBase : MonoBehaviour
{
    public int HP;
    public GameObject UIHandler;
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
        HP--;
        UIHandler.GetComponent<PlayerToUI>().UpdateHP(HP);
    }
}
