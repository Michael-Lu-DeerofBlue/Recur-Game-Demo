using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    // Parent Class of enemy character
    public float moveFrequency = 4;
    public float moveAmplitude = 0.5f;
    public float HP;
    public float MaxHp;
    public float SkillCastingTime;
    public TextMeshPro enemyInfoText;
    public float timer;
    public HeroInfo heroInfo;
    public BattleManager battleManager;
    public bool PauseCasting = false; 
    public string nextMove;//name of the skill that will be executed next.
    public bool Fragiling = false;
    public bool isdead=false;
    public float PauseTime = 0;
    private ItemEventHandler itemEventHandler;
    private TargetSelector targetSelector;
    private TwoDto3D twoDto3D;
    private bool SpendingSkillAnim = false;
    public void Start()
    {
        targetSelector = FindObjectOfType<TargetSelector>();
        heroInfo = FindObjectOfType<HeroInfo>();
        battleManager = FindObjectOfType<BattleManager>();
        GetNextMove();//get casting time for the first turn.
        timer = SkillCastingTime; //added this so that the first move is executed with a timer
        itemEventHandler = FindObjectOfType<ItemEventHandler>();
        twoDto3D = FindObjectOfType<TwoDto3D>();


        Transform mySpriteTransform = FindChildByName(transform, "MySprite");
        if (mySpriteTransform != null)
        {
            StartCoroutine(AnimateSprite(mySpriteTransform,moveFrequency, moveAmplitude));
        }
    }
    public virtual void Update()
    {
        if (PauseCasting||SpendingSkillAnim)//the casting time will not decrease sometime because of debug, or during interruption of block game.
        {
            return;
        }

        if(battleManager.TimeStop==false)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0)
        {
            ExecuteTurn();
        }

        enemyInfoText.text = "HP: " + HP + "\nNext Move: " + nextMove + "\nTime to Execute Turn: " + timer.ToString("F2");

    }

    public virtual void ExecuteTurn()//all enemy will get casting time before they spend skill.
    {
        ExecuteSkill();
        GetNextMove();
        timer = SkillCastingTime;
        //after that, look at the update method. 
    }

    public virtual void ExecuteSkill()
    {

    }
    public virtual void GetNextMove()
    {
    }

    public virtual void ResetCasting()
    {
        // timetoExecuteTurn = skillcastingtime; view in child class
        timer = SkillCastingTime;
    }

    public virtual void Attack(float Damage)
    {
                heroInfo.HitHandle(Damage);
    }

    public void RandomDamageAttack(float DamageMin, float DamageMax)
    {
        float Damage= Random.Range(DamageMin, DamageMax);
            if (heroInfo != null)
            {
                heroInfo.HitHandle(Damage);
            }
            else
            {
                Debug.LogError("Hero object does not have a HeroInfo component.");
            }
        
    }
    public void AddBleeding()
    {
        heroInfo.AddBleeding();
    }


    public void DropDownblock(float second)
    {
        battleManager.DropDownBlock(second);
    }

    public void LockRotation()
    {
        // Access the BattleManager instance and set LockRotation to true
        battleManager.LockRotation();
    }

    public void LockRotationForNextBlock()
    {
        // Access the BattleManager instance and set LockRotation to true
        battleManager.LockRotationForNextBlock();
    }
    public void Heal(float healValue)
    {
        HitHandle(-healValue);
        if (HP >= MaxHp)
        {
            HP = MaxHp;
        }
    }
    public virtual void HitHandle(float damage)
    {
        if (Fragiling)
        {
            HP -= damage*1.5f;
            Debug.Log("Enemy is receving 1.5 times damage. HP: " + HP);
            Fragiling = false;
        }
        else
        {
            HP -= damage;
            
        }
        if (HP <= 0)
        {
            HP = 0;
            deadhandle();
        }
    }

    public virtual void deadhandle()
    {
        isdead = true;// to prevent the enemy from added into the existing enemy array or executing the turn after it is dead.
        targetSelector.SwitchTargetWhileIsDead();
        Enemy[] enemies = FindObjectsOfType<Enemy>().Where(enemy => !enemy.isdead).ToArray();
        if (enemies.Length == 0)
        {
            Debug.Log("Here");
            GameObject gameInstance = GameObject.Find("GameInstance");
            if (gameInstance != null) { gameInstance.GetComponent<TwoDto3D>().TwoDGameOver(); }
            battleManager.GameOver=true;
        }
        Destroy(gameObject);
    }
    public void RefreshChoiceSectionBlock()
    {
        battleManager.ReShapeSelectionBlock();
    }
    public virtual void OnMouseEnter()
    {

    }

    public virtual void OnMouseExit()
    {

    }

    public virtual void OnMouseDown()
    {
        if (battleManager.DisablePlayerInput == true) return;
        if (!isdead)
        {
            targetSelector.SwitchTargetByClick(this);
        }
    }

    public virtual void SelectedByPlayer()
    {
        heroInfo.selectedEnemy = (this);
    }


    public virtual void AttackScaleAnimation(float firstDuration, float firstScale, float secondDuration, float secondScale)
    {
        Transform mySpriteTransform = FindChildByName(transform, "MySprite");
        if (mySpriteTransform != null)
        {
            SpendingSkillAnim = true;
            StartCoroutine(ScaleSprite(mySpriteTransform, firstDuration, firstScale, secondDuration, secondScale));
        }
        else
        {
            Debug.LogWarning("MySprite not found!");
        }
    }

    private Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
            Transform found = FindChildByName(child, name);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    private IEnumerator ScaleSprite(Transform spriteTransform, float firstDuration, float firstScale, float secondDuration, float secondScale)
    {
        Vector3 originalScale = spriteTransform.localScale;
        Vector3 targetScale1 = originalScale * firstScale;
        Vector3 targetScale2 = originalScale * secondScale;

        float elapsedTime = 0f;


        while (elapsedTime < firstDuration)
        {
            spriteTransform.localScale = Vector3.Lerp(originalScale, targetScale1, elapsedTime / firstDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spriteTransform.localScale = targetScale1;

        elapsedTime = 0f;


        while (elapsedTime < secondDuration)
        {
            spriteTransform.localScale = Vector3.Lerp(targetScale1, targetScale2, elapsedTime / secondDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spriteTransform.localScale = targetScale2;


        elapsedTime = 0f;
        while (elapsedTime < secondDuration)
        {
            spriteTransform.localScale = Vector3.Lerp(targetScale2, originalScale, elapsedTime / secondDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spriteTransform.localScale = originalScale;

        SpendingSkillAnim = false;
    }

    private IEnumerator AnimateSprite(Transform spriteTransform, float frequency, float amplitude)
    {
        Vector3 startPosition = spriteTransform.localPosition;
        while (true)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
            spriteTransform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
            yield return null;
        }
    }
}

