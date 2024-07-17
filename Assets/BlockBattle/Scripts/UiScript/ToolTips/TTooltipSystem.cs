using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTooltipSystem : MonoBehaviour
{
    private static TTooltipSystem current;
    public TTooltip Ttooltip;
    public BlockTip blockTip;
    public void Awake()
    {
        current = this;
    }

    // Update is called once per frame
    public static void showInventoryTips(string header = "123", string content = "123132123")
    {
        BattleManager battleManager = FindAnyObjectByType<BattleManager>();
        if (battleManager == null)
        {
            Debug.LogError("BattleManager not found.");
            return;
        }

        if (battleManager.ToolTipsLevel != 2)
        {
            current.Ttooltip.SetText(header, content);
            current.Ttooltip.gameObject.SetActive(true);
        }
    }

    public static void showBlockTips(string header = "123", string content = "123132123", string detail1="", string detail2= "", string detail3= "", string detail4= "")
    {
        BattleManager battleManager = FindAnyObjectByType<BattleManager>();
        if (battleManager == null)
        {
            Debug.LogError("BattleManager not found.");
            return;
        }

        if (battleManager.ToolTipsLevel != 2)
        {
            current.blockTip.SetText(header, content,detail1,detail2,detail3,detail4);
            current.blockTip.gameObject.SetActive(true);
        }
    }
    public static void Hide()
    {
        current.Ttooltip.gameObject.SetActive(false);
        current.blockTip.gameObject.SetActive(false);
    }
}
