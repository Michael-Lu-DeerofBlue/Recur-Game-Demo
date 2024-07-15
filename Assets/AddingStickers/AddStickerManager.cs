using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.UI;
using static StickerInfo;

public class AddStickerManager : MonoBehaviour
{
    public static List<StickerData> stickerData;
    public static Dictionary<string, int> StickersInventory = new Dictionary<string, int>();
    public static Dictionary<string, int> inUseStickersInventory = new Dictionary<string, int>();
    public int block;
    public string stickerName;
    public Vector3 position;
    public static bool inStickerSelection;
    public List<TextMeshProUGUI> stickertTexts;
    // Start is called before the first frame update

    private void Awake()
    {
        StickersInventory = InventoryManager.StickersInventory;
    }
    void Start()
    {
        GetStickerData();
        UpdatePrivateDict();
        StickersInventory = InventoryManager.StickersInventory;
        BroadcastMessage("UpdateAppearence");
    }

    private void Update()
    {
        StickersInventory = InventoryManager.StickersInventory;
        for (int i = 0; i < stickertTexts.Count; i++)
        {
            switch (i)
            {
                case 0:
                    stickertTexts[i].text = (StickersInventory["Critical"]-inUseStickersInventory["Critical"]).ToString() + "/" + StickersInventory["Critical"].ToString();
                    break;
                case 1:
                    stickertTexts[i].text = (StickersInventory["Pierce"]-inUseStickersInventory["Pierce"]).ToString() + "/" + StickersInventory["Pierce"].ToString();
                    break;
                case 2:
                    stickertTexts[i].text = (StickersInventory["Sober"]-inUseStickersInventory["Sober"]).ToString() + "/" + StickersInventory["Sober"].ToString();
                    break;
                case 3:
                    stickertTexts[i].text = (StickersInventory["Swordmaster"]-inUseStickersInventory["Swordmaster"]).ToString() + "/" + StickersInventory["Swordmaster"].ToString();
                    break;
                case 4:
                    stickertTexts[i].text = (StickersInventory["Gunslinger"] - inUseStickersInventory["Gunslinger"]).ToString() + "/" + StickersInventory["Gunslinger"].ToString();
                    break;
                default:
                    break;
            }
        }
    }

    public void GetStickerData()
    {
        stickerData = ES3.Load<List<StickerData>>("StickerData");
    }

    void UpdatePrivateDict()
    {
        inUseStickersInventory = new Dictionary<string, int>
        {
            {"Critical", 0 },
            {"Pierce", 0 },
            {"Sober", 0 },
            {"Swordmaster", 0 },
            {"Gunslinger", 0 },
        };
        foreach (StickerData sticker in stickerData)
        {
            switch (sticker.StickerName)
            {
                case "Critical":
                    UpdateInUseStickerDictionary(sticker.StickerName);
                    break;
                case "Pierce":
                    UpdateInUseStickerDictionary(sticker.StickerName);
                    break;
                case "Sober":
                    UpdateInUseStickerDictionary(sticker.StickerName);
                    break;
                case "Swordmaster":
                    UpdateInUseStickerDictionary(sticker.StickerName);
                    break;
                case "Gunslinger":
                    UpdateInUseStickerDictionary(sticker.StickerName);
                    break;
                default:
                    break;
            }
        }
    }

    void UpdateInUseStickerDictionary(string text)
    {
        int count = inUseStickersInventory[text] + 1;
        inUseStickersInventory[text] = count;
    }
    public void SaveStickerData()
    {
        ES3.Save("StickerData", stickerData);
        UpdatePrivateDict();
        gameObject.BroadcastMessage("UpdateAppearence");
    }
    public void DeletePositionBlock(Vector3 stickerPosition, int blockCode)
    {
        for(int i = 0; i < stickerData.Count; i++)
        {
            if (stickerData[i].IndexShape == blockCode) //block match
            {
                if (stickerData[i].position1 == stickerPosition)
                {
                    stickerData.Remove(stickerData[i]);
                    SaveStickerData();
                    BroadcastMessage("EnableInteractive");
                    break;
                }
            }
        }
    }

    public void AddStickerToStickerData()
    {
        stickerData.Add(new StickerData(block, stickerName, position));
    }
    public void AddPositionBlock(Vector3 vector, int num)
    {
        stickerData = ES3.Load<List<StickerData>>("StickerData");
        position = vector;
        block = num;
        InStickerSelection();
    }
    public void AddStickerandSave(string stick)
    {
        stickerName = stick;
        AddStickerToStickerData();
        SaveStickerData();
        ExitSecletion();
    }

    public void InStickerSelection()
    {
        inStickerSelection = true; //GetReady
        gameObject.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
        BroadcastMessage("DisableInteractive");
    }

    public void ExitSecletion()
    {
        inStickerSelection=false;
        position = new Vector3(0,0,0);
        block = 0;
        stickerName = null;
        gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        BroadcastMessage("EnableInteractive");
        BroadcastMessage("UpdateAppearence");

    }
}
