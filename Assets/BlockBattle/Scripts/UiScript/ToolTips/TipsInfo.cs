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
                        Debug.Log("here");
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
                        header = "格挡架势";
                        content = "获得格挡次数，可抵消伤害";
                        detail1 = "1层格挡";
                        detail2 = "1层格挡";
                        detail3 = "2层格挡";
                        detail4 = "2层格挡";

                        TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                        break;
                    case 6:
                        header = "怒击";
                        content = "造成1点伤害；如果消除了4行的同时消除4个方块，造成30点伤害";
                        detail1 = "造成1点伤害";
                        detail2 = "造成1点伤害";
                        detail3 = "造成1点伤害";
                        detail4 = "符合条件时，伤害+29";
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
                        header = "医疗包";
                        content = "恢复10点HP";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "SprayCan":
                        header = "喷漆罐";
                        content = "刷新选择区的所有指令块";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "Mint":
                        header = "劲爽薄荷糖";
                        content = "移除所有减益，并在10秒内免疫大部分减益";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "PaperCutter":
                        header = "裁纸刀";
                        content = "对目标施加1层脆弱";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "FracturedPocketWatch":
                        header = "破碎的怀表";
                        content = "对所有敌人施加停滞，持续3秒";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;

                }
    }

    public string GetEnemyChineseName(string Name)
    {
        switch (Name)
        {
            case "headless bride":
                return "无首新娘";
            case "Perseus":
                return "帕尔修斯";
            case "lion":
                return "狮子";
            case "deer":
                return "鹿";
            case "mocking bird":
                return "啄木鸟"; 
            case "floral sarcoid":
                return "花瘤";
            case "artemis":
                return "阿尔特弥斯";
        }
        return Name;
    }

    public string GetEnemySkillChineseName(string SkillName)
    {
        switch (SkillName)
        {
            case "Attack":
                return "普通攻击";
            case "CurseOfGorgon":
                return "戈尔贡的诅咒";
            case "SculptureGlane":
                return "扫视";
            case "SculptureGaze":
                return "破碎凝视";
            case "Bite":
                return "撕咬";
            case "GoldenAntler":
                return "黄金鹿角";
            case "Charge":
                return "激励";
            case "Scream":
                return "鸣叫";
            case "ShapeShift":
                return "拟态";
            case "BlindAmbush":
                return "贪婪突袭";
            case "Swap":
                return "变换";
            case "TakeAim":
                return "瞄准";
            case "ChariotOfGolden":
                return "金角战车";
            case "Golden Arrows":
                return "荒野的呼唤";
            case "CalloftheWild":
                return "黄金箭雨";
        }
        return SkillName;
    }

    public string GetEnemySkillDetail(string SkillName,int damage)
    {
        switch (SkillName)
        {
            case "Attack":
                return "造成"+damage+"点伤害";
            case "CurseOfGorgon":
                return "戈尔贡的诅咒";
            case "SculptureGlane":
                return "造成7点伤害，并将1个眩晕置入选择区。";
            case "SculptureGaze":
                return "敌人被偷袭后，失去该技能。造成8点伤害，并将3个眩晕置入选择区。";
            case "Bite":
                return "造成" + damage + "伤害，并施加1层流血。";
            case "GoldenAntler":
                return "造成12点伤害，并治疗所有敌人，治疗量为造成的伤害的一半。";
            case "Charge":
                return "对所有敌人施加兴奋，持续20秒。";
            case "Scream":
                return "施加停滞，持续2秒。";
            case "ShapeShift":
                return "获得无敌。当旋转4次指令块时，移除无敌。";
            case "BlindAmbush":
                return "选择1个随机目标（包括战斗中的其他敌人），对其造成12点伤害。如果目标为其他敌人，使其蹒跚。否则，造成的伤害 * 3。";
            case "Swap":
                return "随机变换选择区的指令块形状。";
            case "TakeAim":
                return "施加1层脆弱。";
            case "ChariotOfGolden":
                return "造成16点伤害，并治疗所有敌人，治疗量为造成的伤害的一半。";
            case "GoldenArrows":
                return "召唤狮子、嘲鸟和鹿，直到战斗中有四个敌人。";
            case "CalloftheWild":
                return "将所有其他单位作为目标（包括其他敌人），对其造成等同于所选目标数8倍的伤害。";
        }
        return SkillName;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
