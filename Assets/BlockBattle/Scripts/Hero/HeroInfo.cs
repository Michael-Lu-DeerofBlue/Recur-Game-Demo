using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Linq;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms;

public class HeroInfo : MonoBehaviour
{
    // Parent Class of player character
    public float HitPoint=100;
    public float MaxHitPoint=100;
    public GameObject PlayerHpBar;
    public GameObject[] SkillICoinsHolder;
    private List<(int index, int clearNumber, int holderIndex)> iconQueue = new List<(int, int, int)>();
    private Queue<(int index, int clearNumber)> extraIconsQueue = new Queue<(int, int)>();
    private Image hpBarImage;
    private float MaxWeight;
    public TextMeshPro Hp;
    public BattleManager battleManager;
    public int parryCount=0;
    public Enemy selectedEnemy;
    private TargetSelector targetSelector;
    private TwoDto3D twoDto3D;
    public Sprite[] SkillIcon;
    public float horizontalSpacing =7.0f;
    private DamageNumber damageNumber;
    private List<IEnumerator> bleedingCoroutines = new List<IEnumerator>();
    private SoundManager soundManager;
    // List to store pairs of index and clearNumber



    // Start is called before the first frame update
    public virtual void Start()
    {
        twoDto3D = FindObjectOfType<TwoDto3D>();
        Hp.text = "HP: " + Mathf.RoundToInt(HitPoint).ToString();
        battleManager = FindObjectOfType<BattleManager>();
        targetSelector = FindObjectOfType<TargetSelector>();
        damageNumber = FindObjectOfType<DamageNumber>();
        hpBarImage = PlayerHpBar.GetComponent<Image>();
        soundManager = FindObjectOfType<SoundManager>();

    }

    // Update is called once per frame
    public virtual void Update()
    {
       // some time it need the follow code to get the selected enemy.
        if(targetSelector.CurrentTarget != null)
        {
            selectedEnemy = targetSelector.CurrentTarget;
        }
        hpBarImage.fillAmount = HitPoint / MaxHitPoint;
    }

    public virtual void SetSelectedEnemy(Enemy Target)
    {
        selectedEnemy = Target;
    }
    public virtual void GenerateIcon(int index, int clearNumber)
    {
        if (index < 0 || index >= SkillIcon.Length)
        {
            Debug.LogError("Index out of range of SkillIcon array.");
            return;
        }

        Sprite targetSprite = SkillIcon[index];

        if (iconQueue.Count < SkillICoinsHolder.Length)
        {
            for (int i = 0; i < SkillICoinsHolder.Length; i++)
            {
                SpriteRenderer sr = SkillICoinsHolder[i].GetComponent<SpriteRenderer>();

                if (sr == null)
                {
                    Debug.LogWarning("SkillICoinsHolder element does not have a SpriteRenderer component.");
                    continue;
                }

                if (sr.sprite == null)
                {
                    sr.sprite = targetSprite;
                    iconQueue.Add((index, clearNumber, i));
                    Debug.Log("add actionqueue. " + index + " clearnumber: " + clearNumber);
                    return;
                }
            }
        }
        else
        {
            extraIconsQueue.Enqueue((index, clearNumber));
            Debug.Log("add extra queue. " + index + " clearnumber: " + clearNumber);
        }

        Debug.LogWarning("No empty SpriteRenderer found in SkillICoinsHolder.");
    }

    public virtual void ExecuteIconSkill()
    {
        if (battleManager.GameOver)
        {
            return;
        }
        StartCoroutine(ExecuteIconSkillCoroutine());
    }

    private IEnumerator ExecuteIconSkillCoroutine()
    {
        while (iconQueue.Count > 0)
        {
            yield return new WaitForSeconds(1f);

            (int index, int clearNumber, int holderIndex) = iconQueue[0];
            ExecuteBehavior(index, clearNumber);
            iconQueue.RemoveAt(0);


            SpriteRenderer sr = SkillICoinsHolder[holderIndex].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = null;
            }
        }


        if (iconQueue.Count == 0 && extraIconsQueue.Count > 0)
        {
            AddExtraIconsToQueue();
            ExecuteIconSkill();
        }
        if(iconQueue.Count==0 && extraIconsQueue.Count == 0)
        {
            battleManager.ContinueBlockGame();
        }

    }

    private void AddExtraIconsToQueue()
    {
        int count = Mathf.Min(SkillICoinsHolder.Length, extraIconsQueue.Count);
        for (int i = 0; i < count; i++)
        {
            var (extraIndex, extraClearNumber) = extraIconsQueue.Dequeue();
            GenerateIcon(extraIndex, extraClearNumber);
        }
    }



    public virtual void ExecuteBehavior(int index, int clearNumber)
    //whatever the player character do, it will be executed here. 
    {
        if(battleManager.GameOver)
        {
            return;
        }
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
                case 8:
                    HandleIndex8(clearNumber);
                    break;
                case 9:
                    HandleIndex9(clearNumber);
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
        damageNumber.ShowDamageNumber(damage);
        soundManager.PlaySound("PlayerTakeDamage");
        if (HitPoint <= 0)
        {
            HitPoint = 0;
            GameObject gameInstance = GameObject.Find("GameInstance");
            if (gameInstance != null) { gameInstance.GetComponent<TwoDto3D>().TwoDGameOver(); }
            Debug.Log("Player is dead.");
        }
    }

    public virtual void Fragile(float damage)
    {
        battleManager.AttackEnemy(damage, selectedEnemy);
        battleManager.FragileEnemy(selectedEnemy);
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
    public virtual void RemoveDebuff(int num)
    {
        battleManager.RemovePlayerDebug(num);
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

    public virtual void HandleIndex8(int clearNumber)
    {
        Debug.Log("gray, stun, do nothing");
    }

    public virtual void HandleIndex9(int clearNumber)
    {
        switch (clearNumber)
        {
            case 1:

                HitHandle(5);
                break;
            case 2:
                HitHandle(6);
                break;
            case 3:
                HitHandle(7);
                break;
            case 4:
                HitHandle(8);
                break;
            default:
                // Handle unexpected clearNumber here
                break;
        }


    }

    // Function to call when the bleeding effect should start
    public void AddBleeding()
    {
        IEnumerator bleedingCoroutine = BleedingEffect();
        bleedingCoroutines.Add(bleedingCoroutine);
        StartCoroutine(bleedingCoroutine);
        battleManager.BleedingCount++;
    }

    // Coroutine for bleeding effect
    private IEnumerator BleedingEffect()
    {
        while (true)
        {
            while (battleManager.BlockGameTimeStop)
            {
                yield return null; 
            }

            yield return new WaitForSeconds(5f);

            if (!battleManager.BlockGameTimeStop)
            {
                HitHandle(1);
                Debug.Log("HP: " + HitPoint);
            }
        }
    }

    // Function to stop one bleeding effect
    public void StopOneBleeding()
    {
        if (bleedingCoroutines.Count > 0)
        {
            StopCoroutine(bleedingCoroutines[0]);
            bleedingCoroutines.RemoveAt(0);
        }
    }

    // Optional: Stop all bleeding effects (for example when the character dies)
    public void StopAllBleeding()
    {
        foreach (var coroutine in bleedingCoroutines)
        {
            StopCoroutine(coroutine);
        }
        bleedingCoroutines.Clear();
    }
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