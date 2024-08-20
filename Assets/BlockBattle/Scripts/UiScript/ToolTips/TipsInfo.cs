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
                                header = "�ṥ��";
                                content = "����˺�; ʹ��������ͣ��";
                                detail1 = "2���˺�";
                                detail2 = "4���˺���ͣ��2��";
                                detail3 = "7���˺���ͣ��3��";
                                detail4 = "10���˺���ͣ��5��";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 1:
                                header = "����";
                                content = "�ָ���������ֵ";
                                detail1 = "5������ֵ";
                                detail2 = "8������ֵ";
                                detail3 = "12������ֵ";
                                detail4 = "15������ֵ";

                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 2:
                                header = "�ع���";
                                content = "����˺� ";
                                detail1 = "5���˺�";
                                detail2 = "8���˺�";
                                detail3 = "12���˺�";
                                detail4 = "15���˺�";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 3:
                                Debug.Log("here");
                                header = "�ָ�����";
                                content = "�Ƴ�������״̬";
                                detail1 = "1������";
                                detail2 = "1������";
                                detail3 = "1������";
                                detail4 = "2������";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 4:
                                header = "����";
                                content = "���6���˺�; ʹĿ������";
                                detail1 = "3���˺�";
                                detail2 = "5���˺�";
                                detail3 = "6���˺���ʹĿ������";
                                detail4 = "8���˺���ʹĿ������";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 5:
                                header = "�񵲼���";
                                content = "��ø񵲴������ɵ����˺�";
                                detail1 = "1���";
                                detail2 = "1���";
                                detail3 = "2���";
                                detail4 = "2���";

                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 6:
                                header = "ŭ��";
                                content = "���1���˺������������4�е�ͬʱ����4�����飬���30���˺�";
                                detail1 = "���1���˺�";
                                detail2 = "���1���˺�";
                                detail3 = "���1���˺�";
                                detail4 = "��������ʱ���˺�+29";
                                TTooltipSystem.showBlockTips(header, content, detail1, detail2, detail3, detail4);
                                break;
                            case 8:
                                header = "��Ч����";
                                content = "Ч��: ��";
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
                        header = "ҽ�ư�";
                        content = "�ָ�10��HP";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "SprayCan":
                        header = "�����";
                        content = "ˢ��ѡ����������ָ���";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "Mint":
                        header = "��ˬ������";
                        content = "�Ƴ����м��棬����10�������ߴ󲿷ּ���";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "PaperCutter":
                        header = "��ֽ��";
                        content = "��Ŀ��ʩ��1�����";
                        TTooltipSystem.showInventoryTips(header, content);
                        break;
                    case "FracturedPocketWatch":
                        header = "����Ļ���";
                        content = "�����е���ʩ��ͣ�ͣ�����3��";
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
                        return "��������";
                    case "Perseus":
                        return "������˹";
                    case "lion":
                        return "ʨ��";
                    case "deer":
                        return "¹";
                    case "mocking bird":
                        return "��ľ��";
                    case "floral sarcoid":
                        return "����";
                    case "deadalus":
                        return "������˹";
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
                        return "��ͨ����";
                    case "CurseOfGorgon":
                        return "�����������";
                    case "SculptureGlane":
                        return "ɨ��";
                    case "SculptureGaze":
                        return "��������";
                    case "Bite":
                        return "˺ҧ";
                    case "GoldenAntler":
                        return "�ƽ�¹��";
                    case "Charge":
                        return "����";
                    case "Scream":
                        return "����";
                    case "ShapeShift":
                        return "��̬";
                    case "BlindAmbush":
                        return "̰��ͻϮ";
                    case "Swap":
                        return "�任";
                    case "TakeAim":
                        return "��׼";
                    case "ChariotOfGolden":
                        return "���ս��";
                    case "Golden Arrows":
                        return "��Ұ�ĺ���";
                    case "CalloftheWild":
                        return "�ƽ����";
                    case "WaxedWings":
                        return "����";
                    case "WaxSPray":
                        return "��Һ����";
                    case "kindledStrike":
                        return "��ȼһ��";
                    case "Thelabyrinth":
                        return "�Գ�";
                    case "MazeShift":
                        return "�Գ�Ų��";
                    case "Rekindle":
                        return "��ȼ";
                    case "BurntWings":
                        return "��ȼ֮��";
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
                        return "���" + damage + "���˺�";
                    case "CurseOfGorgon":
                        return "�����������";
                    case "SculptureGlane":
                        return "���7���˺�������1��ѣ������ѡ������";
                    case "SculptureGaze":
                        return "���˱�͵Ϯ��ʧȥ�ü��ܡ����8���˺�������3��ѣ������ѡ������";
                    case "Bite":
                        return "���" + damage + "�˺�����ʩ��1����Ѫ��";
                    case "GoldenAntler":
                        return "���12���˺������������е��ˣ�������Ϊ��ɵ��˺���һ�롣";
                    case "Charge":
                        return "�����е���ʩ���˷ܣ�����20�롣";
                    case "Scream":
                        return "ʩ��ͣ�ͣ�����2�롣";
                    case "ShapeShift":
                        return "����޵С�����ת4��ָ���ʱ���Ƴ��޵С�";
                    case "BlindAmbush":
                        return "ѡ��1�����Ŀ�꣨����ս���е��������ˣ����������12���˺������Ŀ��Ϊ�������ˣ�ʹ�����ǡ�������ɵ��˺� * 3��";
                    case "Swap":
                        return "����任ѡ������ָ�����״��";
                    case "TakeAim":
                        return "ʩ��1�������";
                    case "ChariotOfGolden":
                        return "���16���˺������������е��ˣ�������Ϊ��ɵ��˺���һ�롣";
                    case "GoldenArrows":
                        return "�ٻ�ʨ�ӡ������¹��ֱ��ս�������ĸ����ˡ�";
                    case "CalloftheWild":
                        return "������������λ��ΪĿ�꣨�����������ˣ���������ɵ�ͬ����ѡĿ����8�����˺���";
                    case "WaxedWings":
                        return "���16���˺�����ʹѡ������һ��ָ��鸲��������";
                    case "WaxSPray":
                        return "���15���˺��������ѡ��12�����飬ʹ�串��������";
                    case "kindledStrike":
                        return "����ӵ��8�����ʱ��ʹ�øü��ܡ��ݻ����������ͱ��������ǵķ��飬���Ƴ����й��ȡ����20+5*X���˺���XΪ���ݻٵ�������������";
                    case "Thelabyrinth":
                        return "��HPС��0ʱ��������";
                    case "MazeShift":
                        return "����ȼ״̬���޷�ʹ�øü��ܡ����10���˺�����ʩ���Գǣ�����15�롣";
                    case "Rekindle":
                        return "����ȼ״̬���޷�ʹ�øü��ܡ�������ֵС��50%ʱ���ſ���ʹ�øü��ܣ���������ֵ��һ��С��50%ʱ��ʹ�øü��ܡ����3����ȼ��";
                    case "BurntWings":
                        return "ֻ������ȼ״̬�ſ���ʹ�øü��ܡ����12���˺���ʩ��2����ȣ���ʧȥ1����ȼ��";
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
