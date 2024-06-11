using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private HeroInfo heroInfo;
    public float Timer;
    public SelectionTool selectionTool;

    //debug type:
    public bool RotationLocked=false;
    public bool LockNextBlockRotation = false;


    void Start()
    {
        heroInfo = FindObjectOfType<HeroInfo>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReceColorMessage(int colorCode, int clearNumber)
    {
        heroInfo.ExecuteBehavior(colorCode, clearNumber);
    }

    public void LockRotation()
    {
        RotationLocked = true;
        StartCoroutine(ResetLockRotation());
    }

    private IEnumerator ResetLockRotation()
    {
        yield return new WaitForSeconds(3);
        RotationLocked = false;
    }



    public void LockRotationForNextBlock()
    {
        LockNextBlockRotation = true;
    }
    public void refreshSelectionBlocks()
    {
        selectionTool= FindObjectOfType<SelectionTool>();
    }
}
