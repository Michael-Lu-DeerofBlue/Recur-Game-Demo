using Fungus;
using PixelCrushers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class BattleManager : MonoBehaviour
{
    //Manage various situations in various battles, such as buffs, debuffs, etc.
    public GameObject EnemyUI;
    public GameObject EnemyDamageNumUI;
    public Sprite TargetUIBGSprite;
    public List<Sprite> EnemySkillIconsList = new List<Sprite>();
    public SpawnBlock spawnBlock;
    private HeroInfo heroInfo;
    public float Timer;
    public SelectionTool selectionTool;
    public IntTranslator inttranslator;
    public bool BlockGameTimeStop = false;
    public bool GameOver = false;
    private SoundManager soundManager;

    //Player status:
    public bool PlayerLandOn = false;
    public int clearedline = 0;


    //when add new debuff type, remember to add a bool to "RemoveAllPlayerDebuff" method.
    //Player debuff type:
    public int BleedingCount = 0;  
    public bool DisablePlayerInput = false;
    public bool RotationLocked = false;
    public bool LockNextBlockRotation = false;
    public bool DropCountDown = false;
    public static bool refreshedBlocks = false;

    //Player buff Type:
    public bool PlayerImmuingDebuff = false;
    public int CriticalNum = 0;

    // enemey debuff type: Fragiling 写在 Enemy上， 可以直接set Enemy.FragilingNum
    
   // PauseCasting in enemy.cs
    //enemy status:
    public bool WeakMinionCompanionsOnHold = false;
    public static bool firstCombat = false;


    //Ui stuff
    public int ToolTipsLevel=0;
    void Start()
    {
     //   FirstCombat();
        heroInfo = FindObjectOfType<HeroInfo>();
        inttranslator = FindObjectOfType<IntTranslator>();
        spawnBlock = FindObjectOfType<SpawnBlock>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        soundManager = FindObjectOfType<SoundManager>();
        soundManager.PlaySfx("CombatEnter");

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FirstCombat()
    {
        if (ES3.KeyExists("First Combat"))
        {
            if (ES3.Load<bool>("First Combat"))
            {
                firstCombat = true;
                Debug.Log("Change Layout for First Combat");
            }
        }
    }


    public void ReceColorMessage(string Colortofind, int clearNumber)
    {
        int index = -1;

        if (inttranslator != null)
        {
            for (int i = 0; i < inttranslator.Colors.Length; i++)
            {
                string hexColor = ColorUtility.ToHtmlStringRGBA(inttranslator.Colors[i]);
                if (hexColor == Colortofind)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                Debug.Log($"Color found at index: {index}" + "clearNumber: " + clearNumber);
            }
            else
            {
                Debug.Log("Color not found." + Colortofind);
            }
        }
        if (index > 7)
        {
            heroInfo.ExecuteBehavior(index, clearNumber);//debuff will not have icon.
        }
        heroInfo.GenerateIcon(index, clearNumber);
    }

    public void ExecuteIconSkill()
    {
        heroInfo.ExecuteIconSkill();
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "BattleLevel - per - tutorial")
        {
            GameObject controller = GameObject.Find("tip controller");
            if (controller != null && controller.GetComponent<HintController>().currentIndex == 6) { controller.GetComponent<HintController>().WaitAndSwitch(); }
        }
    }

    public void AddStunBlock(int Num, int ColorIndex)
    {
       selectionTool = FindObjectOfType<SelectionTool>();
       selectionTool.AddDebuffBlock(Num, ColorIndex);
    }
    public void AttackEnemy(float damage, Enemy Target)
    {
        if (!firstCombat)
        {
            if(CriticalNum >= 1)
            {
                damage = damage * 1.5f* CriticalNum;
                Target.HitHandle(damage);
                CriticalNum=0;
            }else if(CriticalNum == 0) {
                string currentSceneName = SceneManager.GetActiveScene().name;
                if (currentSceneName != "BattleLevel - tutorial")
                {
                    Target.HitHandle(damage);
                }
            }

        }
    }
    public void FragilePlayer(int FragileNum) 
    {
        heroInfo.FragiledByEnemy(FragileNum);
    }
    public void FragileEnemy(Enemy Target)
    {
            Target.FragilingNum++;
    }
    public void ZornhauyEnmey(float damage, Enemy Target)
    {
        if (Target.HP <= damage)
        {
            Target.HitHandle(damage);
        }
        
    }

    public void SpeedUpCastingAllEnimes(float duration)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.CastingSpeedRate = 1.5f;
        }
        StartCoroutine(ResetCastingSpeed(enemies, duration));
    }
    private IEnumerator ResetCastingSpeed(Enemy[] enemies, float duration)
    {
        yield return new WaitForSeconds(duration);

        foreach (Enemy enemy in enemies)
        {
            enemy.CastingSpeedRate = 1.0f;
        }
    }

    public void disableInputForSeconds(float seconds)
    {
        StartCoroutine(CoroutineDisableInputForSeconds(seconds));
    }

    private IEnumerator CoroutineDisableInputForSeconds(float seconds)
    {

        DisablePlayerInput = true;
        yield return new WaitForSeconds(seconds);
        DisablePlayerInput = false;
    }
    public void RemoveAllPlayerDebug()
    {
        DisablePlayerInput = false;
        RotationLocked = false;
        LockNextBlockRotation = false;
        DropCountDown = false;
        refreshedBlocks = false;
        heroInfo.StopAllBleeding();
        BleedingCount = 0;
}

    public void RemovePlayerDebug(int num)
    {
        List<System.Action> trueFlags = new List<System.Action>();

        if (DisablePlayerInput) trueFlags.Add(() => DisablePlayerInput = false);
        if (RotationLocked) trueFlags.Add(() => RotationLocked = false);
        if (LockNextBlockRotation) trueFlags.Add(() => LockNextBlockRotation = false);
        if (DropCountDown) trueFlags.Add(() => DropCountDown = false);
        if (refreshedBlocks) trueFlags.Add(() => refreshedBlocks = false);
        for (int i = 0; i < BleedingCount; i++)    //每一层bleeding 算作一个debuff。 
        {
            trueFlags.Add(() =>
            {
                heroInfo.StopOneBleeding();
                BleedingCount--;
            });
        }


        System.Random rand = new System.Random();

        while (num > 0 && trueFlags.Count > 0)
        {
            int index = rand.Next(trueFlags.Count);
            trueFlags[index]();
            trueFlags.RemoveAt(index);
            num--;
        }
    }
    public void PlayerImmueDebuffDuring(float second)
    {
        StartCoroutine(ImmueAllDebuffAfterDelay(second));
                PlayerImmuingDebuff = true;
    }
    public void HandleStickerEffect(String stickerName)
    {
        switch (stickerName)
        {
            case "Critical":
                CriticalNum++;
                Debug.Log("CriticalCriticalCriticalCriticalCriticalCritical");
                return;
            case "Piercing":
                FragileEnemy(heroInfo.selectedEnemy);
                return;
            case "Sober":
                Debug.Log("SoberStickerSoberStickerSoberStickerSoberSticker");
                return;
            case "Swordmaster":
                Debug.Log("SwordmasterSwordmasterSwordmasterSwordmasterSwordmaster");
                return;
            case "Gunslinger":
                Debug.Log("GunslingerGunslingerGunslingerGunslingerGunslingerGunslinger");
                return;



        }
    }
    private IEnumerator ImmueAllDebuffAfterDelay(float seconds)
    {
        PlayerImmuingDebuff = true;  
        yield return new WaitForSeconds(seconds);  
        PlayerImmuingDebuff = false;  
    }
 

    public void DropDownBlock(float second)
    //we can just set the bool when doing remove rebug instead of using coroutine.
    {
        if(PlayerImmuingDebuff == false)
        {
            StartCoroutine(DropDownAfterDelay(second));
            DropCountDown = true;
        }
    }

    private IEnumerator DropDownAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (DropCountDown)
        {
            BlockManager blockManager = FindObjectOfType<BlockManager>();
            blockManager.DropDown();
        }
        DropCountDown = false;
    }
    public void IntrruptBlockGame()//called when start to select enemy.
    {
        BlockGameTimeStop = true;
    }

    public void CheckAndPlayLineCleardSound()
    {
        if (clearedline == 0)
        {
        }
        if (clearedline == 1)
        {
            soundManager.PlaySfx("ClearOneLine");
        }
        if (clearedline == 2)
        {
            soundManager.PlaySfx("ClearTwoLines");
        }
        if (clearedline == 3)
        {
            soundManager.PlaySfx("ClearTwoLines");

        }
        if (clearedline == 4)
        {
            soundManager.PlaySfx("ClearFourLines");
        }
    }
    public void ContinueBlockGame()//called when start to select enemy.
    {
        BlockGameTimeStop = false;
        clearedline = 0;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemies)
        {
           if( enemy.FragilingNum>0 )
            {
                enemy.FragilingNum--;
            }
        }

    }
    public void addclearedlineNumber()
    {
        clearedline++;
    }


    public void LockRotation()
    {
        if(PlayerImmuingDebuff == false)
        {
            RotationLocked = true;
            StartCoroutine(ResetLockRotation());
        }
    }

    public void WeakMinoCompanionsStart()
    {
        WeakMinionCompanionsOnHold = true;
    }
    public void CheckCompanions()
    {
        WeakMinion[] WeakMinions = FindObjectsOfType<WeakMinion>();
        foreach (WeakMinion minion in WeakMinions)
        {
            if (!minion.waiting)
            {
                return;
            }
        }
        WeakMinion[] WeakMinions_a = FindObjectsOfType<WeakMinion>();
        foreach (WeakMinion minion in WeakMinions_a)
        {
            minion.DoCampanions();
        }
        WeakMinionCompanionsOnHold = false;
    }

    public void LockRotationForNextBlock()
    //set the bool instead of method, aviod to reset in 3 seconds.
    {
        if (PlayerImmuingDebuff == false)
        {
            LockNextBlockRotation = true;
        }
    }
    private IEnumerator ResetLockRotation()
    {
        yield return new WaitForSeconds(3);
        RotationLocked = false;
    }

    public void PuaseSingleEnemyActionBar(float PausePeriod, Enemy Target)
    {
        Target.PauseTime = +PausePeriod;
        Target.PauseCasting = true;
        StartCoroutine(ResumeEnemyActionBarAfterDelay(Target));
        
    }

    private IEnumerator ResumeEnemyActionBarAfterDelay(Enemy Target)
    {
        while (Target.PauseTime > 0)
        {
            yield return new WaitForSeconds(1);
            Target.PauseTime -= 1;
        }
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.PauseCasting = false;
        }

    }
    public void ReShapeSelectionBlock()
    {
        if (!PlayerImmuingDebuff)
        {
            refreshedBlocks = true;
            selectionTool = FindObjectOfType<SelectionTool>();
            selectionTool.ReShapeSelectionBlocks();
        }
    }



    public void ResetEnemyActionBar()
    {
        Enemy enemy = FindObjectOfType<Enemy>();//for now, we only have one enemy in default, have to modify that after we have method to choice enmey target.
        enemy.ResetCasting();
    }



    public void CheckOnLand(string BlockColor)
    {
        if (inttranslator != null)
        {
            for (int i = 0; i < inttranslator.Colors.Length; i++)
            {
                string hexColor = ColorUtility.ToHtmlStringRGBA(inttranslator.Colors[i]);
                if (hexColor == BlockColor)
                {
                    heroInfo.CheckLandOn(i);
                    break;
                }
            }

        }
    }


}
