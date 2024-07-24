using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArtemisRaged: Enemy
{
    public Enemy[] SummonenemyPrefabs;
    public int attackDamage = 12;
    public int ChariotofGoldenDamage = 20;
    public int GoldenArrowsDamage = 12;

    public int attackWeight = 25;
    public int TakeAimWeight = 20;
    public int ChariotOfGoldenWeight = 30;
    public int CallOfTheWildWeight = 15;
    public int GoldenArrowsWeight = 10;

    public int attackCastingTime = 6;
    public int TakeAimCastingTime = 3;
    public int ChariotOfGoldenCastingTime = 8;
    public int CallOfTheWildCastingTime = 2;
    public int GoldenArrowsCastingTime = 12;



    private enum SkillType { Attack, TakeAim, ChariotOfGolden, CallOfTheWild, GoldenArrows }
    private SkillType nextSkill;

    public override void ExecuteSkill()
    {
        switch (nextSkill)
        {
            case SkillType.Attack:
                AttackScaleAnimation(0.2f, 1.3f, 0.6f, 1.0f, attackDamage);
                break;
            case SkillType.TakeAim:
                battleManager.FragilePlayer(1);
                break;
            case SkillType.ChariotOfGolden:
                DealAttackDamage(ChariotofGoldenDamage);
                Enemy[] allEnemies = FindObjectsOfType<Enemy>();
                foreach (Enemy enemy in allEnemies)
                {
                    if (enemy.isdead == false)
                    {
                        enemy.Heal(ChariotofGoldenDamage / 2);
                    }
                }
                break;
            case SkillType.CallOfTheWild:
                CallofTheWind();
                battleManager.SpeedUpCastingAllEnimes(20);
                break;
            case SkillType.GoldenArrows:
                Enemy[] Enemies = FindObjectsOfType<Enemy>();
                List<Enemy> existingEnemiesList = new List<Enemy>(Enemies);
                existingEnemiesList.Remove(this);

                // Convert the list back to an array
                Enemy[] existingEnemiesExcludingSelf = existingEnemiesList.ToArray();
                int TargetCount = existingEnemiesExcludingSelf.Length + 1; // Add 1 for the current enemy count
                DealAttackDamage(TargetCount * GoldenArrowsDamage);

                foreach (Enemy enemy in existingEnemiesExcludingSelf)
                {
                    if (!enemy.isdead)
                    {
                        enemy.HitHandle(TargetCount * GoldenArrowsDamage);
                    }
                }
                Debug.Log("golden arrow" + TargetCount * GoldenArrowsDamage);
                break;
        }
    }

    public void CallofTheWind()
    {
        int RandomIndex = Random.Range(0, SummonenemyPrefabs.Length);
        Enemy summonedEnemy = Instantiate(SummonenemyPrefabs[RandomIndex], new Vector3(0, 0, 0), Quaternion.identity);
        EnemyLayOutManager enemyLayOutManager = FindObjectOfType<EnemyLayOutManager>();
        enemyLayOutManager.RelayoutEnemies();
        Enemy[] alEnemies = FindObjectsOfType<Enemy>();
        if (alEnemies.Length < 8)
        {
            CallofTheWind();
        }
    }
    public override void GetNextMove()
    {
        if (HP / MaxHp <= 0.5f)
        {
            int sum = attackWeight + TakeAimWeight + ChariotOfGoldenWeight + CallOfTheWildWeight + GoldenArrowsWeight;
            float attackProbability = (float)attackWeight / sum;
            float TakeAimProbability = (float)TakeAimWeight / sum;
            float ChariotOfGoldenCProbability = (float)ChariotOfGoldenWeight / sum;
            float CallOfTheWildProbability = (float)CallOfTheWildWeight / sum;
            float GoldenArrowsCastingTime = (float)GoldenArrowsWeight / sum;

            float randomValue = Random.value;
            Enemy[] Enemies = FindObjectsOfType<Enemy>();
            if (Enemies.Length >= 8)
            {
                CallOfTheWildProbability = 0;
            }


            if (randomValue < attackProbability)
            {
                SkillCastingTime = attackCastingTime;  // Attack action
                nextSkill = SkillType.Attack;
                CurrentSkillIcons = new string[] { "Damage" };
                NextSkillDamage = attackDamage;

            }
            else if (randomValue < attackProbability + TakeAimProbability)
            {
                SkillCastingTime = TakeAimCastingTime;
                nextSkill = SkillType.TakeAim;
                CurrentSkillIcons = new string[] { "Debuff" };

            }
            else if (randomValue < attackProbability + TakeAimProbability + ChariotOfGoldenCProbability)
            {
                SkillCastingTime = ChariotOfGoldenCastingTime;
                nextSkill = SkillType.ChariotOfGolden;
                CurrentSkillIcons = new string[] { "Damage", "Heal" };

            }
            else if (randomValue < attackProbability + TakeAimProbability + ChariotOfGoldenCProbability + CallOfTheWildProbability)
            {
                SkillCastingTime = CallOfTheWildCastingTime;
                nextSkill = SkillType.CallOfTheWild;
                CurrentSkillIcons = new string[] { "Damage", "Heal" };

            }
            else
            {
                SkillCastingTime = GoldenArrowsCastingTime;
                nextSkill = SkillType.GoldenArrows;
                CurrentSkillIcons = new string[] { "Damage" };

            }
            nextMove = nextSkill.ToString();
        }
        else
        {
            int sum = attackWeight + TakeAimWeight + ChariotOfGoldenWeight;
            float attackProbability = (float)attackWeight / sum;
            float TakeAimProbability = (float)TakeAimWeight / sum;
            float ChariotOfGoldenCProbability = (float)ChariotOfGoldenWeight / sum;


            float randomValue = Random.value;
            if (randomValue < attackProbability)
            {
                SkillCastingTime = attackCastingTime;  // Attack action
                nextSkill = SkillType.Attack;
                CurrentSkillIcons = new string[] { "Damage" };
                NextSkillDamage = attackDamage;

            }
            else if (randomValue < attackProbability + TakeAimProbability)
            {
                SkillCastingTime = TakeAimCastingTime;
                nextSkill = SkillType.TakeAim;
                CurrentSkillIcons = new string[] { "Debuff" };

            }
            else
            {
                SkillCastingTime = ChariotOfGoldenCastingTime;
                nextSkill = SkillType.ChariotOfGolden;
                CurrentSkillIcons = new string[] { "Damage", "Heal" };

            }
            nextMove = nextSkill.ToString();
        }
    }


    public override void HitHandle(float damage)
    {
        base.HitHandle(damage);
        if (HP / MaxHp <= 0.5 && isdead == false)
        {
            SkillCastingTime = CallOfTheWildCastingTime;
            nextSkill = SkillType.CallOfTheWild;
            CurrentSkillIcons = new string[] { "Damage", "Heal" };
            nextMove = nextSkill.ToString();
        }
    }
    public override void Start()
    {
        base.Start();
        StaggerReis = 3;
}
}

