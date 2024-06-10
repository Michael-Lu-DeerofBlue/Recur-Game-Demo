using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private HeroInfo heroInfo;

    // Start is called before the first frame update
    void Start()
    {
        heroInfo = FindObjectOfType<HeroInfo>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReceColorMessage(int colorCode, int clearNumber)
    {
        heroInfo.ExecuteBehavior(colorCode, clearNumber);
    }
}
