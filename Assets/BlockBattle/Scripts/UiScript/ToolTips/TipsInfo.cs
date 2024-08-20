using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
                switch (LocalizationManager.CurrentLanguage)
                {
                    case "English":
                        switch (colorindex)
                        {
                            case 0:
                                header = "Light Attack";
                                content = "Deal danmage, puts enemy into Pause";
                                detail1 = "Deal 2 damage";
                                detail2 = "Deal 4 damage; Inflict Pause for 2 seconds";
                                detail3 = "Deal 7 damage; Inflict Pause for 3 seconds";
                                detail4 = "Deal 10 damage; Inflict Pause for 5 seconds";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 1:
                                header = "Heal";
                                content = "Heal HP";
                                detail1 = "Heal 5 HP";
                                detail2 = "Heal 8 HP";
                                detail3 = "Heal 12 HP";
                                detail4 = "Heal 15 HP";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 2:
                                header = "Heavy Attack";
                                content = "Deal Danmage";
                                detail1 = "Deal 5 damage";
                                detail2 = "Deal 8 damage";
                                detail3 = "Deal 12 damage";
                                detail4 = "Deal 15 damage";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 3:
                                header = "Restore Stance";
                                content = "Remove Debuffs";
                                detail1 = "Remove 1 Debuff";
                                detail2 = "Remove 1 Debuff";
                                detail3 = "Remove 1 Debuff";
                                detail4 = "Remove 2 Debuff";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 4:
                                header = "Krumphau";
                                content = "Deal danmage, Stagger the enemy";
                                detail1 = "Deal 3 damage";
                                detail2 = "Deal 5 damage";
                                detail3 = "Deal 6 damage, Stagger";
                                detail4 = "Deal 8 damage, Stagger";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 5:
                                header = "Parry";
                                content = "Gain Parry to block attacks";
                                detail1 = "Gain 1 Parry";
                                detail2 = "Gain 1 Parry";
                                detail3 = "Gain 2 Parry";
                                detail4 = "Gain 2 Parry";

                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 6:
                                header = "Zornhau";
                                content = "Deal damage; if elimating 4 rows of it at a time, deals 30";
                                detail1 = "Deal 1 damage";
                                detail2 = "Deal 1 damage";
                                detail3 = "Deal 1 damage";
                                detail4 = "If 4 lines are cleared, +29 damage";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 8:
                                header = "Uneffective Block";
                                content = "No effects";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                        }
                        break;
                    case "Chinese (Simplified)":
                        switch (colorindex)
                        {
                            case 0:
                                header = "轻攻击";
                                content = "造成伤害; 使敌人陷入停滞";
                                detail1 = "2点伤害";
                                detail2 = "4点伤害；停滞2秒";
                                detail3 = "7点伤害，停滞3秒";
                                detail4 = "10点伤害，停滞5秒";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
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
                                content = "效果: 无";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                        }
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
        switch (LocalizationManager.CurrentLanguage)
        {
            case "English":
                switch (Name)
                {
                    case "MedKit":
                        header = "Med Kit";
                        content = "Heal 10 HP";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "SprayCan":
                        header = "Spray Can";
                        content = "Refresh all Command Blocks in Choice Section";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "Mint":
                        header = "Mint";
                        content = "Remove all debuffs and immune to most debuffs for 10 seconds";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "PaperCutter":
                        header = "Paper Cutter";
                        content = "Inflict 1 layer of Fragile";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "FracturedPocketWatch":
                        header = "Fractured Pocket Watch";
                        content = "Pause 3 seconds to all enemies";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;

                }
                break;
            case "Chinese (Simplified)":
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
                break;
        }



        
    }

    public string GetEnemyChineseName(string Name)
    {
        switch (LocalizationManager.CurrentLanguage)
        {
            case "English":
                switch (Name)
                {
                    case "headless bride":
                        return "Headless Bride";
                    case "Perseus":
                        return "Perseus";
                    case "lion":
                        return "Lion";
                    case "deer":
                        return "Deer";
                    case "mocking bird":
                        return "Mocking Bird";
                    case "floral sarcoid":
                        return "Floral Sarcoid";
                    case "artemis":
                        return "Artemis";
                    case "deadalus":
                        return "Deadalus";
                }
                break;
            case "Chinese (Simplified)":
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
                    case "deadalus":
                        return "代达罗斯";
                }
                break;
        }
        
        return Name;
    }

    public string GetEnemySkillChineseName(string SkillName)
    {
        switch (LocalizationManager.CurrentLanguage)
        {
            case "English":
                switch (SkillName)
                {
                    case "Attack":
                        return "Attack";
                    case "CurseOfGorgon":
                        return "Curse of Gorgon";
                    case "SculptureGlane":
                        return "Sculpture Glace";
                    case "SculptureGaze":
                        return "Sculpture Gaze";
                    case "Bite":
                        return "Bite";
                    case "GoldenAntler":
                        return "Golden Antler";
                    case "Charge":
                        return "Charge";
                    case "Scream":
                        return "Scream";
                    case "ShapeShift":
                        return "Shape Shift";
                    case "BlindAmbush":
                        return "Blind Ambush";
                    case "Swap":
                        return "Swap";
                    case "TakeAim":
                        return "Take Aim";
                    case "ChariotOfGolden":
                        return "Chariot of Golden";
                    case "Golden Arrows":
                        return "Golden Arrows";
                    case "CalloftheWild":
                        return "Call of the Wild";
                    case "WaxedWings":
                        return "Waxed Wings";
                    case "WaxSPray":
                        return "Wax Spray";
                    case "kindledStrike":
                        return "Kindled Strike";
                    case "Thelabyrinth":
                        return "The Labyrinth";
                    case "MazeShift":
                        return "Maze Shift";
                    case "Rekindle":
                        return "Rekindle";
                    case "BurntWings":
                        return "BurntWings";
                }
                break;
            case "Chinese (Simplified)":
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
                    case "WaxedWings":
                        return "蜡翅";
                    case "WaxSPray":
                        return "蜡液喷洒";
                    case "kindledStrike":
                        return "点燃一击";
                    case "Thelabyrinth":
                        return "迷城";
                    case "MazeShift":
                        return "迷城挪移";
                    case "Rekindle":
                        return "重燃";
                    case "BurntWings":
                        return "灼燃之翅";
                }
                break;
        }
        
        return SkillName;
    }

    public string GetEnemySkillDetail(string SkillName,int damage)
    {
        switch (LocalizationManager.CurrentLanguage)
        {
            case "English":
                switch (SkillName)
                {
                    case "Attack":
                        return "Cause " + damage + " Danmage";
                    case "CurseOfGorgon":
                        return "Curse of Gorgon";
                    case "SculptureGlane":
                        return "Deal 7 damage; Add 1 Stun to Choice Section.";
                    case "SculptureGaze":
                        return "Lose this skill if sneak attacked. Deal 8 damage; Add 3 Stun to Choice Section.";
                    case "Bite":
                        return "Deal 7 damage; Inflict 1 Bleed.";
                    case "GoldenAntler":
                        return "Deal 12 damage; Heal all enemies by half of damage dealt.";
                    case "Charge":
                        return "Inflict Excited to all enemies, lasting 20 seconds.";
                    case "Scream":
                        return "Inflict Pause, lasting 2 seconds.";
                    case "ShapeShift":
                        return "Gain Invincible. Remove after 4 Command Block rotations.";
                    case "BlindAmbush":
                        return "Target a random unit in combat, including other enemies. Deal 12 damage. If this skill hits another enemy, Stagger target. Otherwise, damage * 3.";
                    case "Swap":
                        return "Randomly change shape of all Command Blocks in Choice Section.";
                    case "TakeAim":
                        return "Inflict 1 Fragile.";
                    case "ChariotOfGolden":
                        return "Deal 16 damage; Heal all enemies by half of damage dealt.";
                    case "GoldenArrows":
                        return "Summon Lion, Mocking Bird and Deer until there are 4 enemies.";
                    case "CalloftheWild":
                        return "Target all other units in combat (including other enemies). Deal 8 * target number damage. ";
                    case "WaxedWings":
                        return "Deal 16 damage; Cover a Command Block in Choice Section with Burning Wax.";
                    case "WaxSPray":
                        return "Deal 15 damage; Randomly select 12 cubes and cover them with Burning Wax.";
                    case "kindledStrike":
                        return "When you have 8 Overheat, use this skill. Destroy all Burning Wax and covered cubes and remove all Overheat. Deal 20 + 5 * X damage. (X is the number of Burning Wax destroyed)";
                    case "Thelabyrinth":
                        return "When HP drops to below 0, ???";
                    case "MazeShift":
                        return "Cannot use this skill when Rekindled. Use only when HP < 50%. When HP < 50% for the first time, use this skill. Gain 3 Rekindled.";
                    case "Rekindle":
                        return "Cannot use this skill when Rekindled. Use only when HP < 50%. When HP < 50% for the first time, use this skill. Gain 3 Rekindled.";
                    case "BurntWings":
                        return "Can only use this skill when Rekindled. Deal 12 damage; Inflict 2 Overheat; Remove 1 Rekindled.";
                }
                break;
            case "Chinese (Simplified)":
                switch (SkillName)
                {
                    case "Attack":
                        return "造成" + damage + "点伤害";
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
                    case "WaxedWings":
                        return "造成16点伤害，并使选择区的一个指令块覆盖融蜡。";
                    case "WaxSPray":
                        return "造成15点伤害，并随机选择12个方块，使其覆盖融蜡。";
                    case "kindledStrike":
                        return "当你拥有8层过热时，使用该技能。摧毁所有融蜡和被融蜡覆盖的方块，并移除所有过热。造成20+5*X点伤害（X为被摧毁的融蜡数量）。";
                    case "Thelabyrinth":
                        return "当HP小于0时，？？？";
                    case "MazeShift":
                        return "在重燃状态下无法使用该技能。造成10点伤害，并施加迷城，持续15秒。";
                    case "Rekindle":
                        return "在重燃状态下无法使用该技能。当生命值小于50%时，才可以使用该技能，且在生命值第一次小于50%时，使用该技能。获得3层重燃。";
                    case "BurntWings":
                        return "只有在重燃状态才可以使用该技能。造成12点伤害，施加2层过热，并失去1层重燃。";
                }
                break;
        }
      
        return SkillName;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
