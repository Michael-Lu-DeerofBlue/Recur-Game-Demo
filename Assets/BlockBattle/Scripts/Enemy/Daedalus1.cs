using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Daedalus1 : Enemy
{
    public Sprite WaxSprite;
    public int attackDamage = 10;
    public int WaxedWingsDamage = 16;
    public int WaxSprayDamage = 15;
    public int KindledStrikeDamage = 20;
    public int KindledStrikeEachCubeDamage = 5;

    public int attackWeight = 25;
    public int WaxedWingsWeight = 40;
    public int WaxSPrayWeight = 35;
    public int KindledStrikeWeight = 0;
    public int TheLabyrinthWeight = 0;

    public float attackCastingTime = 8;
    public float WaxedWingsCastingTime = 4;
    public float WaxSPrayCastingTime = 10;
    public float kindledStrikeCastingTime = 20;
    public float ThelabyrinthCastingTime = 5;



    private enum SkillType { Attack, WaxedWings, WaxSPray, kindledStrike, Thelabyrinth }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f, attackDamage);
                break;
            case SkillType.WaxedWings:
                WaxWings();
                break;
            case SkillType.WaxSPray:
                WaxSpray();
                DealAttackDamage(WaxSprayDamage);
                break;
        }
    }
    public void KindledStrike()
    {
        battleManager.KindledStrike(KindledStrikeDamage, KindledStrikeEachCubeDamage);
        DealAttackDamage(KindledStrikeDamage);

    }
    public void WaxSpray()
    {

        int times = 12;

        while (times > 0)
        {
            float randomValue = Random.value;

            if (randomValue < 0.1f) // 20% probability to call WaxWings()
            {
                WaxWings();
                times -= 4;
            }
            else // 80% probability to call X event
            {
                if (WaxRandomBlockOnGrid(WaxSprite) == true)
                {
                    times -= 1;
                }
            }
        }
    }

    public bool WaxRandomBlockOnGrid(Sprite WaxSprite)
    {
        BlockManager blockManager = FindObjectOfType<BlockManager>();
        bool Success = blockManager.AddSpriteToRandomBlock(WaxSprite);
        return Success;

    }


    public void WaxWings()
    {
        InSelectionBar[] allBlocks = FindObjectsOfType<InSelectionBar>();
        List<InSelectionBar> activeBlocks = new List<InSelectionBar>();

        // Filter only active InSelectionBar scripts without existing "WaxSpriteChild"
        foreach (InSelectionBar block in allBlocks)
        {
            if (block.isActiveAndEnabled && !HasWaxSpriteChild(block))
            {
                activeBlocks.Add(block);
            }
        }

        if (activeBlocks.Count == 0)
        {
            Debug.Log("No active blocks found in the selection bar.");
        }
        else
        {
            // Randomly select one active block
            InSelectionBar selectedBlock = activeBlocks[Random.Range(0, activeBlocks.Count)];
            SpriteRenderer[] squares = selectedBlock.GetComponentsInChildren<SpriteRenderer>();

            if (squares.Length == 0)
            {
                Debug.Log("No Square components found in the selected block.");
            }
            else
            {
                foreach (SpriteRenderer square in squares)
                {
                    // Add a new child game object with a SpriteRenderer component to each square
                    GameObject newChild = new GameObject("WaxSpriteChild");
                    newChild.transform.parent = square.transform;
                    newChild.transform.localPosition = Vector3.zero; // Ensure the local position is (0, 0, 0)
                    SpriteRenderer spriteRenderer = newChild.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = WaxSprite;
                    spriteRenderer.sortingOrder = 12;
                }
            }
        }
    }

    private bool HasWaxSpriteChild(InSelectionBar block)
    {
        foreach (Transform child in block.transform)
        {
            foreach (Transform grandchild in child)
            {
                if (grandchild.name == "WaxSpriteChild")
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void GetNextMove()
    {
        int sum = attackWeight + WaxedWingsWeight + WaxSPrayWeight;
        float attackProbability = (float)attackWeight / sum;
        float WaxedWingsProbability = (float)WaxedWingsWeight / sum;
        float WaxSPrayProbability = (float)WaxSPrayWeight / sum;

        float randomValue = Random.value;


        if (randomValue < attackProbability)
        {
            SkillCastingTime = attackCastingTime;  // Attack action
            nextSkill = SkillType.Attack;
            CurrentSkillIcons = new string[] { "Damage" };
            NextSkillDamage = attackDamage;

        }
        else if (randomValue < attackProbability + WaxedWingsProbability)
        {
            SkillCastingTime = WaxedWingsCastingTime;
            nextSkill = SkillType.WaxedWings;
            CurrentSkillIcons = new string[] { "Interrupt", "Damage" };

        }
        else
        {
            SkillCastingTime = WaxSPrayCastingTime;
            nextSkill = SkillType.WaxSPray;
            CurrentSkillIcons = new string[] { "Interrupt", "Damage" };

        }
        nextMove = nextSkill.ToString();
    }
    public override void Start()
    {
        base.Start();
        StaggerReis = 3;
    }


}

