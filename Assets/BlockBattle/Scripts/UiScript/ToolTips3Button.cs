using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTips3Button : MonoBehaviour
{
    public Button[] buttonTypes;
    BattleManager battleManager;
    // Start is called before the first frame update
    void Start()
    {
        battleManager = FindAnyObjectByType<BattleManager>();
        for (int i = 0; i < buttonTypes.Length; i++)
        {
            int index = i;
            buttonTypes[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnButtonClick(int index)
    {
        battleManager.ToolTipsLevel = index;
    }
}
