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

    public void ReceColorMessage(string colorCode)
    {
        Debug.Log("Received color code: " + colorCode);
        heroInfo.ExecuteBehavior(colorCode);
    }
}
