using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : LevelController
{
    public GameObject Player;

    // Start is called before the first frame update
    void Awake()
    {
        Player.GetComponent<GadgetsTool>().MagneticBoots = false;
        Player.GetComponent<GadgetsTool>().Camera = true;
    }

    // Update is called once per frame
    public override void GoToBattle()
    {
        base.GoToBattle();
    }
}
