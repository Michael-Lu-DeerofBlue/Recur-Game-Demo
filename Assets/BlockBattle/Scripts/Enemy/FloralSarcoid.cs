using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloralSarcoid : Enemy
{
    public int attackDamage = 15;
    public int BlindAmbushDamage = 12;

    public int attackWeight = 2;
    public int ShapeShiftWeight = 3;
    public int BlindAmbushWeight=2;
    public int SwapWeight = 3;

    public float attackCastingTime = 10;
    public float ShapeShiftCastingTime = 5;
    public float BlindAmbushCastingTime = 15;
    public float SwapCastingTime = 3;

    public bool IsAnyBlockEnable = false;
    public int RotaLeftForInvincible = 0;
    private enum SkillType { Attack, ShapeShift, BlindAmbush, Swap }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f, attackDamage);
                break;
            case SkillType.ShapeShift:
                RotaLeftForInvincible = 4;
                Invincible = true;

                break;
            case SkillType.BlindAmbush:
                Enemy[] allEnemies = FindObjectsOfType<Enemy>();
                Enemy[] enemies = System.Array.FindAll(allEnemies, enemy => enemy != this);
                if (enemies.Length > 0)
                {
                    int randomIndex = Random.Range(-1, enemies.Length);
                    if (randomIndex == -1)
                    {
                        DealAttackDamage(BlindAmbushDamage * 3);
                    }
                    else
                    {
                        enemies[randomIndex].HitHandle(BlindAmbushDamage);
                        enemies[randomIndex].ResetCasting();
                    }
                    Debug.Log(randomIndex);
                }
                else //only this enemy on the battle, attack hero
                {
                    DealAttackDamage(BlindAmbushDamage * 3);
                }
                break;
            case SkillType.Swap:
                RefreshChoiceSectionBlock();
                break;
        }
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + ShapeShiftWeight + BlindAmbushWeight + SwapWeight;
        float attackProbability = (float)attackWeight / sum;
        float ShapeShiftProbability = (float)ShapeShiftWeight / sum;
        float BlindAmbushProbability=(float) BlindAmbushWeight / sum;
        float SwapProbability = (float)SwapWeight / sum;

        float randomValue = Random.value;


        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
            NextSkillDamage = attackDamage;
            CurrentSkillIcons = new string[] { "Damage" };
        }
        else if (randomValue < attackProbability + ShapeShiftProbability)
        {
            SkillCastingTime = ShapeShiftCastingTime;
            nextSkill = SkillType.ShapeShift;
            CurrentSkillIcons = new string[] { "Buff" };
        }
        else if(randomValue < attackProbability + ShapeShiftProbability+ BlindAmbushProbability)
        {
            SkillCastingTime = BlindAmbushCastingTime;
            nextSkill = SkillType.BlindAmbush;
            CurrentSkillIcons = new string[] { "Damage" };
        }
        else
        {
            SkillCastingTime = SwapCastingTime;
            nextSkill = SkillType.Swap;
            CurrentSkillIcons = new string[] { "Interrupt" };
        }
        nextMove = nextSkill.ToString();
    }


    public override void Update()
    {
        CheckForEnabledBlockManager();
        if (PauseCasting || SpendingSkillAnim)//the casting time will not decrease sometime because of debug, or during interruption of block game.
        {
            return;
        }
        if (timer <= 0 && !SpendingSkillAnim)
        {
            ExecuteAnimation();

        }
        if (battleManager.BlockGameTimeStop == false && !SpendingSkillAnim && !IsAnyBlockEnable)
        {
            CastingTimerReduce(CastingSpeedRate);
        }


        enemyInfoText.text = "HP: " + HP + "\nNext Move: " + nextMove + "\nTime to Execute Turn: " + timer.ToString("F2");
        UpdateEnemyUI();

        if (targetSelector.CurrentTarget != null)
        {
            if (targetSelector.CurrentTarget == this)
            {
                SelectedByPlayer();
            }
        }
        if (ShowingTip)
        {
            TTooltipSystem.showEnemyTips(DisplayName, HP.ToString("F2"), timer.ToString("F2"), nextMove, NextSkillDamage);
        }

    }
    private void CheckForEnabledBlockManager()
    {
        BlockManager[] blockManagers = FindObjectsOfType<BlockManager>();
        IsAnyBlockEnable = false;

        foreach (var blockManager in blockManagers)
        {
            if (blockManager.enabled)
            {
                IsAnyBlockEnable = true;
                break;
            }
        }

    }


    public void OneSecFasterWhileBlockRota()
    {
        timer--;
        RotaLeftForInvincible --;
        if(RotaLeftForInvincible<=0 )
        {
            RotaLeftForInvincible = 0;
            Invincible = false;
        }

    }
}
