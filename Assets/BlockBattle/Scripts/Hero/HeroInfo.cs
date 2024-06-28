using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Linq;
using System;
using UnityEngine.UIElements;

public class HeroInfo : MonoBehaviour
{
    // Parent Class of player character
    public float HitPoint;
    public float MaxHitPoint;
    private float MaxWeight;
    public TextMeshPro Hp;
    public BattleManager battleManager;
    public int parryCount=0;
    public Enemy selectedEnemy;
    private TargetSelector targetSelector;
    private TwoDto3D twoDto3D;
    public GameObject[] SkillIcon;
    private Vector3 initialPosition = new Vector3(50, 5, 0);
    public float horizontalSpacing =7.0f;
    private List<GameObject> generatedIcons = new List<GameObject>();

    // List to store pairs of index and clearNumber
    private List<(int index, int clearNumber)> iconQueue = new List<(int, int)>();



    // Start is called before the first frame update
    public virtual void Start()
    {
        twoDto3D = FindObjectOfType<TwoDto3D>();
        Hp.text = "HP: " + Mathf.RoundToInt(HitPoint).ToString();
        battleManager = FindObjectOfType<BattleManager>();
        targetSelector = FindObjectOfType<TargetSelector>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
       // some time it need the follow code to get the selected enemy.
        if(targetSelector.CurrentTarget != null)
        {
            selectedEnemy = targetSelector.CurrentTarget;
        }
    }

    public virtual void SetSelectedEnemy(Enemy Target)
    {
        selectedEnemy = Target;
    }
    public virtual void GenerateIcon(int index, int clearNumber)
    {
        if (index >= 0 && index < SkillIcon.Length)
        {
            // Generate the icon
            GameObject icon = Instantiate(SkillIcon[index], transform);
            Vector3 position = initialPosition - new Vector3(iconQueue.Count * horizontalSpacing, 0, 0);
            icon.transform.position = position;
            iconQueue.Add((index, clearNumber));
            generatedIcons.Add(icon);
            Debug.Log($"Icon generated at position: {position}");
        }
    }

    public virtual void ExecuteIconSkill()
    {
        StartCoroutine(ExecuteIconSkillCoroutine());
    }

    private IEnumerator ExecuteIconSkillCoroutine()
    {
        while (iconQueue.Count > 0)
        {
            yield return new WaitForSeconds(1f);

            (int index, int clearNumber) = iconQueue[0];
            ExecuteBehavior(index, clearNumber);
            iconQueue.RemoveAt(0);

            // Destroy the first generated icon
            if (generatedIcons.Count > 0)
            {
                Destroy(generatedIcons[0]);
                generatedIcons.RemoveAt(0);
            }
        }
        battleManager.ContinueBlockGame();
    }


    public virtual void ExecuteBehavior(int index, int clearNumber)
    //whatever the player character do, it will be executed here. 
    {
        switch (index)
            {
                case 0:
                    HandleIndex0(clearNumber);
                    break;
                case 1:
                    HandleIndex1(clearNumber);
                    break;
                case 2:
                    HandleIndex2(clearNumber);
                    break;
                case 3:
                    HandleIndex3(clearNumber);
                    break;
                case 4:
                    HandleIndex4(clearNumber);
                    break;
                case 5:
                    HandleIndex5(clearNumber);
                    break;
                case 6:
                    HandleIndex6(clearNumber);
                    break;
        }

    }

    public virtual void HitHandle(float damage)
    {
       if(parryCount > 0)
        {
            parryCount--;
            return;
        }
        HitPoint -= damage;
        Hp.text = "HP: " + Mathf.RoundToInt(HitPoint).ToString();
        Debug.Log("Player is hit. HP: " + HitPoint);
        if (HitPoint <= 0)
        {
            HitPoint = 0;
            twoDto3D.TwoDGameOver();
            Debug.Log("Player is dead.");
        }
    }

    public virtual void Fragile(float damage)
    {
        battleManager.AttackEnemy(damage, selectedEnemy);
        battleManager.FragileEnemy(damage, selectedEnemy);
    }

    public virtual void parry(int turnnumber)
    {
        parryCount+= turnnumber;
    }

    public virtual void AttackEnemy(float Damage)
    {
        battleManager.AttackEnemy(Damage,selectedEnemy);
    }

    public virtual async Task Zornhauy(float damagevalue)
    {
        if (selectedEnemy.HP <= damagevalue)
        {
            AttackEnemy(damagevalue);
            await Task.Delay(1000); // wait for 1 second
            await Zornhauy(damagevalue);
        }
        else
        {
            AttackEnemy(damagevalue);
        }
    }

    public virtual void Heal(float number)
    {
        HitPoint += number;
        if (HitPoint >= MaxHitPoint)
        {
            HitPoint = MaxHitPoint;
        }
        Hp.text = "HP: " + Mathf.RoundToInt(HitPoint).ToString();
        Debug.Log("Player is healed. HP: " + HitPoint);
    }


    public virtual void PauseSingleEnemyActionBar(float Delay)
    {
        battleManager.PuaseSingleEnemyActionBar(Delay, selectedEnemy);
    }

    public virtual void resetEnemyActionBar()
    {
        battleManager.ResetEnemyActionBar();
    }


    public virtual void CheckLandOn(int ColorIndex)
    {
        
    }

    public virtual void HandleIndex0(int clearNumber)
    {

    }
    public virtual void HandleIndex1(int clearNumber)
    {

    }
    public virtual void HandleIndex2(int clearNumber)
    {

    }
    public virtual void HandleIndex3(int clearNumber)
    {

    }
    public virtual void HandleIndex4(int clearNumber)
    {

    }
    public virtual void HandleIndex5(int clearNumber)
    {

    }
    public virtual void HandleIndex6(int clearNumber)
    {

    }







    //the old target select method. 

    //public virtual void CheckAndSelectEnemy()
    //{
    //    BlockManager blockManager = FindObjectOfType<BlockManager>();
    //    blockManager.StopClearBlock();
    //    battleManager.SelectingEnemy = true;
    //    StartCoroutine(GetEnemyTargetCoroutine());
    //}
    //private IEnumerator GetEnemyTargetCoroutine()
    //{
    //    selectedEnemy = null;
    //    BlockManager blockManager = FindObjectOfType<BlockManager>();
    //    blockManager.StopClearBlock();
    //    Enemy[] enemies = FindObjectsOfType<Enemy>().Where(enemy => !enemy.isdead).ToArray();
    //    if (enemies.Length == 1)
    //    {
    //        Enemy enemy = enemies[0].GetComponent<Enemy>();
    //        Debug.Log("Only one enemy in the scene.");
    //        selectedEnemy = enemy;
    //        blockManager.StartClearBlock();
    //        yield break; 
    //    }
    //    else if (enemies.Length > 1)
    //    {
    //        Debug.Log("More than one enemy in the scene.");

    //        yield return new WaitUntil(() => selectedEnemy != null);
    //        Debug.Log("Enemy selected.");
    //        battleManager.SelectingEnemy = false;
    //        blockManager.StartClearBlock();
    //    }
    //}


    //public virtual void CheckAndSelectZornhauy(float Damage)
    //{
    //    BlockManager blockManager = FindObjectOfType<BlockManager>();
    //    blockManager.StopClearBlock();
    //    battleManager.SelectingEnemy = true;
    //    StartCoroutine(GetZornhauyTargetCoroutine(Damage));
    //}
    //private IEnumerator GetZornhauyTargetCoroutine(float Damage)
    //{
    //    selectedEnemy = null;
    //    BlockManager blockManager = FindObjectOfType<BlockManager>();
    //    blockManager.StopClearBlock();
    //    Enemy[] enemies = FindObjectsOfType<Enemy>().Where(enemy => !enemy.isdead).ToArray();
    //    if (enemies.Length == 1)
    //    {
    //        Enemy enemy = enemies[0].GetComponent<Enemy>();
    //        Debug.Log("Only one enemy in the scene.");
    //        selectedEnemy = enemy;
    //        if (selectedEnemy.HP<=Damage)
    //        {
    //            selectedEnemy.HitHandle(Damage);
    //            CheckAndSelectZornhauy(Damage);
    //        }
    //        else
    //        {
    //            HitHandle(Damage);
    //        }
    //        yield break;
    //    }
    //    else if (enemies.Length > 1)
    //    {
    //        Debug.Log("More than one enemy in the scene.");

    //        yield return new WaitUntil(() => selectedEnemy != null);
    //        Debug.Log("Enemy selected.");
    //        battleManager.SelectingEnemy = false;
    //        if (selectedEnemy.HP <= Damage)
    //        {
    //            selectedEnemy.HitHandle(Damage);
    //            CheckAndSelectZornhauy(Damage);
    //        }
    //        else
    //        {
    //            selectedEnemy.HitHandle(Damage);
    //        }
    //    }
    //}
}