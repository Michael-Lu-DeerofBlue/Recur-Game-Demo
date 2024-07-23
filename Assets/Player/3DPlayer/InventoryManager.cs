using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static Dictionary<string, int> ConsumablesInventory = new Dictionary<string, int>();
    public static Dictionary<string, int> StickersInventory = new Dictionary<string, int>();
    public static bool sprintB;
    public static bool bootsB;
    public static bool grappleB;
    public static bool keyB;
    public static bool flashlightB;
    public static  bool longswordB;
    public static bool gadgetsB;
    public static float CombatHP;
    public static int MoveHP;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ES3.KeyExists("Consumables"))
        {
            ConsumablesInventory = ES3.Load<Dictionary<string, int>>("Consumables");
        }
        if (ES3.KeyExists("Stickers"))
        {
            StickersInventory = ES3.Load<Dictionary<string, int>>("Stickers");
        }
        
        //Important Items
        if (ES3.KeyExists("Sprint"))
        {
            sprintB = ES3.Load<bool>("Sprint");
        }
        if (ES3.KeyExists("MagBoots"))
        {
            bootsB = ES3.Load<bool>("MagBoots");
        }
        if (ES3.KeyExists("GrapplingGun"))
        {
            grappleB = ES3.Load<bool>("GrapplingGun");
        }
        if (ES3.KeyExists("Key"))
        {
            keyB = ES3.Load<bool>("Key");
        }
        if (ES3.KeyExists("Flashlight"))
        {
            flashlightB = ES3.Load<bool>("Flashlight");
        }
        if (ES3.KeyExists("Longsword"))
        {
            longswordB = ES3.Load<bool>("Longsword");
        }

        //Vitals
        if (ES3.KeyExists("MoveHP"))
        {
            MoveHP = ES3.Load<int>("MoveHP"); 
        }
        if (ES3.KeyExists("CombatHP"))
        {
            CombatHP = ES3.Load<float>("CombatHP");
        }

        //Stages of UI
        if (ES3.KeyExists("Gadgets"))
        {
            gadgetsB = ES3.Load<bool>("Gadgets");
        }
    }
    public void AddHealth(float number)
    {
        float combatHealth = ES3.Load<float>("CombatHP");
        combatHealth = combatHealth + number;
        ES3.Save("CombatHP", combatHealth);
        Debug.Log(number);
    }
    public void AddConsumables(string name, int number)
    {
        Dictionary<string, int> consumablesInt = ES3.Load<Dictionary<string, int>>("Consumables");
        consumablesInt[name] = consumablesInt[name] + number;
        ES3.Save("Consumables", consumablesInt);
    }
    public void AddSticker(string name, int number)
    {
        Dictionary<string, int> stickersInt = ES3.Load<Dictionary<string, int>>("Stickers");
        stickersInt[name] = stickersInt[name] + number;
        ES3.Save("Stickers", stickersInt);
    }
}
