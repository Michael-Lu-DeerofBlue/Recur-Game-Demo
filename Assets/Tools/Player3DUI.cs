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
        if (ES3.KeyExists("Sprint"))
        {
            sprintB = ES3.Load<bool>("Sprint");
        }
        if (ES3.KeyExists("Flashlight"))
        {
            flashlightB = ES3.Load<bool>("Flashlight");
        }
        if (ES3.KeyExists("Longsword"))
        {
            longswordB = ES3.Load<bool>("Longsword");
        }
        if (ES3.KeyExists("MoveHP"))
        {
            MoveHP =  ES3.Load<int>("MoveHP");
        }
        if (ES3.KeyExists("CombatHP"))
        {
            CombatHP = ES3.Load<float>("CombatHP");
        }
        if (ES3.KeyExists("Gadgets"))
        {
            gadgetsB = ES3.Load<bool>("Gadgets");
        }
    }


}
