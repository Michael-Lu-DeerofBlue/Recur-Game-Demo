using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntTranslator : MonoBehaviour
{
    public GameObject[] BlockShapes;
    public GameObject[] GBlockShapes;
    public Color[] Colors;
    public Sprite[] LongSwordSkillIcon;
    // Start is called before the first frame update
    public GameObject intToBlock(int index)
    {
        return BlockShapes[index];
    }

    public GameObject intToGBlock(int index)
    {
        return GBlockShapes[index];
    }

    // Update is called once per frame
    public Color intToColor(int index)
    {
        return Colors[index];
    }
}
