using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemEventHandler : MonoBehaviour
{
    BattleManager battleManager;
    private HeroInfo heroInfo;
    public bool ItemSelectingEnemy = false;
    public string ItemUsing = "";
    private void Start()
    {
        heroInfo = FindObjectOfType<HeroInfo>();
        battleManager = FindObjectOfType<BattleManager>();
    }
    public void HandleItemEvent(string itemName)
    {
        switch (itemName)
        {
            case "SculptureClay":
                UseSculptureClay();
                break;
            case "SprayCan":
                UseSprayCan();
                break;
            case "ArtRubber":
                UseArtRubber();
                break;
            case "PaperCutter":
                UsePaperCutter();
                break;
            case "FracturedPocketWatch":
                UseFracturedPocketWatch();
                break;
            default:
                Debug.LogWarning("No event handler defined for item: " + itemName);
                break;
        }
    }

    void UseSculptureClay()
    {
        heroInfo.Heal(10);
        Debug.Log("Sculpture Clay used!");
    }

    void UseSprayCan()
    {
        //Õâ¸öË¦¸øÄã
        Debug.Log("Spray Can used!");
    }

    void UseArtRubber()
    {
        battleManager.RemoveAllPlayerDebug();
        battleManager.PlayerImmueDebuffDuring(10);
        Debug.Log("Art Rubber used!");
    }

    void UsePaperCutter()
    {
        ItemSelectingEnemy = true;
        ItemUsing = "PaperCutter";
    }

    void UseFracturedPocketWatch()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            battleManager.PuaseSingleEnemyActionBar(3, enemy);
        }
        Debug.Log("Fractured Pocket Watch used!");
    }
}

