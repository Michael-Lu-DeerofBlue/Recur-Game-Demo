using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static StickerInfo;

public class StickerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int block;
    public string stickerName;
    public GameObject stickerManger;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int difference = 0;
        if (AddStickerManager.inUseStickersInventory.ContainsKey(stickerName) && AddStickerManager.StickersInventory.ContainsKey(stickerName))
        {
           difference = AddStickerManager.StickersInventory[stickerName] - AddStickerManager.inUseStickersInventory[stickerName];
        }
        if (difference > 0)
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public void AddMe()
    {
        if (AddStickerManager.inStickerSelection)
        {
            stickerManger.GetComponent<AddStickerManager>().AddStickerandSave(stickerName);
        }
    }
}
