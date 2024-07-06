using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem instance;
    public ToolTip tooltip;
    private BattleManager battleManager;
    // Start is called before the first frame update
    public void Awake()
    {
        instance = this; 
        battleManager =FindAnyObjectByType<BattleManager>();
    }
    public static void Show(string header= "123", string content = "123132123")
    {
        BattleManager battleManager = FindAnyObjectByType<BattleManager>();
        if (battleManager.ToolTipsLevel != 2)
        {
            instance.tooltip.SetText(header, content);
            instance.tooltip.gameObject.SetActive(true);
        }

    }

    // Update is called once per frame
    public static void Hide()
    {
        instance.tooltip.gameObject.SetActive(false);
    }
}
