using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerseusTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Pause();
    }

    void Start()
    {
        
    }

    public void Pause()
    {
        GameObject BM = GameObject.Find("BattleManager");
        BM.GetComponent<BattleManager>().BlockGameTimeStop = true;
    }

    public void Resume()
    {
        GameObject BM = GameObject.Find("BattleManager");
        BM.GetComponent<BattleManager>().BlockGameTimeStop = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
