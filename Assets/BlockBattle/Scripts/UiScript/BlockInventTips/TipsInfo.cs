using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsInfo : MonoBehaviour
{
    string header = string.Empty;
    string content = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void FindBlockTipsContext(int colorindex)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            LongSword longSword = player.GetComponent<LongSword>();
            if (longSword != null)
            {

                switch (colorindex)
                {
                    case 0:
                        header = "redblock";
                        content="longsword 红色 red block skill ";
                        TTooltipSystem.showInventoryTips(header,content);
                        break;
                    case 1:
                        header = "Greenblock";
                        content = "longsword green block skill ";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case 2:
                        header = "Orangeblock";
                        content = "longsword Orange block skill 橙色技能 ";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case 3:
                        header = "DBlueblock";
                        content = "longsword Dblue block skill ";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case 4:
                        header = "Purpleblock";
                        content = "longsword Purple block skill ";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case 5:
                        header = "Yellowblock";
                        content = "longsword yellow block skill ";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case 6:
                        header = "Lightblueblock";
                        content = "longsword lb block skill ";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case 8:
                        header = "stunblock";
                        content = "do nothing block ";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                }
            }
            else
            {
                Debug.Log("Player object does not have LongSword script.");
            }
        }
        else
        {
            Debug.Log("Player object not found.");
        }
    }

    public void FindInventoryTipsContext(string Name)
    {


                switch (Name)
                {
                    case "MedKit":
                        header = "Med Kit";
                        content = "+10HP";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "SprayCan":
                        header = "Spray Can";
                        content = "Refresh all Action Blocks in Choice Section";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "Mint":
                        header = "Mint";
                        content = "Remove all debuffs and immune to all debuffs for 10 seconds";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "PaperCutter":
                        header = "Paper Cutter";
                        content = "Inflict [Fragile]";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "FracturedPocketWatch":
                        header = "Fractured Pocket Watch";
                        content = "Pause Action Bar for 3 seconds ";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;

                }
            }





    // Update is called once per frame
    void Update()
    {
        
    }
}
