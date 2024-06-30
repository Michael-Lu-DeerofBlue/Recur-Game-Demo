using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SelectionTool : MonoBehaviour
{
    public Dictionary<int, int> actionBlockDictionary = new Dictionary<int, int>();
    public List<int> blockList = new List<int>();
    public List<int> threeBlockList = new List<int>();
    public List<int> threeColorList = new List<int>();
    public int storageBlock = 7;
    public GameObject SelectionUI;
    public GameObject Spawner;
    public GameObject Translator;
    public bool stillFalling;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var kvp in actionBlockDictionary)
        {
            blockList.Add(kvp.Key);
        }

        threeBlockList = DrawRandomIntegers(blockList, 3);
        for (int i = 0; i < threeBlockList.Count; i++)
        {
            threeColorList.Add(actionBlockDictionary[threeBlockList[i]]);
        }
        SelectionUI.GetComponent<SelectionToolUI>().UpdateChoiceBlocks();

    }

    public void addToStorage(int index)
    {
        storageBlock = index;
        SelectionUI.GetComponent<SelectionToolUI>().UpdateStorageBlocks();
    }
    public void addToFall(int index)//这个index是用来代表方块的形状的
    {
        stillFalling = true;

        int Position = threeBlockList.IndexOf(index);// position就是三个里面的第几个
        Color color = Translator.GetComponent<IntTranslator>().intToColor(threeColorList[Position]); // 三个里面的第“position”个的颜色
        if (BattleManager.refreshedBlocks) 
        {
            color = Translator.GetComponent<IntTranslator>().intToColor(threeColorList[threeBlockList.FindIndex(x => x == index)]);
            BattleManager.refreshedBlocks = false;
        }
        threeBlockList = DrawRandomIntegers(blockList, 3);//这个在确定下一行新的三个choice的形状
        for (int i = 0; i < threeBlockList.Count; i++)
        {
            threeColorList[i] = actionBlockDictionary[threeBlockList[i]];//这个在找他们对应的颜色
        }

        SelectionUI.GetComponent<SelectionToolUI>().UpdateChoiceBlocks();
        Spawner.GetComponent<SpawnBlock>().SpawnNewBlock(index, color, actionBlockDictionary[index]);//这Spawn是Spawn在grid上，index代表方块形状，color代表我要在grid上生成的方块的颜色
    }

    List<int> DrawRandomIntegers(List<int> list, int count)
    {
        List<int> result = new List<int>();
        List<int> tempList = new List<int>(list);
        System.Random random = new System.Random();

        for (int i = 0; i < count; i++)
        {
            if (tempList.Count == 0) break;

            int randomIndex = random.Next(tempList.Count);
            result.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }
        
        return result;
    }
    public void ReShapeSelectionBlocks()
    {
        List<int> allBlockList = new List<int> {0, 1, 2, 3, 4, 5, 6};
        threeBlockList = DrawRandomIntegers(allBlockList, 3);
        SelectionUI.GetComponent<SelectionToolUI>().UpdateChoiceBlocks();
    }


    public void AddDebuffBlock(int Num, int ColorIndex)
    {
        // Ensure the Translator reference is assigned
        if (Translator == null) return;

        IntTranslator translator = Translator.GetComponent<IntTranslator>();

        // Iterate through threeColorList
        for (int i = 0; i < threeColorList.Count; i++)
        {
            // Get the color index from threeColorList
            int currentColorIndex = threeColorList[i];

            // Check if the current color index is between 0 and 6 (inclusive)
            if (currentColorIndex >= 0 && currentColorIndex <= 6&& Num>0 )
            {
                // Change the current color index to ColorIndex
                threeColorList[i] = ColorIndex;
                Num--;

            }
        }

        // Update the choice blocks in the UI to reflect the color change
        SelectionUI.GetComponent<SelectionToolUI>().UpdateChoiceBlocks();
    }


}
