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
    public string DisplayName;
    public TextMeshPro enemyInfoText;
    public float timer;
    public HeroInfo heroInfo;
    public BattleManager battleManager;
    public string nextMove;//name of the skill that will be executed next.
    public bool isdead=false;
    private TargetSelector targetSelector;
    private TwoDto3D twoDto3D;
    private bool SpendingSkillAnim = false;
    private GameObject enemyUIInstance;
    
    //Enemy debuff type:
    public int FragilingNum = 0;
    public float PauseTime = 0;
    public bool PauseCasting = false;

    public string[] CurrentSkillIcons;
    private TextMeshProUGUI EnemyName;
    private Image hpBar;
    private Image castingBar;
    private Image nextSkillIcon;
    private Image UIBG;
    private Sprite originalUISprite;
    public Image CurrentIcon;

    public void Start()
    {
        targetSelector = FindObjectOfType<TargetSelector>();
        heroInfo = FindObjectOfType<HeroInfo>();
        battleManager = FindObjectOfType<BattleManager>();
        GetNextMove();//get casting time for the first turn.
        timer = SkillCastingTime; //added this so that the first move is executed with a timer
        twoDto3D = FindObjectOfType<TwoDto3D>();

        CreateEnemyUI();

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
        UpdateEnemyUI();
    }



    private void CreateEnemyUI()
    {
        if (battleManager != null && battleManager.EnemyUI != null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {

                Vector3 screenPosition = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y - 6, transform.position.z));
                enemyUIInstance = Instantiate(battleManager.EnemyUI, screenPosition, Quaternion.identity);
                enemyUIInstance.transform.SetParent(canvas.transform, false);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPosition, Camera.main, out Vector2 canvasPosition);
                enemyUIInstance.GetComponent<RectTransform>().anchoredPosition = canvasPosition;
                CurrentIcon= FindChildByName(enemyUIInstance.transform,"NextSkillIcon").GetComponent<Image>();
                hpBar = FindChildByName(enemyUIInstance.transform, "HPBar").GetComponent<Image>();
                castingBar = FindChildByName(enemyUIInstance.transform, "CastingBar").GetComponent<Image>();
                UIBG= FindChildByName(enemyUIInstance.transform, "UntargetBG").GetComponent<Image>();
                EnemyName=FindChildByName(enemyUIInstance.transform, "EnemyName").GetComponent<TextMeshProUGUI>();
                originalUISprite =UIBG.sprite;
                if(DisplayName!=null)
                {
                    EnemyName.text = DisplayName;
                }

            }
            else
            {
                Debug.LogError("Canvas not found in the scene.");
            }
        }
        else
        {
            Debug.LogError("BattleManager or EnemyUI prefab not assigned.");
        }
        DisplaySkillIcon();
    }
    private void UpdateEnemyUI()
    {
        if (enemyUIInstance != null)
        {
            hpBar.fillAmount = HP / MaxHp;
            castingBar.fillAmount = (timer - SkillCastingTime) / (-SkillCastingTime);
        }
    }

    public virtual void ExecuteTurn()//all enemy will get casting time before they spend skill.
    {
        ExecuteSkill();
        GetNextMove();
        if (CurrentSkillIcons.Length != 0)
        {
            DisplaySkillIcon();
        }
        else
        {
            nextSkillIcon.sprite = null;
        }
        timer = SkillCastingTime;
        //after that, look at the update method. 
    }
    public virtual void DisplaySkillIcon()
    {
        if (CurrentSkillIcons != null && CurrentSkillIcons.Length > 0)
        {
            List<Sprite> foundSprites = new List<Sprite>();

            foreach (string iconName in CurrentSkillIcons)
            {
                Sprite foundSprite = battleManager.EnemySkillIconsList.Find(sprite => sprite.name == iconName);
                if (foundSprite != null)
                {
                    foundSprites.Add(foundSprite);
                }
                else
                {
                    Debug.Log($"No matching sprite found for: {iconName}");
                }
            }

            if (foundSprites.Count > 0)
            {
                if (foundSprites.Count == 1)
                {
                    CurrentIcon.sprite = foundSprites[0];
                }
                else
                {
                    StartCoroutine(DisplayMultipleIcons(foundSprites));
                }
            }
        }
    }

    private IEnumerator DisplayMultipleIcons(List<Sprite> sprites)
    {
        int index = 0;
        while (true)
        {
            CurrentIcon.sprite = sprites[index];
            yield return new WaitForSeconds(1.5f);
            index = (index + 1) % sprites.Count;
        }
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
        if (FragilingNum > 0)
        {
            HP -= damage * 1.5f;
            FragilingNum--;
            UpdateEnemyUI();
            enemyInfoText.text = "HP: " + HP + "\nNext Move: " + nextMove + "\nTime to Execute Turn: " + timer.ToString("F2");


        }
        else if(FragilingNum == 0)
        {
            HP -= damage;
            UpdateEnemyUI();
            enemyInfoText.text = "HP: " + HP + "\nNext Move: " + nextMove + "\nTime to Execute Turn: " + timer.ToString("F2");

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
        Destroy(enemyUIInstance);
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
        UIBG = FindChildByName(enemyUIInstance.transform, "UntargetBG").GetComponent<Image>();
        UIBG.sprite = battleManager.TargetUIBGSprite;


        RectTransform uiRectTransform = enemyUIInstance.GetComponent<RectTransform>();
        if (uiRectTransform != null)
        {
            uiRectTransform.localScale = new Vector3(1.6f, 1.6f, 1);
        }
        Enemy[] foundEnemies = FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in foundEnemies)
        {
            if (enemy != this.GetComponent<Enemy>())
            {
                enemy.UnselectedByPlayer();
            }
        }
    }

    public virtual void UnselectedByPlayer()
    {
        UIBG.sprite = originalUISprite;
        RectTransform uiRectTransform = enemyUIInstance.GetComponent<RectTransform>();
        if (uiRectTransform != null)
        {
            uiRectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

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

