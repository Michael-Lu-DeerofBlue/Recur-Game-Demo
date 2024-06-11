using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class BattleManager : MonoBehaviour
{
    private HeroInfo heroInfo;
    public float Timer;
    public SelectionTool selectionTool;
    public IntTranslator inttranslator;

    //Player debug type:
    public bool RotationLocked=false;
    public bool LockNextBlockRotation = false;
    public bool DropCountDown = false;

    // enemey debug type:
    public float PauseTime = 0;//enemy action bar pause time.

    void Start()
    {
        heroInfo = FindObjectOfType<HeroInfo>();
        inttranslator = FindObjectOfType<IntTranslator>();
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
                Debug.Log($"Color found at index: {index}"+"clearNumber: "+ clearNumber);
            }
            else
            {
                Debug.Log("Color not found in RandomColors."+ Colortofind);
            }
        }
        else
        {
            Debug.LogError("SpawnBlock instance not found."+ Colortofind);
        }
        heroInfo.ExecuteBehavior(index, clearNumber);
    }

    public void DropDownBlock(float second)
    //we can just set the bool when doing remove rebug instead of using coroutine.
    {
        StartCoroutine(DropDownAfterDelay(second));
       DropCountDown= true;
    }

    private IEnumerator DropDownAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (DropCountDown )
        {
            BlockManager blockManager = FindObjectOfType<BlockManager>();
            blockManager.DropDown();
        }
        DropCountDown = false;
    }

    public void LockRotation()
    {
        RotationLocked = true;
        StartCoroutine(ResetLockRotation());
    }

    public void LockRotationForNextBlock()
    //set the bool instead of method, aviod to reset in 3 seconds.
    {
        LockNextBlockRotation = true;
    }
    private IEnumerator ResetLockRotation()
    {
        yield return new WaitForSeconds(3);
        RotationLocked = false;
    }

    public void PuaseEnemyActionBar(float PausePeriod)
    {
         PauseTime=+PausePeriod;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.PauseActionBar = true;
            StartCoroutine(ResumeEnemyActionBarAfterDelay());
        }
    }

    private IEnumerator ResumeEnemyActionBarAfterDelay()
    {
        while (PauseTime > 0)
        {
            yield return new WaitForSeconds(1);
            PauseTime -= 1;
        }
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.PauseActionBar = false;
        }
    }
    public void refreshSelectionBlocks()
    {
        selectionTool= FindObjectOfType<SelectionTool>();
    }
}
