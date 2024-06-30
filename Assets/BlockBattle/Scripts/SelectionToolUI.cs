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
    public void UpdateStorageBlocks(int storedBlock, int storedColor)
    {
        storageBlock = selectionToolProcessor.GetComponent<SelectionTool>().storageBlock;
        Destroy(previousGeneratedStorageObject);

        GameObject sblock = Translator.GetComponent<IntTranslator>().intToBlock(storedBlock);
        GameObject storage = Instantiate(sblock, storagePlaceholder.transform.position, storagePlaceholder.transform.rotation);
        storage.GetComponent<BlockStageController>().inSelection = true;
        storage.GetComponent<InSelectionBar>().inStorage = true;
        storage.GetComponent<BlockStageController>().index = storageBlock;

        GiveColor(storedBlock, storage, storedColor);
        previousGeneratedStorageObject = storage;
    }



    public void UpdateChoiceBlocks()
    {
        foreach (GameObject gameObject in previousGeneratedObject)
        {
            Destroy(gameObject);
        }
        previousGeneratedObject.Clear();

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


    private void GiveColor(int index, GameObject block, int colorCode)
    {
        Color color;
        if (index <= 6)
        {
            color = Translator.GetComponent<IntTranslator>().intToColor(colorCode);
        }
        else
        {
            color = Translator.GetComponent<IntTranslator>().intToColor(index);
        }

        Renderer[] renderers = block.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }


    // Method to remove a block at a given position
    public void RemoveBlockAtPosition(int position)
    {
        if (previousGeneratedObject != null && previousGeneratedObject.Count > position)
        {
            Destroy(previousGeneratedObject[position]);
            previousGeneratedObject.RemoveAt(position);
        }
    }

}
