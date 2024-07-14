using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player3DUI : MonoBehaviour
{
    public GameObject sprint;
    public GameObject flashlight;
    public GameObject longsword;
    public GameObject gadgets;
    public List<GameObject> moveHPs;
    public Image CombatHPbar;
    public bool sprintB;
    public bool flashlightB;
    public bool longswordB;
    public bool gadgetsB;
    public float CombatHP;
    public int MoveHP;
    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    public void UpdateUI()
    {
        LoadB();
        sprint.SetActive(sprintB);
        longsword.SetActive(longswordB);
        flashlight.SetActive(flashlightB);
        gadgets.SetActive(gadgetsB);
        CombatHPbar.fillAmount = CombatHP/100f;
        for (int i = 0; i < MoveHP; i++)
        {
            moveHPs[i].SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        UpdateUI();
    }

    private void LoadB()
    {
        sprintB = InventoryManager.sprintB;
        flashlightB = InventoryManager.flashlightB;
        longswordB = InventoryManager.longswordB;
        gadgetsB = InventoryManager.gadgetsB;
        CombatHP = InventoryManager.CombatHP;
        MoveHP = InventoryManager.MoveHP;
    }


}
