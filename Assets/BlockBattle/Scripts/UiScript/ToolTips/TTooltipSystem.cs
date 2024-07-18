using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTooltipSystem : MonoBehaviour
{
    private static TTooltipSystem current;
    public TTooltip Ttooltip;
    public BlockTip blockTip;
    public EnemyTip enemyTip;
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
            current.Ttooltip.gameObject.SetActive(true);
            current.Ttooltip.SetText(header, content);
        }
    }

    public static void showEnemyTips(string Name = "123", string HP = "123", string CastingTime = "123", string NextMove = "123", int nextskilldamage=0)
    {
        BattleManager battleManager = FindAnyObjectByType<BattleManager>();
        if (battleManager == null)
        {
            Debug.LogError("BattleManager not found.");
            return;
        }

        if (battleManager.ToolTipsLevel != 2)
        {
            current.enemyTip.gameObject.SetActive(true);
            current.enemyTip.SetText(Name,HP,CastingTime,NextMove,nextskilldamage);
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
            current.blockTip.gameObject.SetActive(true);
            current.blockTip.SetText(header, content,detail1,detail2,detail3,detail4);

        }
    }
    public static void Hide()
    {
        current.Ttooltip.gameObject.SetActive(false);
        current.blockTip.gameObject.SetActive(false);
        current.enemyTip.gameObject.SetActive(false);
    }
}
