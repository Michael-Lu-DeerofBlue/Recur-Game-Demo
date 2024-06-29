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
        //这是用来找颜色的
        Color color = Translator.GetComponent<IntTranslator>().intToColor(actionBlockDictionary[index]);
        if (BattleManager.refreshedBlocks) //这也是用来找颜色的，但是是在refreshed block，就是直接让颜色不变，形状变的时候用来找颜色的
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
}
