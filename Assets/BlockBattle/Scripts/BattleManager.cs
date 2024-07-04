using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class BattleManager : MonoBehaviour
{
    //Manage various situations in various battles, such as buffs, debuffs, etc.
    public SpawnBlock spawnBlock;
    private HeroInfo heroInfo;
    public float Timer;
    public SelectionTool selectionTool;
    public IntTranslator inttranslator;
    public bool TimeStop = false;
    public bool GameOver = false;

    //Player status:
    public bool PlayerLandOn = false;

    //Player debuff type:
    public bool DisablePlayerInput = false;
    //when add new debuff type, remember to add a bool to "RemoveAllPlayerDebuff" method.
    public bool RotationLocked = false;
    public bool LockNextBlockRotation = false;
    public bool DropCountDown = false;
    public static bool refreshedBlocks = false;

    //Player buffer Type:
    public bool PlayerImmuingDebuff = false;

    // enemey debuff type:

    //enemy status:
    public bool WeakMinionCompanionsOnHold = false;
    void Start()
    {
        heroInfo = FindObjectOfType<HeroInfo>();
        inttranslator = FindObjectOfType<IntTranslator>();
        spawnBlock = FindObjectOfType<SpawnBlock>();
    }

    // Update is called once per frame
    void Update()
    {

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
    }

    public void AddStunBlock(int Num, int ColorIndex)
    {
       selectionTool = FindObjectOfType<SelectionTool>();
       selectionTool.AddDebuffBlock(Num, ColorIndex);
    }
    public void AttackEnemy(float damage, Enemy Target)
    {
        Target.HitHandle(damage);
    }
    public void FragileEnemy(float damage, Enemy Target)
    {
        Target.Fragiling = true;
    }
    public void ZornhauyEnmey(float damage, Enemy Target)
    {
        if (Target.HP <= damage)
        {
            Target.HitHandle(damage);
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
        RotationLocked = false;
        LockNextBlockRotation = false;
        DropCountDown = false;
        refreshedBlocks = false;
    }

    public void PlayerImmueDebuffDuring(float second)
    {
        StartCoroutine(ImmueAllDebuffAfterDelay(second));
                PlayerImmuingDebuff = true;
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
        TimeStop = true;
        //  heroInfo.CheckAndSelectEnemy();
    }
    public void ContinueBlockGame()//called when start to select enemy.
    {
        TimeStop = false;
        BlockManager blockManager = FindObjectOfType<BlockManager>();
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
