using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuaseMenu : MonoBehaviour
{
    public Button Backpack;
    public Button Equip;
    public Button Collection;
    public Button System;
    public MainUIManager mainUIManager;
    // Start is called before the first frame update
    void Start()
    {
        mainUIManager = FindObjectOfType<MainUIManager>();
        Backpack.onClick.AddListener(() => ButtonClicked("Backpack"));
        Equip.onClick.AddListener(() => ButtonClicked("Equip"));
        Collection.onClick.AddListener(() => ButtonClicked("Collection"));
        System.onClick.AddListener(() => ButtonClicked("System"));
    }

    void ButtonClicked(string buttonName)
    {
        mainUIManager.ButtonClicked(buttonName);
    }
    void Update()
    {
        
    }
}
