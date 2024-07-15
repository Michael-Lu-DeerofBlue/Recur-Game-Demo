using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StickerInfo;

public class StickerInfo : MonoBehaviour
{
        public struct StickerData
        {
            public int IndexShape;
            public string StickerName;
            public Vector3 position1;

            public StickerData(int number, string str1, Vector3 position1)
            {
                this.IndexShape = number;
                this.StickerName = str1;
                this.position1 = position1;
            }
        }
    public List<StickerData> dataList;
    private InSelectionBar inSelectionBar;
    void Start()
        {
        dataList = new List<StickerData>();
        inSelectionBar = FindObjectOfType<InSelectionBar>();
        dataList = ES3.Load<List<StickerData>>("StickerData");
        foreach (var data in dataList)
        {
            Debug.Log(data.StickerName);
        }
    }
}
