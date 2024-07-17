using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsInfo : MonoBehaviour
{
    string header = string.Empty;
    string content = string.Empty;
    string detail1= string.Empty;
    string detail2 = string.Empty;
    string detail3= string.Empty;
    string detail4= string.Empty;

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
                        header = "轻攻击";
                        content="造成伤害; 使敌人陷入停滞";
                        detail1 = "2点伤害";
                        detail2 = "4点伤害；停滞2秒";
                        detail3 = "7点伤害，停滞3秒";
                        detail4 = "10点伤害，停滞5秒";
                        TTooltipSystem.showBlockTips(header,content,detail1,detail2,detail3,detail4);
                        break;
                    case 1:
                        header = "治疗";
                        content = "恢复自身生命值";
                        detail1 = "5点生命值";
                        detail2 = "8点生命值";
                        detail3 = "12点生命值";
                        detail4 = "15点生命值";

                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 2:
                        header = "重攻击";
                        content = "造成伤害 ";
                        detail1 = "5点伤害";
                        detail2 = "8点伤害";
                        detail3 = "12点伤害";
                        detail4 = "15点伤害";
                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 3:
                        header = "恢复架势";
                        content = "移除自身负面状态";
                        detail1 = "1个减益";
                        detail2 = "1个减益";
                        detail3 = "1个减益";
                        detail4 = "2个减益";
                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 4:
                        header = "曲击";
                        content = "造成6点伤害; 使目标蹒跚";
                        detail1 = "3点伤害";
                        detail2 = "5点伤害";
                        detail3 = "6点伤害；使目标蹒跚";
                        detail4 = "8点伤害；使目标蹒跚";
                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 5:
                        header = "Yellowblock";
                        content = "longsword yellow block skill ";
                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 6:
                        header = "Lightblueblock";
                        content = "longsword lb block skill ";
                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 8:
                        header = "无效方块";
                        content = "效果：无";
                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
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
