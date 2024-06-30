using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class SelectionToolUI : MonoBehaviour
{
    public GameObject selectionToolProcessor;
    public List<int> threeBlockList = new List<int>();
    public List<int> threeColorList = new List<int>();
    public int storageBlock;
    public GameObject[] blockPlaceholder;
    public GameObject storagePlaceholder;
    public GameObject Translator;
    public List<GameObject> previousGeneratedObject;
    public GameObject previousGeneratedStorageObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateChoiceBlocks()
    {
        foreach (GameObject gameObject in previousGeneratedObject)
        {
            Destroy(gameObject);
        }
        threeBlockList = selectionToolProcessor.GetComponent<SelectionTool>().threeBlockList;
        threeColorList = selectionToolProcessor.GetComponent<SelectionTool>().threeColorList;
        if (threeBlockList != null)
        {
            for (int i = 0; i < threeBlockList.Count; i++)
            {
                GameObject block = Translator.GetComponent<IntTranslator>().intToBlock(threeBlockList[i]);
                GameObject symbol = Instantiate(block, blockPlaceholder[i].transform.position, blockPlaceholder[i].transform.rotation);
                symbol.GetComponent<BlockStageController>().inSelection = true;
                symbol.GetComponent<BlockStageController>().index = threeBlockList[i];
                GiveColor(threeBlockList[i], symbol, threeColorList[i]);
                previousGeneratedObject.Add(symbol);
            }
        }
    }

    public void UpdateStorageBlocks()
    {
        storageBlock = selectionToolProcessor.GetComponent<SelectionTool>().storageBlock;
        Destroy(previousGeneratedStorageObject);
        GameObject sblock = Translator.GetComponent<IntTranslator>().intToBlock(storageBlock);
        GameObject storage = Instantiate(sblock, storagePlaceholder.transform.position, storagePlaceholder.transform.rotation);
        storage.GetComponent<BlockStageController>().inSelection = true;
        storage.GetComponent<InSelectionBar>().inStorage = true;
        storage.GetComponent<BlockStageController>().index = storageBlock;
        int colorCode = selectionToolProcessor.GetComponent<SelectionTool>().actionBlockDictionary[storageBlock];
        GiveColor(storageBlock, storage, colorCode);
        previousGeneratedStorageObject = storage;
    }

    private void GiveColor(int index, GameObject block, int colorCode)
    {
        if (index <= 6)
        {
            Color color = Translator.GetComponent<IntTranslator>().intToColor(colorCode);
            Renderer[] renderers = block.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = color;
            }
        }
        else
        {
            Color color = Translator.GetComponent<IntTranslator>().intToColor(index);
            Renderer[] renderers = block.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = color;
            }
        }
    }

}
