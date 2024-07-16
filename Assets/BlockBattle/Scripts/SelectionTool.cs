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
    private SoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        soundManager=FindAnyObjectByType<SoundManager>();
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

        // Proceed with the original logic
        storageBlock = index;

        int position = threeBlockList.IndexOf(index);
        int storedBlock = threeBlockList[position];
        int storedColorIndex = threeColorList[position];

        // Remove the block from the selection
        threeBlockList.RemoveAt(position);
        threeColorList.RemoveAt(position);


        // Update the UI to reflect changes
        SelectionUI.GetComponent<SelectionToolUI>().UpdateStorageBlocks(storedBlock, storedColorIndex);
        SelectionUI.GetComponent<SelectionToolUI>().UpdateChoiceBlocks();
        soundManager.PlaySound("StoreActionBlock");
    }

    public void addToFall(int index, bool Storage)
    {
        stillFalling = true;
        if (!Storage)
        {
            int Position = threeBlockList.IndexOf(index);
            Color color = Translator.GetComponent<IntTranslator>().intToColor(threeColorList[Position]); // 三个里面的第“position”个的颜色


            // 保存原始的形状和颜色列表
            List<int> originalBlockList = new List<int>(threeBlockList);
            List<int> originalColorList = new List<int>(threeColorList);

            // 生成新的三个choice的形状
            threeBlockList = DrawRandomIntegers(blockList, 3);

            for (int i = 0; i < originalBlockList.Count; i++)  
            {
                if (originalColorList[i] >= 7 && i!=Position) // 如果有两个debuff方块， i就是for loop里面搜到的那个， Position就是选择的那个，只有选择的会被替换。
                {
                    threeBlockList[i] = originalBlockList[i];
                    threeColorList[i] = originalColorList[i];

                }
                else
                {

                    threeColorList[i] = actionBlockDictionary[threeBlockList[i]];
                }
            }
            SelectionUI.GetComponent<SelectionToolUI>().UpdateChoiceBlocks();
            Spawner.GetComponent<SpawnBlock>().SpawnNewBlock(index, color, threeColorList[Position]); // 这Spawn是Spawn在grid上，index代表方块形状，color代表我要在grid上生成的方块的颜色
            soundManager.PlaySound("ActionBlockChoose");
        }
        else
        {
            SelectionToolUI selectionToolUI = SelectionUI.GetComponent<SelectionToolUI>();

            if (selectionToolUI.previousGeneratedStorageObject != null)
            {
                // Get the shape (index) of the previous generated storage object
                int storedShapeIndex = selectionToolUI.previousGeneratedStorageObject.GetComponent<BlockStageController>().index;

                // Find the Renderer component in the child objects
                Renderer renderer = selectionToolUI.previousGeneratedStorageObject.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    // Get the color of the previous generated storage object
                    Color storedColor = renderer.material.color;
                    Spawner.GetComponent<SpawnBlock>().SpawnNewBlock(storedShapeIndex, storedColor, actionBlockDictionary[storedShapeIndex]);
                }
            }
        }
    }
    List<int> DrawRandomIntegers(List<int> list, int count)
    {
        List<int> result = new List<int>();
        List<int> tempList = new List<int>(list);
        System.Random random = new System.Random();

        while (result.Count < count && tempList.Count > 0)
        {
            int randomIndex = random.Next(tempList.Count);
            int randomValue = tempList[randomIndex];

            // Ensure no duplicates
            if (!result.Contains(randomValue) && !threeBlockList.Contains(randomValue))
            {
                result.Add(randomValue);
                tempList.RemoveAt(randomIndex);
            }
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
        bool foundInRange = false;
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
                foundInRange = true;

            }
        }
           if (!foundInRange && Num > 0)
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(threeColorList.Count);
        threeColorList[randomIndex] = ColorIndex;
    }

        // Update the choice blocks in the UI to reflect the color change
        SelectionUI.GetComponent<SelectionToolUI>().UpdateChoiceBlocks();
    }


}
