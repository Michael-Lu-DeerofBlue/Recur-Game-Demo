using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerInfo : MonoBehaviour
{
        public struct StickerData
        {
            public int IndexShape;
            public string StickerName;
            public Vector3 position1;
            public Vector3 position2;

            public StickerData(int number, string str1, Vector3 position1, Vector3 position2)
            {
                this.IndexShape = number;
                this.StickerName = str1;
                this.position1 = position1;
                this.position2 = position2;
            }
        }
    public List<StickerData> dataList;
    private InSelectionBar inSelectionBar;
    void Start()
        {
        dataList = new List<StickerData>();
        inSelectionBar = FindObjectOfType<InSelectionBar>();

        dataList.Add(new StickerData(4, "StickerExample", new Vector3(0, 0, 0), new Vector3(-1, 0, 0)));
    }
}
