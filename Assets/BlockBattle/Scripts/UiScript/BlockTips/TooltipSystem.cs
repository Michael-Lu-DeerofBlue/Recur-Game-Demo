using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem instance;
    public ToolTipUI ToolTipUI;

    // Start is called before the first frame update
    public void Awake()
    {
        instance = this; 
    }
    public static void Show(string header = "123", string content = "123132123")
    {
        BattleManager battleManager = FindAnyObjectByType<BattleManager>();
        if (battleManager == null)
        {
            Debug.LogError("BattleManager not found.");
            return;
        }

        if (battleManager.ToolTipsLevel != 2)
        {
            instance.ToolTipUI.SetText(header, content);
            instance.ToolTipUI.gameObject.SetActive(true);
        }
    }

    public static void Hide()
    {
        if (instance != null && instance.ToolTipUI != null)
        {
            instance.ToolTipUI.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("TooltipSystem instance or ToolTipUI is null in Hide.");
        }
    }
}
