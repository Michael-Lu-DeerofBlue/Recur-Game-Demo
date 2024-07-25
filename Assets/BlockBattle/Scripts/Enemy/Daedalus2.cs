
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Daedalus2 : Enemy
{
    public Sprite WaxSprite;
    
    public int attackDamage = 8;
    public int SwapDamage = 12;
    public int MazeShitDamage = 10;
    public int KindledStrikeDamage = 15;
    public int KindledStrikeEachCubeDamage = 6;
    public int BurntWingsDamage = 12;
    public int WaxSprayDamage = 18;

    public int attackWeight = 20;
    public int SwapWeight = 25;
    public int MazeShiftWeight = 25;
    public int RekindleWeight = 40;
    public int BurntWingsWeight = 35;
    public int WaxSprayWeight = 65;

    public float attackCastingTime = 6;
    public float SwapCastingTime = 8;
    public float MazeShiftCastingTime = 10;
    public float RekindleCastingTime = 5;
    public float kindledStrikeCastingTime = 18;
    public float BurntWingsCastingTime = 8;
    public float WaxSprayCastingTime = 12;

    public int RekindledNum = 0;



    private enum SkillType { Attack, Swap, MazeShift, Rekindle,  kindledStrike, BurntWings, WaxSPray }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f, attackDamage);
                break;
            case SkillType.Swap:
                RefreshChoiceSectionBlock();
                soundManager.PlaySfx("Swap");
                break;
            case SkillType.MazeShift:
                soundManager.PlaySfx("Daedalus_Maze");
                Debug.Log("Will add with the start fill line with 1 block lost functions");
                break;
            case SkillType.Rekindle:
                soundManager.PlaySfx("Daedalus_Rekindle");
                RekindledNum += 3;
                break;
            case SkillType.BurntWings:
                soundManager.PlaySfx("Daedalus_Wing_Burn");
                DealAttackDamage(BurntWingsDamage);
                battleManager.PlayerGetOverheat(2);
                RekindledNum--; 
                break;
            case SkillType.WaxSPray:
                soundManager.PlaySfx("Daedalus_Wing");
                DealAttackDamage(WaxSprayDamage);
                WaxSpray();
                break;

        }
    }
    public void KindledStrike()
    {
        battleManager.KindledStrike(KindledStrikeDamage, KindledStrikeEachCubeDamage);

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
        if(RekindledNum==0)
        {
            int sum = attackWeight + SwapWeight + MazeShiftWeight+RekindleWeight;
            float attackProbability = (float)attackWeight / sum;
            float SwapProbability = (float)SwapWeight / sum;
            float MazeShiftProbability = (float)MazeShiftWeight / sum;

            float randomValue = Random.value;


            if (randomValue < attackProbability)
            {
                SkillCastingTime = attackCastingTime;  // Attack action
                nextSkill = SkillType.Attack;
                CurrentSkillIcons = new string[] { "Damage" };
                NextSkillDamage = attackDamage;

            }
            else if (randomValue < attackProbability + SwapProbability)
            {
                SkillCastingTime = SwapCastingTime;
                nextSkill = SkillType.Swap;
                CurrentSkillIcons = new string[] { "Interrupt", "Damage" };

            }
            else
            {
                SkillCastingTime = MazeShiftCastingTime;
                nextSkill = SkillType.MazeShift;
                CurrentSkillIcons = new string[] { "Interrupt", "Damage" };

            }
            nextMove = nextSkill.ToString();
        }
        else
        {
            int sum = BurntWingsWeight + WaxSprayWeight;
            float BurnWingsProbability = BurntWingsWeight + WaxSprayWeight;
            float randomValue = Random.value;

             if (randomValue < BurnWingsProbability)
            {
                SkillCastingTime = BurntWingsCastingTime;
                nextSkill = SkillType.BurntWings;
                CurrentSkillIcons = new string[] { "Interrupt", "Damage" };

            }
            else
            {
                SkillCastingTime = WaxSprayCastingTime;
                nextSkill = SkillType.WaxSPray;
                CurrentSkillIcons = new string[] { "Interrupt", "Damage" };

            }
            nextMove = nextSkill.ToString();


        }

    }
    public override void Start()
    {
        base.Start();
        StaggerReis = 3;
        SelectionToolUI selectionToolUI = FindObjectOfType<SelectionToolUI>();
        selectionToolUI.StartSingleBlockMode();
    }
    public override void deadhandle()
    {
        base.deadhandle();
        twoDto3D.TheLabyrinth();
    }
}
