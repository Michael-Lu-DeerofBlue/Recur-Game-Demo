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
    public void addToFall(int index)//���index���������������״��
    {
        stillFalling = true;

        int Position = threeBlockList.IndexOf(index);// position������������ĵڼ���
        Color color = Translator.GetComponent<IntTranslator>().intToColor(threeColorList[Position]); // ��������ĵڡ�position��������ɫ
        if (BattleManager.refreshedBlocks) 
        {
            color = Translator.GetComponent<IntTranslator>().intToColor(threeColorList[threeBlockList.FindIndex(x => x == index)]);
            BattleManager.refreshedBlocks = false;
        }
        threeBlockList = DrawRandomIntegers(blockList, 3);//�����ȷ����һ���µ�����choice����״
        for (int i = 0; i < threeBlockList.Count; i++)
        {
            threeColorList[i] = actionBlockDictionary[threeBlockList[i]];//����������Ƕ�Ӧ����ɫ
        }

        SelectionUI.GetComponent<SelectionToolUI>().UpdateChoiceBlocks();
        Spawner.GetComponent<SpawnBlock>().SpawnNewBlock(index, color, actionBlockDictionary[index]);//��Spawn��Spawn��grid�ϣ�index��������״��color������Ҫ��grid�����ɵķ������ɫ
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
